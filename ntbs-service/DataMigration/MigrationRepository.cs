using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Configuration;
using ntbs_service.DataMigration.RawModels;

namespace ntbs_service.DataMigration
{
    public interface IMigrationRepository
    {
        /// <returns>Groups of notifications, indexed by group id, or notification id for singletons</returns>
        Task<IEnumerable<IGrouping<string, string>>> GetGroupedNotificationIdsById(IEnumerable<string> legacyIds);

        /// <returns>Groups of notifications, indexed by group id, or notification id for singletons</returns>
        Task<IEnumerable<IGrouping<string, string>>> GetGroupedNotificationIdsByDate(DateTime rangeStartDate,
            DateTime endStartDate);

        Task<IEnumerable<MigrationDbNotification>> GetNotificationsById(List<string> legacyIds);
        Task<IEnumerable<MigrationDbSite>> GetNotificationSites(IEnumerable<string> legacyIds);
        Task<IEnumerable<MigrationDbManualTest>> GetManualTestResults(List<string> legacyIds);
        Task<IEnumerable<MigrationDbSocialContextVenue>> GetSocialContextVenues(List<string> legacyIds);
        Task<IEnumerable<MigrationDbSocialContextAddress>> GetSocialContextAddresses(List<string> legacyIds);
        Task<IEnumerable<MigrationDbTransferEvent>> GetTransferEvents(List<string> legacyIds);
        Task<IEnumerable<MigrationDbOutcomeEvent>> GetOutcomeEvents(List<string> legacyIds);
        Task<IEnumerable<MigrationDbMBovisAnimal>> GetMigrationMBovisAnimalExposure(List<string> legacyIds);
        Task<IEnumerable<MigrationDbMBovisKnownCase>> GetMigrationMBovisExposureToKnownCase(List<string> legacyIds);
        Task<IEnumerable<MigrationDbMBovisOccupation>> GetMigrationMBovisOccupationExposures(List<string> legacyIds);
        Task<IEnumerable<MigrationDbMBovisMilkConsumption>> GetMigrationMBovisUnpasteurisedMilkConsumption(List<string> legacyIds);

        Task<IEnumerable<(string LegacyId, string ReferenceLaboratoryNumber)>> GetReferenceLaboratoryMatches(
            IEnumerable<string> legacyIds);
    }

    public class MigrationRepository : IMigrationRepository
    {
        private string NotificationIdsWithGroupIdsByIdQuery =>
            $@"SELECT DISTINCT notificationInGroup.OldNotificationId, notificationInGroup.GroupId
			FROM MigrationNotificationsView notificationInGroup
			JOIN MigrationNotificationsView targetNotification 
				ON targetNotification.GroupId = notificationInGroup.GroupId 
				OR targetNotification.OldNotificationId = notificationInGroup.OldNotificationId
			WHERE targetNotification.OldNotificationId IN @Ids OR targetNotification.GroupId IN @Ids
            AND NOT EXISTS ({_importHelper.SelectImportedNotificationWhereIdEquals("notificationInGroup.OldNotificationId")})";

        private string NotificationsIdsWithGroupIdsByDateQuery =>
            $@"SELECT DISTINCT notificationInGroup.OldNotificationId, notificationInGroup.GroupId
			FROM MigrationNotificationsView notificationInGroup
			JOIN MigrationNotificationsView notificationInRange
				ON notificationInRange.GroupId = notificationInGroup.GroupId 
				OR notificationInRange.OldNotificationId = notificationInGroup.OldNotificationId
            WHERE notificationInRange.NotificationDate >= @StartDate AND notificationInRange.NotificationDate < @EndDate
            AND NOT EXISTS ({_importHelper.SelectImportedNotificationWhereIdEquals("notificationInGroup.OldNotificationId")})";

        const string NotificationsByIdQuery = @"
            SELECT *
            FROM MigrationNotificationsView n
            WITH (NOLOCK)
            WHERE n.OldNotificationId IN @Ids";

        const string NotificationSitesQuery = @"
            SELECT *
            FROM NotificationSite
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string ManualTestResultsQuery = @"
            SELECT *
            FROM ManualTestResults
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string SocialContextVenuesQuery = @"
            SELECT *
            FROM MigrationSocialContextVenueView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string SocialContextAddressesQuery = @"
            SELECT *
            FROM MigrationSocialContextAddressView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string TransferEventsQuery = @"
            SELECT *
            FROM MigrationTransferEventsView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string OutcomeEventsQuery = @"
            SELECT *
            FROM MigrationTreatmentOutcomeEventsView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string MigrationMBovisAnimalExposureQuery = @"
            SELECT *
            FROM MigrationMBovisAnimalExposureView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string MigrationMBovisExposureToKnownCaseQuery = @"
            SELECT *
            FROM MigrationMBovisExposureToKnownCaseView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string MigrationMBovisOccupationExposuresQuery = @"
            SELECT *
            FROM MigrationMBovisOccupationExposuresView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";

        const string MigrationMBovisUnpasteurisedMilkConsumptionQuery = @"
            SELECT *
            FROM MigrationMBovisUnpasteurisedMilkConsumptionView
            WITH (NOLOCK)
            WHERE OldNotificationId IN @Ids
        ";


        const string ReferenceLaboratoryMatchesQuery = @"
            SELECT LegacyId, ReferenceLaboratoryNumber
            FROM EtsLaboratoryResultsView
            WITH (NOLOCK)
            WHERE LegacyId IN @Ids
        ";

        private readonly string connectionString;
        private readonly INotificationImportHelper _importHelper;

        public MigrationRepository(IConfiguration _configuration, INotificationImportHelper importHelper)
        {
            connectionString = _configuration.GetConnectionString("migration");
            _importHelper = importHelper;
        }

        public async Task<IEnumerable<IGrouping<string, string>>> GetGroupedNotificationIdsById(
            IEnumerable<string> legacyIds)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return (await connection.QueryAsync<(string notificationId, string groupId)>(
                        NotificationIdsWithGroupIdsByIdQuery,
                        new {Ids = legacyIds}))
                    .GroupBy(
                        // Prefix "ETS" guarantees that we don't accidentally combine an ets notification 123 with the
                        // ltbr notifications 123-1 and 123-2 into a single group
                        t => t.groupId ?? $"ETS-{t.notificationId}", 
                        t => t.notificationId);
            }
        }

        public async Task<IEnumerable<IGrouping<string, string>>> GetGroupedNotificationIdsByDate(
            DateTime rangeStartDate,
            DateTime endStartDate)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return (await connection.QueryAsync<(string notificationId, string groupId)>(
                        NotificationsIdsWithGroupIdsByDateQuery,
                        new {StartDate = rangeStartDate.ToString("s"), EndDate = endStartDate.ToString("s")}))
                    .GroupBy(t => t.groupId ?? t.notificationId, t => t.notificationId);
            }
        }

        public async Task<IEnumerable<MigrationDbNotification>> GetNotificationsById(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbNotification>(NotificationsByIdQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbSite>> GetNotificationSites(IEnumerable<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbSite>(NotificationSitesQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbManualTest>> GetManualTestResults(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbManualTest>(ManualTestResultsQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbSocialContextVenue>> GetSocialContextVenues(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbSocialContextVenue>(SocialContextVenuesQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbSocialContextAddress>> GetSocialContextAddresses(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbSocialContextAddress>(SocialContextAddressesQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbTransferEvent>> GetTransferEvents(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbTransferEvent>(TransferEventsQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbOutcomeEvent>> GetOutcomeEvents(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbOutcomeEvent>(OutcomeEventsQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbMBovisAnimal>> GetMigrationMBovisAnimalExposure(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbMBovisAnimal>(MigrationMBovisAnimalExposureQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbMBovisKnownCase>> GetMigrationMBovisExposureToKnownCase(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbMBovisKnownCase>(MigrationMBovisExposureToKnownCaseQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbMBovisOccupation>> GetMigrationMBovisOccupationExposures(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbMBovisOccupation>(MigrationMBovisOccupationExposuresQuery, legacyIds);
        }

        public async Task<IEnumerable<MigrationDbMBovisMilkConsumption>> GetMigrationMBovisUnpasteurisedMilkConsumption(List<string> legacyIds)
        {
            return await ExecuteByIdQuery<MigrationDbMBovisMilkConsumption>(MigrationMBovisUnpasteurisedMilkConsumptionQuery, legacyIds);
        }

        public async Task<IEnumerable<(string LegacyId, string ReferenceLaboratoryNumber)>>
            GetReferenceLaboratoryMatches(IEnumerable<string> legacyIds)
        {
            // The table we're referencing here has legacyIds stored as INTs (since they are all ETS ids)
            // Therefore we need to convert to and from strings
            var intIds = legacyIds
                .Select(id => int.TryParse(id, out var intId) ? intId : (int?)null)
                .Where(id => id.HasValue)
                .Select((id => id.Value));
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return (await connection.QueryAsync<(int LegacyId, string ReferenceLaboratoryNumber)>(
                        ReferenceLaboratoryMatchesQuery,
                        new {Ids = intIds}))
                    .Select(tuple =>
                    {
                        string legacyId = tuple.LegacyId.ToString();
                        return (LegacyId: legacyId, tuple.ReferenceLaboratoryNumber);
                    });
            }
        }

        private async Task<IEnumerable<T>> ExecuteByIdQuery<T>(string query, IEnumerable<string> legacyIds)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<T>(query, new {Ids = legacyIds});
            }
        }
    }
}
