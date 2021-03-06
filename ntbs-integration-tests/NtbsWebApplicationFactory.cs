﻿using System;
using EFAuditer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ntbs_integration_tests.Helpers;
using ntbs_integration_tests.TestServices;
using ntbs_service.DataAccess;
using ntbs_service.Services;
using Serilog;
using Serilog.Events;

namespace ntbs_integration_tests
{
    public class NtbsWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSerilog();
            builder.UseEnvironment("CI");

            builder.ConfigureServices(services =>
            {
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                services.AddDbContext<NtbsContext>(options =>
                {
                    options.UseInMemoryDatabase("Ntbs_Test_Db");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                services.AddDbContext<KeysContext>(options =>
                {
                    options.UseInMemoryDatabase("Ntbs_Keys_Db");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                services.AddDbContext<AuditDatabaseContext>(options =>
                {
                    options.UseInMemoryDatabase("Ntbs_Audit_Test_Db");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                var sp = services.BuildServiceProvider();

                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<NtbsContext>();

                    db.Database.EnsureCreated();
                    Utilities.SeedDatabase(db);
                }
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddScoped<ICultureAndResistanceService>(
                    sp => new MockCultureAndResistanceService(Utilities.NOTIFIED_ID));
                services.AddScoped<ISpecimenService>(
                    sp => new MockSpecimenService(Utilities.NOTIFIED_ID,
                        Utilities.TBSERVICE_ABINGDON_COMMUNITY_HOSPITAL_ID,
                        Utilities.PERMITTED_PHEC_CODE)); 
                
                services.AddScoped<IHomepageKpiService>(sp => new MockHomepageKpiService());
            });
        }

        public void ConfigureLogger(string testName)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Debug();
            // // Uncomment to have SUT app log into files
            // var logFilePath = $"../../../{DateTime.Now:yyyy-M-d}-{testName}-logs.txt";
            // logger = logger.WriteTo.File(logFilePath); 
            Log.Logger = logger.CreateLogger();
        }
    }
}
