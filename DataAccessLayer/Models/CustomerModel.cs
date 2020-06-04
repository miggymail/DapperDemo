using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccessLayer.Models
{
    public class CustomerModel
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AccountNumber { get; set; }
        public PhoneModel MobileNo { get; set; }
        public PhoneModel WorkNo { get; set; }
        public AddressModel HomeAddress { get; set; }
        public AddressModel WorkAddress { get; set; }
    }
}
