using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ntbs_service.Models.Entities;
using ntbs_service.Models.ReferenceEntities;

namespace ntbs_service.DataAccess
{
    public interface IUserRepository
    {
        Task AddOrUpdateUser(User user, IEnumerable<TBService> tbServices);
        Task AddUserLoginEvent(UserLoginEvent userLoginEvent);
        Task<User> GetUserByUsername(string username);
        Task UpdateUserContactDetails(User user);
        Task<Dictionary<string, string>> GetUsernameDictionary();
    }

    public class UserRepository : IUserRepository
    {
        private readonly NtbsContext _context;

        public UserRepository(NtbsContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateUser(User user, IEnumerable<TBService> tbServices)
        {
            var existingUser = await _context.User.Include(u => u.CaseManagerTbServices)
                .SingleOrDefaultAsync(u => u.Username == user.Username);

            if (existingUser != null)
            {
                await UpdateUserAdDetails(existingUser, user, tbServices);
            }
            else
            {
                await AddUser(user, tbServices);
            }
        }

        public async Task AddUserLoginEvent(UserLoginEvent userLoginEvent)
        {
            await _context.UserLoginEvent.AddAsync(userLoginEvent);
            await _context.SaveChangesAsync();
        }

        public async Task<User> GetUserByUsername(string username)
        {
            var user = await _context.User
                .Include(u => u.CaseManagerTbServices)
                .ThenInclude(c => c.TbService)
                .ThenInclude(tb => tb.PHEC)
                .FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
            return user;
        }

        /// <summary>
        /// Our user models consist of values that come from the AD (automatically synced) and others that are supplied
        /// by the user. This method is used for updating the latter, in a way that does not wipe data from the
        /// user-sync fields.
        ///
        /// See also UpdateUserAdDetails method in this class
        /// </summary>
        public async Task UpdateUserContactDetails(User user)
        {
            _context.Attach(user);
            _context.Entry(user).Property(x => x.JobTitle).IsModified = true;
            _context.Entry(user).Property(x => x.PhoneNumberPrimary).IsModified = true;
            _context.Entry(user).Property(x => x.PhoneNumberSecondary).IsModified = true;
            _context.Entry(user).Property(x => x.EmailPrimary).IsModified = true;
            _context.Entry(user).Property(x => x.EmailSecondary).IsModified = true;
            _context.Entry(user).Property(x => x.Notes).IsModified = true;
            await _context.SaveChangesAsync();
        }

        public Task<Dictionary<string, string>> GetUsernameDictionary()
        {
            return _context.User.ToDictionaryAsync(
                user => user.Username,
                user => user.FullName
            );
        }

        private async Task AddUser(User user, IEnumerable<TBService> tbServices)
        {
            await _context.User.AddAsync(user);
            SyncCaseManagerTbServices(user, tbServices);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Our user models consist of values that come from the AD (automatically synced) and others that are supplied
        /// by the user. This method is used for updating the former, in a way that does not wipe data from the
        /// user-supplied fields.
        ///
        /// See also UpdateUserContactDetails method in this class
        /// </summary>
        private async Task UpdateUserAdDetails(User existingUser, User newUser, IEnumerable<TBService> tbServices)
        {
            existingUser.GivenName = newUser.GivenName;
            existingUser.FamilyName = newUser.FamilyName;
            existingUser.DisplayName = newUser.DisplayName;
            existingUser.AdGroups = newUser.AdGroups;
            existingUser.IsActive = newUser.IsActive;
            existingUser.IsCaseManager = newUser.IsCaseManager;
            SyncCaseManagerTbServices(existingUser, tbServices);
            await _context.SaveChangesAsync();
        }

        private static void SyncCaseManagerTbServices(User user, IEnumerable<TBService> tbServices)
        {
            var caseManagerTbServices = tbServices
                .Select(tb => new CaseManagerTbService
                {
                    TbServiceCode = tb.Code,
                    CaseManagerUsername = user.Username
                })
                .ToList();

            if (user.CaseManagerTbServices == null)
            {
                user.CaseManagerTbServices = caseManagerTbServices;
            }
            else
            {
                RemoveUnmatchedCaseManagerTbServices(user.CaseManagerTbServices, caseManagerTbServices);

                foreach (var caseManagerTbService in caseManagerTbServices)
                {
                    if (!user.CaseManagerTbServices.Any(c => c.Equals(caseManagerTbService)))
                    {
                        user.CaseManagerTbServices.Add(caseManagerTbService);
                    }
                }
            }
        }

        private static void RemoveUnmatchedCaseManagerTbServices(
            ICollection<CaseManagerTbService> userCaseManagerTbServices,
            IList<CaseManagerTbService> adCaseManagerTbServices)
        {
            var caseManagerTbServicesToRemove = new List<CaseManagerTbService>();
            foreach (var caseManagerTbService in userCaseManagerTbServices)
            {
                if (!adCaseManagerTbServices.Any(c => caseManagerTbService.Equals(c)))
                {
                    caseManagerTbServicesToRemove.Add(caseManagerTbService);
                }
            }
            foreach (var caseManagerTbService in caseManagerTbServicesToRemove)
            {
                userCaseManagerTbServices.Remove(caseManagerTbService);
            }
        }
    }
}
