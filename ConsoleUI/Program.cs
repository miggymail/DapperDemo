using Dapper;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace ConsoleUI
{
    class Program
    {
        static void Main()
        {
            MapMultipleObjects();

            //MapMultipleObjectsWithParameters();

            //MultipleSets();

            //MultipleSetsWithParameters("Banner", "7898", "10");

            //OutputParamers("Carol", "Danvers", "ACCT006");

            //WithTransaction("Natasha", "Romanoff", "ACCT007");

            //InsertCollectionOfData();

            Console.ReadLine();
        }

        static void MapMultipleObjects()
        {
            using IDbConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDb"].ConnectionString);
            string sql = @"SELECT 
	                           C.Id, C.FirstName, C.LastName, C.AccountNumber, 
                                P1.*, P2.*,
	                            A1.*, A2.*
                            FROM dbo.[Customer] C
                            LEFT OUTER JOIN dbo.[Phone] P1 ON C.MobileNo = P1.Id
                            LEFT OUTER JOIN dbo.[Phone] P2 ON C.WorkNo = P2.Id
                            LEFT OUTER JOIN dbo.[Address] A1 ON C.HomeAddress = A1.Id
                            LEFT OUTER JOIN dbo.[Address] A2 ON C.WorkAddress = A2.Id;";
            var customers = cnn.Query<CustomerModel, PhoneModel, PhoneModel, AddressModel, AddressModel, CustomerModel>(sql, (customer, phone1, phone2, address1, address2) =>
            {
                customer.MobileNo = phone1;
                customer.WorkNo = phone2;
                customer.HomeAddress = address1;
                customer.WorkAddress = address2;

                return customer;

            }, splitOn: "Id,Id");

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - Account #{c.AccountNumber} ");
                Console.WriteLine($"\tHome Address: {c.HomeAddress.Street} {c.HomeAddress.City}, {c.HomeAddress.State} {c.HomeAddress.ZipCode}  Mobile No: {c.MobileNo.PhoneNumber}");
                Console.WriteLine($"\tWork Address: {c.WorkAddress.Street} {c.WorkAddress.City}, {c.WorkAddress.State} {c.WorkAddress.ZipCode}  Work No: {c.WorkNo.PhoneNumber}");
            }
        }

        static void MapMultipleObjectsWithParameters()
        {
            using IDbConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDb"].ConnectionString);
            var ln = new
            {
                LastName = "Stark"
            };

            string sql = @"SELECT 
	                           C.Id, C.FirstName, C.LastName, C.AccountNumber, 
                                P1.*, P2.*,
	                            A1.*, A2.*
                            FROM dbo.[Customer] C
                            LEFT OUTER JOIN dbo.[Phone] P1 ON C.MobileNo = P1.Id
                            LEFT OUTER JOIN dbo.[Phone] P2 ON C.WorkNo = P2.Id
                            LEFT OUTER JOIN dbo.[Address] A1 ON C.HomeAddress = A1.Id
                            LEFT OUTER JOIN dbo.[Address] A2 ON C.WorkAddress = A2.Id
                            WHERE C.LastName = @LastName;";
            var customers = cnn.Query<CustomerModel, PhoneModel, PhoneModel, AddressModel, AddressModel, CustomerModel>(sql, (customer, phone1, phone2, address1, address2) =>
            {
                customer.MobileNo = phone1;
                customer.WorkNo = phone2;
                customer.HomeAddress = address1;
                customer.WorkAddress = address2;

                return customer;

            }, param: ln, splitOn: "Id,Id");

            foreach (var c in customers)
            {
                Console.WriteLine($"{c.FirstName} {c.LastName} - Account #{c.AccountNumber} ");
                Console.WriteLine($"\tHome Address: {c.HomeAddress.Street} {c.HomeAddress.City}, {c.HomeAddress.State} {c.HomeAddress.ZipCode}  Mobile No: {c.MobileNo.PhoneNumber}");
                Console.WriteLine($"\tWork Address: {c.WorkAddress.Street} {c.WorkAddress.City}, {c.WorkAddress.State} {c.WorkAddress.ZipCode}  Work No: {c.WorkNo.PhoneNumber}");
            }
        }

        static void MultipleSets()
        {
            using IDbConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDb"].ConnectionString);
            string sql = @"SELECT C.Id, C.FirstName, C.LastName, C.AccountNumber FROM dbo.[Customer] C; 
                                SELECT P.* FROM dbo.[Phone] P;
	                            SELECT A.* FROM dbo.[Address] A;";


            List<CustomerModel> customers = null;
            List<PhoneModel> phones = null;
            List<AddressModel> addresses = null;

            using (var qry = cnn.QueryMultiple(sql))
            {
                customers = qry.Read<CustomerModel>().ToList();
                phones = qry.Read<PhoneModel>().ToList();
                addresses = qry.Read<AddressModel>().ToList();
            }

            Console.WriteLine("Customers List");
            foreach (var c in customers)
            {
                Console.WriteLine($"# {c.Id}. {c.FirstName} {c.LastName} - Account #{c.AccountNumber} ");
            }

            Console.WriteLine("\nPhone List");
            foreach (var p in phones)
            {
                Console.WriteLine($"# {p.Id}. Phone Number : {p.PhoneNumber}");
            }

            Console.WriteLine("\nAddress List");
            foreach (var a in addresses)
            {
                Console.WriteLine($"# {a.Id}. Address: {a.Street} {a.City}, {a.State} {a.ZipCode}");
            }
        }

        static void MultipleSetsWithParameters(string lastname, string phoneending, string streetwithnumber)
        {
            using IDbConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDb"].ConnectionString);
            string sql = @"SELECT C.Id, C.FirstName, C.LastName, C.AccountNumber FROM dbo.[Customer] C WHERE C.LastName = @LastName; 
                                SELECT P.* FROM dbo.[Phone] P WHERE P.PhoneNumber LIKE '%' + @PhoneEnding;
	                            SELECT A.* FROM dbo.[Address] A WHERE A.Street LIKE '%' + @StreetWithNumber + '%';";

            var @params = new
            {
                LastName = lastname,
                PhoneEnding = phoneending,
                StreetWithNumber = streetwithnumber
            };

            List<CustomerModel> customers = null;
            List<PhoneModel> phones = null;
            List<AddressModel> addresses = null;

            using (var qry = cnn.QueryMultiple(sql, @params))
            {
                customers = qry.Read<CustomerModel>().ToList();
                phones = qry.Read<PhoneModel>().ToList();
                addresses = qry.Read<AddressModel>().ToList();
            }

            Console.WriteLine("Customers List");
            foreach (var c in customers)
            {
                Console.WriteLine($"# {c.Id}. {c.FirstName} {c.LastName} - Account #{c.AccountNumber} ");
            }

            Console.WriteLine("\nPhone List");
            foreach (var p in phones)
            {
                Console.WriteLine($"# {p.Id}. Phone Number : {p.PhoneNumber}");
            }

            Console.WriteLine("\nAddress List");
            foreach (var a in addresses)
            {
                Console.WriteLine($"# {a.Id}. Address: {a.Street} {a.City}, {a.State} {a.ZipCode}");
            }
        }

        static void OutputParamers(string firstname, string lastname, string accountnumber)
        {
            using IDbConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDb"].ConnectionString);
            //Insert Phone Numbers
            string sql = @"INSERT INTO dbo.[Phone] ([PhoneNumber]) VALUES (@PhoneNumber); 
                               SELECT @Id = @@IDENTITY;";

            var p = new DynamicParameters();
            p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
            p.Add("@PhoneNumber", "231-456-7891");

            cnn.Execute(sql, p);
            int phoneId1 = p.Get<int>("@Id");

            p = new DynamicParameters();
            p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
            p.Add("@PhoneNumber", "231-456-7892");

            cnn.Execute(sql, p);
            int phoneId2 = p.Get<int>("@Id");


            //Insert Addresses
            sql = @"INSERT INTO dbo.[Address] ([Street],[City],[State],[ZipCode]) VALUES (@Street, @City, @State, @ZipCode); 
                        SELECT @Id = @@IDENTITY;";

            p = new DynamicParameters();
            p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
            p.Add("@Street", "11 Main Street");
            p.Add("@City", "Los Angeles");
            p.Add("@State", "California");
            p.Add("@ZipCode", "90011");

            cnn.Execute(sql, p);
            int addressId1 = p.Get<int>("@Id");

            p = new DynamicParameters();
            p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
            p.Add("@Street", "12 Main Street");
            p.Add("@City", "Los Angeles");
            p.Add("@State", "California");
            p.Add("@ZipCode", "90012");

            cnn.Execute(sql, p);
            int addressId2 = p.Get<int>("@Id");

            sql = @"INSERT INTO dbo.[Customer] (FirstName, LastName, AccountNumber, MobileNo, WorkNo, HomeAddress, WorkAddress) VALUES (@FirstName, @LastName, @AccountNumber, @MobileNo, @WorkNo, @HomeAddress, @WorkAddress); 
                                SELECT @Id = @@IDENTITY;";

            p = new DynamicParameters();
            p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
            p.Add("@FirstName", firstname);
            p.Add("@LastName", lastname);
            p.Add("@AccountNumber", accountnumber);
            p.Add("@MobileNo", phoneId1);
            p.Add("@WorkNo", phoneId2);
            p.Add("@HomeAddress", addressId1);
            p.Add("@WorkAddress", addressId2);

            cnn.Execute(sql, p);
            int newId = p.Get<int>("@Id");

            Console.WriteLine($"New Customer Id #{ newId }");

            MapMultipleObjects();
        }

        static void WithTransaction(string firstname, string lastname, string accountnumber)
        {
            using IDbConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDb"].ConnectionString);
            string sql = @"INSERT INTO dbo.[Customer] (FirstName, LastName, AccountNumber, MobileNo, WorkNo, HomeAddress, WorkAddress) VALUES (@FirstName, @LastName, @AccountNumber, 0, 0, 0, 0);";

            var p = new DynamicParameters();
            p.Add("@FirstName", firstname);
            p.Add("@LastName", lastname);
            p.Add("@AccountNumber", accountnumber);

            cnn.Open();
            using (var trans = cnn.BeginTransaction())
            {
                try
                {
                    int records = cnn.Execute(sql, p, transaction: trans);

                    Console.WriteLine($"Records Updated: { records }");

                    cnn.Execute(@"UPDATE dbo.[Customer] SET Id = 1", transaction: trans);

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Console.WriteLine($"Transaction Rollback, Error : { ex.Message } \n");
                }
            }
            MapMultipleObjects();

        }

        static void InsertCollectionOfData()
        {
            using IDbConnection cnn = new SqlConnection(ConfigurationManager.ConnectionStrings["DapperDemoDb"].ConnectionString);
            
            var listofaddresses = GetAddresses();
            var listofphonenumbers = GetPhoneNumbers();
            var listofcustomers = GetCustomers();

            cnn.Open();

            var trx = cnn.BeginTransaction();

            try
            {
                string sql = @"[dbo].[spAddressUDT_Insert]";
                var addr = new
                {
                    addresses = listofaddresses.AsTableValuedParameter("AddressUDT")
                };

                int rowsAffected = cnn.Execute(sql, addr, commandType: CommandType.StoredProcedure, transaction: trx);

                Console.WriteLine($"Address rows affected: { rowsAffected }");

                sql = @"[dbo].[spPhoneUDT_Insert]";
                var phn = new
                {
                    phonenumbers = listofphonenumbers.AsTableValuedParameter("PhoneUDT")
                };

                rowsAffected = cnn.Execute(sql, phn, commandType: CommandType.StoredProcedure, transaction: trx);

                Console.WriteLine($"Phone Number rows affected: { rowsAffected }");

                sql = @"[dbo].[spCustomerUDT_Insert]";
                var cust = new
                {
                    customers = listofcustomers.AsTableValuedParameter("CustomerUDT")
                };

                rowsAffected = cnn.Execute(sql, cust, commandType: CommandType.StoredProcedure, transaction: trx);

                Console.WriteLine($"Customer rows affected: { rowsAffected }");

                trx.Commit();
            }
            catch (Exception ex)
            {
                trx.Rollback();
                Console.WriteLine($"Error : { ex.Message }");
            }

            MapMultipleObjects();
        }

        public static DataTable GetCustomers()
        {
            var customers = new DataTable();

            customers.Columns.Add("FirstName", typeof(string));
            customers.Columns.Add("LastName", typeof(string));
            customers.Columns.Add("AccountNumber", typeof(string));
            customers.Columns.Add("MobileNo", typeof(int));
            customers.Columns.Add("WorkNo", typeof(int));
            customers.Columns.Add("HomeAddress", typeof(int));
            customers.Columns.Add("WorkAddress", typeof(int));

            customers.Rows.Add("Clint", "Barton", "ACCT008", 13, 14, 13, 14);
            customers.Rows.Add("James", "Rhodes", "ACCT009", 15, 16, 15, 16);
            customers.Rows.Add("Scott", "Lang", "ACCT010", 17, 18, 17, 18);

            return customers;
        }

        public static DataTable GetPhoneNumbers()
        {
            var phonenumbers = new DataTable();

            phonenumbers.Columns.Add("PhoneNumber", typeof(string));

            phonenumbers.Rows.Add("231-456-7893");
            phonenumbers.Rows.Add("231-456-7894");
            phonenumbers.Rows.Add("231-456-7895");
            phonenumbers.Rows.Add("231-456-7896");
            phonenumbers.Rows.Add("231-456-7897");
            phonenumbers.Rows.Add("231-456-7898");

            return phonenumbers;
        }

        public static DataTable GetAddresses()
        {
            var addresses = new DataTable();

            addresses.Columns.Add("Street", typeof(string));
            addresses.Columns.Add("City", typeof(string));
            addresses.Columns.Add("State", typeof(string));
            addresses.Columns.Add("ZipCode", typeof(string));

            addresses.Rows.Add("13 Rodeo Drive", "Beverly Hills", "California", "90210");
            addresses.Rows.Add("14 Rodeo Drive", "Beverly Hills", "California", "90210");
            addresses.Rows.Add("15 Rodeo Drive", "Beverly Hills", "California", "90210");
            addresses.Rows.Add("16 Rodeo Drive", "Beverly Hills", "California", "90210");
            addresses.Rows.Add("17 Rodeo Drive", "Beverly Hills", "California", "90210");
            addresses.Rows.Add("18 Rodeo Drive", "Beverly Hills", "California", "90210");

            return addresses;
        }
    }
}

