
namespace WebAPI.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Entities;
    using Microsoft.AspNetCore.Identity;
    using Helper;

    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();

            var user = await _userHelper.GetUserByEmailAsync("emaldonado@itworks.ec");
            if (user == null)
            {
                user = new User
                {
                    DisplayName = "Daverick",
                    Email = "emaldonado@itworks.ec",
                    UserName = "emaldonado@itworks.ec"
                };

                var result = await _userHelper.AddUserAsync(user, "Pwd1234");
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException("Could not create the user in seeder");
                }
            }

            if (!_context.Applications.Any())
            {
                AddApplication("Cambium 2021.07", user);
                AddApplication("Cambium 2021.01", user);
                AddApplication("Cambium 2021.04", user);
                await _context.SaveChangesAsync();
            }
        }

        //private async Task<User> CheckUser(string userName, string displayName, string role)
        //{
        //    // Add user
        //    var user = await _userHelper.GetUserByEmailAsync(userName);
        //    if (user == null)
        //    {
        //        user = await AddUser(userName, displayName, role);
        //    }

        //    var isInRole = await _userHelper.IsUserInRoleAsync(user, role);
        //    if (!isInRole)
        //    {
        //        await _userHelper.AddUserToRoleAsync(user, role);
        //    }

        //    return user;
        //}

        //private async Task<User> AddUser(string userName, string displayName, string role)
        //{
        //    var user = new User
        //    {
        //        DisplayName = displayName,
        //        Email = userName,
        //        PhoneNumber = "350 634 2747"
        //    };

        //    var result = await _userHelper.AddUserAsync(user, "123456");
        //    if (result != IdentityResult.Success)
        //    {
        //        throw new InvalidOperationException("Could not create the user in seeder");
        //    }

        //    await this._userHelper.AddUserToRoleAsync(user, role);
        //    var token = await this._userHelper.GenerateEmailConfirmationTokenAsync(user);
        //    await this._userHelper.ConfirmEmailAsync(user, token);
        //    return user;
        //}

        //private async Task CheckRolesAsync()
        //{
        //    await _userHelper.CheckRoleAsync("Admin");
        //    await _userHelper.CheckRoleAsync("Customer");
        //}

        private void AddApplication(string name, User user)
        {
            _context.Applications.Add(new Application
            {
                Name = name,
                AppId = Guid.NewGuid(),
                User = user
            });
        }
    }
}
