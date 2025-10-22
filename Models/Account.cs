using System;
using System.ComponentModel.DataAnnotations;

namespace acc_sf_test.Models
{
    public class Account
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Industry { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        // Salesforce Id returned by backend
        public string SalesforceId { get; set; }
    }
}
