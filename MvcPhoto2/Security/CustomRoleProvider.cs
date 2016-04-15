using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Byu.Utilities.LDAP;
using MvcPhoto2.Models;
using System.Configuration.Provider;

namespace MvcPhoto2.Security
{
    
    public class CustomRoleProvider : RoleProvider
    {
        public string username { get; set; }
        private Photo2Entities1 db = new Photo2Entities1();
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                return "BYU Photo";
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        //dont need this
        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }
        // end


        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (roleName == "Admin")
                return db.Authentications.Where(a => a.NetId == usernameToMatch && a.role == "Admin").Select(a => a.NetId).ToArray();
            else
                return db.Authentications.Where(a => a.NetId == usernameToMatch && (a.role == "Photographer" || a.role == "Admin")).Select(a => a.NetId).ToArray();
            
        }

        public override string[] GetAllRoles()
        {
            string[] roles = { "Admin", "Photographer"};
            return roles;
        }

        public override string[] GetRolesForUser(string username)
        {
            
            string[] roles = { "Admin", "Photographer" };
            if (username == null || username == "")
                throw new ProviderException("User name cannot be empty or null.");
            var t = db.Authentications;
            var usr = db.Authentications.FirstOrDefault(a => a.NetId == username);
            string roleName = usr == null ? null : usr.role;

            if (roleName == "Admin")
                return roles;
            else
                return new string[] { "Photographer" };
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (roleName == "Admin")
                return db.Authentications.Where(a => (a.role == roleName || a.role == "Admin")).Select(a => a.NetId).ToArray();
            else
                return db.Authentications.Where(a => (a.role == roleName || a.role == "Admin" || a.role == "Photographer")).Select(a => a.NetId).ToArray();
            
        }
        
        public override bool IsUserInRole(string username, string roleName)
        {
            
            if (roleName == "Admin")
                return db.Authentications.Where(a => a.NetId == username && a.role == roleName).Count() > 0;
            
            else
                return db.Authentications.Where(a => (a.NetId == username && (a.role == roleName || a.role == "Admin" || a.role == "Photographer"))).Count() > 0;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            string[] roles = { "Admin", "Photographer" };
            return roles.Contains(roleName);

        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (db != null)
                {
                    db.Dispose();
                    db = null;
                }
            }
        }
    }
}