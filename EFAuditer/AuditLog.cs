using System;

namespace EFAuditer
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int OriginalId { get; set; }
        public string EntityType { get; set; }
        public string EventType { get; set; }
        public string AuditData { get; set; }
        public DateTime AuditDateTime { get; set; }
        public string AuditUser { get; set; }
    }
}