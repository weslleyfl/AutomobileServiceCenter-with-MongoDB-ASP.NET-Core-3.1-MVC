using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Models.Models;


namespace ASC.Web.Models.ServiceRequestViewModels
{
    public class ServiceRequestDetailViewModel
    {
        public UpdateServiceRequestViewModel ServiceRequest { get; set; }
        public List<ServiceRequest> ServiceRequestAudit { get; set; }
    }
}
