using AspNetCore.Identity.MongoDbCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models
{
	// Name this collection Users
	// [MongoDbGenericRepository.Attributes.CollectionName("Users")]
	public class ApplicationUser : MongoIdentityUser<Guid>
    {
		public ApplicationUser() : base()
		{
		}

		public ApplicationUser(string userName, string email) : base(userName, email)
		{
		}
	}
}
