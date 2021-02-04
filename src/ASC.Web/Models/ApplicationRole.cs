﻿using AspNetCore.Identity.MongoDbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models
{
    public class ApplicationRole : MongoIdentityRole<Guid>
    {
		public ApplicationRole() : base()
		{
		}

		public ApplicationRole(string roleName) : base(roleName)
		{
		}
	}
}
