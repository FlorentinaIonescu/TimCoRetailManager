﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TRMApi.Data;
using TRMApi.Models;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _config;

        public UserController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IConfiguration config)
        {
            _context = context;
            _userManager = userManager;
            _config = config;
        }
        
        [HttpGet]
        [Route("User")]
        public UserModel GetById()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); /*RequestContext.Principal.Identity.GetUserId();*/
            UserData data = new UserData(_config);
            try
            {
                return data.GetUserById(userId).First();
            }
            catch (Exception)
            {

                throw;
            }
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllUsers")]
        public List<ApplicationUserModel> GetAllUsers()
        {
            List<ApplicationUserModel> output = new List<ApplicationUserModel>();

            
            var users = _context.Users.ToList();
            // linq query
            var userRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            select new { ur.UserId, ur.RoleId, r.Name };

            foreach (var user in users)
            {
                ApplicationUserModel u = new ApplicationUserModel
                {
                    Id = user.Id,
                    Email = user.Email
                };

                u.Roles = userRoles.Where(x => x.UserId == u.Id)
                     .ToDictionary(key => key.RoleId, val => val.Name);

                

                output.Add(u);
            }

            return output;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("Admin/GetAllRoles")]

        public Dictionary<string, string> GetAllRoles()
        {
            var roles = _context.Roles.ToDictionary(x => x.Id, x => x.Name);

            return roles;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/AddRole")]

        public async Task AddARole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.AddToRoleAsync(user, pairing.RoleName);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("Admin/RemoveRole")]

        public async Task RemoveARole(UserRolePairModel pairing)
        {
            var user = await _userManager.FindByIdAsync(pairing.UserId);
            await _userManager.RemoveFromRoleAsync(user, pairing.RoleName);

            //using (var context = new ApplicationDbContext())
            //{
            //    var userStore = new UserStore<ApplicationUser>(context);
            //    var userManager = new UserManager<ApplicationUser>(userStore);

            //    userManager.RemoveFromRole(pairing.UserId, pairing.RoleName);
            //}
        }
    }
}
