using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASC.Web.Models.AccountViewModels
{
    /// <summary>
    /// which will support adding or updating an existing Service Engineer
    /// </summary>
    public class ServiceEngineerRegistrationViewModel : RegisterViewModel
    {
        public string UserName { get; set; }
        public bool IsEdit { get; set; }
        public bool IsActive { get; set; }
    }
}
