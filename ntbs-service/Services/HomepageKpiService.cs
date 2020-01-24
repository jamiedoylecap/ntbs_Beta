using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Audit.Core;
using Dapper;
using Microsoft.Extensions.Configuration;
using ntbs_service.Helpers;
using ntbs_service.Models.Entities;
using ntbs_service.Models.ReferenceEntities;

namespace ntbs_service.Services
{
    public interface IHomepageKpiService
    {
        Task<IEnumerable<HomepageKpi>> GetKpiForPhec(IEnumerable<string> phecCodes);
        Task<IEnumerable<HomepageKpi>> GetKpiForTbService(IEnumerable<string> tbServiceCodes);
    }

    public class HomepageKpiService : IHomepageKpiService
    {
        private readonly string _connectionString;

        public HomepageKpiService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("reporting");
        }

        public async Task<IEnumerable<HomepageKpi>> GetKpiForPhec(IEnumerable<string> phecCodes)
        {
            var query = HomepageKpiQueryHelper.GetKpiForPhecQuery;
            var formattedPhecCodes = HomepageKpiQueryHelper.FormatEnumerableParams(phecCodes);

            var homepageKpiResults = await ExecuteGetKpiQuery(query, formattedPhecCodes);
            return homepageKpiResults;
        }
        
        public async Task<IEnumerable<HomepageKpi>> GetKpiForTbService(IEnumerable<string> tbServiceCodes)
        {
            var query = HomepageKpiQueryHelper.GetKpiForServiceQuery;
            var formattedServiceCodes = HomepageKpiQueryHelper.FormatEnumerableParams(tbServiceCodes);

            var homepageKpiResults = await ExecuteGetKpiQuery(query, formattedServiceCodes);
            return homepageKpiResults;
        }
        
        private async Task<IEnumerable<HomepageKpi>> ExecuteGetKpiQuery(string query, string param = null)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<HomepageKpi>(query, new {param});
            }
        }
    }
}
