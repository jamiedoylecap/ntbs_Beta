﻿using System;

namespace EFAuditer
{
    public class AuditLog
    {
        public int Id { get; set; }
        public string OriginalId { get; set; }
        public string EntityType { get; set; }
        public string EventType { get; set; }
        public string AuditDetails { get; set; }
        public string AuditData { get; set; }
        public DateTime AuditDateTime { get; set; }
        public string AuditUser { get; set; }
        public string RootEntity { get; set; }
        public string RootId { get; set; }

        public AuditLog Clone()
        {
            return this.MemberwiseClone() as AuditLog;
        }

        public override string ToString()
        {
            return
                $@"Log({Id}, {OriginalId}, {EntityType}, {EventType}, {AuditDetails}, {AuditData}, {AuditDateTime}, {AuditUser}, {RootEntity}, {RootId})";
        }
    }
}
