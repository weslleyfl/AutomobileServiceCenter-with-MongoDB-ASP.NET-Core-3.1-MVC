using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.AccountViewModels
{
    /// <summary>
    /// which will wrap a list of Service Engineers along with ServiceEngineerRegistrationViewModel
    /// </summary>
    public class ServiceEngineerViewModel
    {
        public List<ApplicationUser> ServiceEngineers { get; set; }
        public ServiceEngineerRegistrationViewModel Registration { get; set; }
    }
}
