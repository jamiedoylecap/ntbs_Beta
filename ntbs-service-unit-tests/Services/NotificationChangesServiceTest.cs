﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using EFAuditer;
using Moq;
using ntbs_service.DataAccess;
using ntbs_service.Services;
using ntbs_service.TagHelpers;
using Xunit;
using static ntbs_service.Helpers.CsvParser;

namespace ntbs_service_unit_tests.Services
{

    // This test was created by using the application to generate the appropriate audit records in the audit DB,
    // then copying the values out of the table.
    // Similarly, the expected strings were taken from the finished page after checking they were what we wanted
    // from the audit page.
    public class NotificationChangesServiceTest
    {
        readonly NotificationChangesService _changesService;
        private readonly Mock<IAuditService> _auditServiceMock = new Mock<IAuditService>();
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();

        public NotificationChangesServiceTest()
        {
            _changesService = new NotificationChangesService(_auditServiceMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task FullHistoryOfLogs_GetsTranslatedToChangesCorrectly()
        {
            // Arrange
            var auditLogs = GetAuditLogs("auditLogsForNotification1");
            _auditServiceMock.Setup(service => service.GetWriteAuditsForNotification(1))
                .ReturnsAsync(auditLogs);
            _userRepositoryMock.Setup(repo => repo.GetUsernameDictionary())
                .ReturnsAsync(new Dictionary<string, string> {{"Developer@ntbs.phe.com", "John Johnson"}});

            // Act
            var changes = (await _changesService.GetChangesList(1)).ToList();

            // Assert
            Assert.Collection(PrintInOrder(changes),
                c => Assert.Equal("25 Jun 2020, 15:33 John Johnson denotified Notification", c),
                c => Assert.Equal("25 Jun 2020, 15:18 John Johnson rejected Transfer", c),
                c => Assert.Equal("25 Jun 2020, 15:18 John Johnson requested Transfer", c),
                c => Assert.Equal("25 Jun 2020, 15:00 John Johnson accepted Transfer", c),
                c => Assert.Equal("25 Jun 2020, 14:59 John Johnson requested Transfer", c),
                c => Assert.Equal("25 Jun 2020, 11:34 John Johnson unmatched Specimen", c),
                c => Assert.Equal("25 Jun 2020, 11:34 John Johnson matched Specimen", c),
                c => Assert.Equal("25 Jun 2020, 11:22 NTBS updated Cluster membership", c),
                c => Assert.Equal("25 Jun 2020, 11:19 NTBS updated Cluster membership", c),
                c => Assert.Equal("24 Jun 2020, 18:13 John Johnson added M. bovis - unpasteurised milk consumption", c),
                c => Assert.Equal("24 Jun 2020, 18:13 John Johnson updated M. bovis details", c),
                c => Assert.Equal("24 Jun 2020, 18:13 John Johnson added M. bovis - exposure to another case", c),
                c => Assert.Equal("24 Jun 2020, 18:12 John Johnson updated M. bovis details", c),
                c => Assert.Equal("24 Jun 2020, 18:00 John Johnson updated MDR Details", c),
                c => Assert.Equal("23 Jun 2020, 11:43 John Johnson updated Social Risk Factors", c),
                c => Assert.Equal("23 Jun 2020, 11:40 John Johnson updated Social Context Venue", c),
                c => Assert.Equal("23 Jun 2020, 11:23 John Johnson deleted Social Context Venue", c),
                c => Assert.Equal("23 Jun 2020, 10:44 John Johnson updated Previous History", c),
                c => Assert.Equal("23 Jun 2020, 10:44 John Johnson added Social Context Venue", c),
                c => Assert.Equal("23 Jun 2020, 10:44 John Johnson added Social Context Venue", c),
                c => Assert.Equal("23 Jun 2020, 10:42 John Johnson added Social Context Address", c),
                c => Assert.Equal("23 Jun 2020, 10:41 John Johnson updated Immunosuppression", c),
                c => Assert.Equal("23 Jun 2020, 10:41 John Johnson updated Co-morbidities", c),
                c => Assert.Equal("23 Jun 2020, 10:41 John Johnson updated Visitor details", c),
                c => Assert.Equal("23 Jun 2020, 10:41 John Johnson updated Travel details", c),
                c => Assert.Equal("23 Jun 2020, 10:41 John Johnson updated Social Risk Factors", c),
                c => Assert.Equal("23 Jun 2020, 10:41 John Johnson added Test result", c),
                c => Assert.Equal("23 Jun 2020, 10:40 John Johnson updated Test Results", c),
                c => Assert.Equal("23 Jun 2020, 10:40 John Johnson updated Contact Tracing", c),
                c => Assert.Equal("23 Jun 2020, 10:40 John Johnson added Treatment event", c),
                c => Assert.Equal("23 Jun 2020, 10:39 John Johnson updated Notification", c),
                c => Assert.Equal("23 Jun 2020, 10:39 John Johnson updated Clinical Details", c),
                c => Assert.Equal("23 Jun 2020, 10:39 John Johnson updated Hospital details", c),
                c => Assert.Equal("23 Jun 2020, 10:39 John Johnson updated Personal details", c),
                c => Assert.Equal("23 Jun 2020, 09:44 John Johnson submitted Notification", c),
                c => Assert.Equal("23 Jun 2020, 09:41 John Johnson created Draft", c)
            );
        }

        [Fact]
        public async Task ImportLogs_GetTranslatedToSingle()
        {
            // Arrange
            var auditLogs = GetAuditLogs("auditLogsForNotification2");
            _auditServiceMock.Setup(service => service.GetWriteAuditsForNotification(2))
                .ReturnsAsync(auditLogs);
            _userRepositoryMock.Setup(repo => repo.GetUsernameDictionary())
                .ReturnsAsync(new Dictionary<string, string> {{"Developer@ntbs.phe.com", "John Johnson"}});

            // Act
            var changes = (await _changesService.GetChangesList(2)).ToList();

            // Assert
            Assert.Collection(PrintInOrder(changes),
                c => Assert.Equal("30 Jun 2020, 16:47 John Johnson updated Previous History", c),
                c => Assert.Equal("25 Jun 2020, 09:15 John Johnson imported Notification", c)
            );
        }

        [Fact]
        public async Task ClosedNotificationLogs_GetTranslatedCorrectly()
        {
            // Arrange
            var auditLogs = GetAuditLogs("auditLogsForNotification3");
            _auditServiceMock.Setup(service => service.GetWriteAuditsForNotification(3))
                .ReturnsAsync(auditLogs);
            _userRepositoryMock.Setup(repo => repo.GetUsernameDictionary())
                .ReturnsAsync(new Dictionary<string, string> {{"Developer@ntbs.phe.com", "John Johnson"}});

            // Act
            var changes = (await _changesService.GetChangesList(3)).ToList();

            // Assert
            Assert.Collection(PrintInOrder(changes),
            c => Assert.Equal("06 Mar 2020, 08:36 NTBS closed Notification", c),
            c => Assert.Equal("05 Mar 2020, 19:15 John Johnson added Treatment event", c),
            c => Assert.Equal("05 Mar 2020, 11:08 John Johnson updated Clinical Details", c),
            c => Assert.Equal("05 Mar 2020, 11:07 John Johnson updated Clinical Details", c),
            c => Assert.Equal("05 Mar 2020, 11:03 John Johnson updated Personal details", c),
            c => Assert.Equal("05 Mar 2020, 11:02 John Johnson updated Treatment event", c),
            c => Assert.Equal("05 Mar 2020, 11:01 John Johnson added Treatment event", c),
            c => Assert.Equal("05 Mar 2020, 11:01 John Johnson updated Treatment event", c),
            c => Assert.Equal("05 Mar 2020, 11:00 John Johnson updated Personal details", c),
            c => Assert.Equal("05 Mar 2020, 11:00 John Johnson added Treatment event", c),
            c => Assert.Equal("05 Mar 2020, 10:59 John Johnson added Social Context Address", c),
            c => Assert.Equal("05 Mar 2020, 10:57 John Johnson updated Notification", c),
            c => Assert.Equal("05 Mar 2020, 10:55 John Johnson updated Clinical Details", c),
            c => Assert.Equal("03 Mar 2020, 16:35 John Johnson submitted Notification", c),
            c => Assert.Equal("13 Feb 2020, 15:35 John Johnson created Draft", c)
            );
        }

        [Fact]
        // This came from a "real" notification on test that didn't pass QA, so works well as a regression test
        public async Task Notification1463OnTest_GetTranslatedCorrectly()
        {
            // Arrange
            var auditLogs = GetAuditLogs("auditLogsForNotification4");
            _auditServiceMock.Setup(service => service.GetWriteAuditsForNotification(4))
                .ReturnsAsync(auditLogs);
            _userRepositoryMock.Setup(repo => repo.GetUsernameDictionary())
                .ReturnsAsync(new Dictionary<string, string> {{"Developer@ntbs.phe.com", "John Johnson"}});

            // Act
            var changes = (await _changesService.GetChangesList(4)).ToList();

            // Assert
            Assert.Collection(PrintInOrder(changes),
            c => Assert.Equal("02 Jul 2020, 08:58 John Johnson updated Travel details", c),
            c => Assert.Equal("01 Jul 2020, 19:37 John Johnson updated Travel details", c),
            c => Assert.Equal("01 Jul 2020, 19:37 John Johnson updated Travel details", c),
            c => Assert.Equal("01 Jul 2020, 19:37 John Johnson updated Travel details", c),
            c => Assert.Equal("01 Jul 2020, 19:36 John Johnson updated Travel details", c),
            c => Assert.Equal("01 Jul 2020, 19:36 John Johnson updated Travel details", c),
            c => Assert.Equal("01 Jul 2020, 19:36 John Johnson updated Travel details", c),
            c => Assert.Equal("01 Jul 2020, 11:49 John Johnson matched Specimen", c),
            c => Assert.Equal("01 Jul 2020, 11:45 John Johnson submitted Notification", c),
            c => Assert.Equal("01 Jul 2020, 11:40 John Johnson created Draft", c)
            );
        }

        private static List<AuditLog> GetAuditLogs(string filename)
        {
            return GetRecordsFromCsv($"../../../TestData/{filename}.csv", ParseAuditLog)
                .OrderBy(log => log.AuditDateTime)
                .ToList();
        }

        private static AuditLog ParseAuditLog(CsvReader csvReader) =>
            new AuditLog
            {
                Id = csvReader.GetField<int>(nameof(AuditLog.Id)),
                OriginalId = csvReader.GetField(nameof(AuditLog.OriginalId)),
                EntityType = csvReader.GetField(nameof(AuditLog.EntityType)),
                EventType = csvReader.GetField(nameof(AuditLog.EventType)),
                AuditDetails = csvReader.GetField(nameof(AuditLog.AuditDetails)),
                AuditData = csvReader.GetField(nameof(AuditLog.AuditData)),
                AuditDateTime = csvReader.GetField<DateTime>(nameof(AuditLog.AuditDateTime)),
                AuditUser = csvReader.GetField(nameof(AuditLog.AuditUser)),
                RootEntity = csvReader.GetField(nameof(AuditLog.RootEntity)),
                RootId = csvReader.GetField(nameof(AuditLog.RootId)),
            };

        private static IEnumerable<string> PrintInOrder(IEnumerable<NotificationHistoryListItemModel> changes)
        {
            return changes
                .OrderByDescending(c => c.Date)
                .Select(c => $"{c.Date:dd MMM yyyy, HH:mm} {c.Username} {c.Action} {c.Subject}");
        }
    }
}
