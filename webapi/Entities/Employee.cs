using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace webapi.Entities
{
    public                  class Employee
    {
        [Key]
        public Guid Id{ get; set; }= Guid.NewGuid();
        public string? Name{ get; set; }
        public DateTime?  CreatedDate{ get; set; }
        public DateOnly? DOB{ get; set; }
        public string? Position{ get; set; }
        public string? Department{ get; set; }

        public string? EmailAddress{ get; set; }
    }
}