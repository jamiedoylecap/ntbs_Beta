﻿using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using ntbs_service.Properties;

namespace ntbs_service.Pages
{
    public class Health : PageModel
    {
        public Health(IConfiguration configuration)
        {
            Release = configuration.GetValue<string>(Constants.Release);
            
            AuditEnabled = configuration.GetValue<bool>(Constants.AuditEnabledConfigValue);
            
            // Note the value negation, since we're turning mocked => enabled
            ClusterMatchingEnabled = !configuration.GetSection(Constants.ClusterMatchingConfig)
                .GetValue<bool>(Constants.ClusterMatchingConfigMockOut);
            
            // Note the value negation, since we're turning mocked => enabled
            ReferenceLabResultsEnabled = !configuration.GetSection(Constants.ReferenceLabResultsConfig)
                .GetValue<bool>(Constants.ReferenceLabResultsConfigMockOut);
            
            var reportingDbString = configuration.GetConnectionString(Constants.DbConnectionStringReporting);
            // Note the value negation, since we're turning lack of connection string  => enabled
            ReportingServicesEnabled = !string.IsNullOrEmpty(reportingDbString);
            
            configuration.GetSection("ScheduledJobsConfig").Bind(ScheduledJobsConfig);
        }

        public string Release { get; }
        public bool AuditEnabled { get; }
        public bool ClusterMatchingEnabled { get; }
        public bool ReferenceLabResultsEnabled { get; }
        public bool ReportingServicesEnabled { get; }

        public ScheduledJobsConfig ScheduledJobsConfig { get; } = new ScheduledJobsConfig();
    }
}
