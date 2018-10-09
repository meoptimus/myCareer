using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using myCareer.Models;

namespace myCareer.Security
{
    public class PrincipalController : IPrincipal
    {
        private user user;
        private AccountModel am = new AccountModel();
        public PrincipalController(user user)
        {
            this.user = am.find(user.user_name);
            this.Identity = new GenericIdentity(user.user_name);

        }

        public IIdentity Identity
        {
            get;
            set;
        }

        // GET: Principal
 

        public bool IsInRole(string role)
        {
            return this.user.role.Equals(role);
        }
    }
}