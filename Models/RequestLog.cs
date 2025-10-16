using System;
using System.ComponentModel.DataAnnotations;

namespace accounts_sf_sa.Models
{
    public class RequestLog
    {
        [Key]
        public int Id { get; set; }
        public string Operation { get; set; } = string.Empty;
        public DateTimeOffset TimestampUtc { get; set; } = DateTimeOffset.UtcNow;
        public bool Success { get; set; }
        public string ReferenceId { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }
}
