using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DataAccessLayer.Models
{
    public class PhoneModel
    {
        [Key]
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
    }
}
