using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace ASC.Web.Models.ServiceRequestViewModels
{
    /// <summary>
    /// This view model is primarily used to update the service request information and it holds
    /// properties like RowKey, PartitionKey etc.
    /// </summary>
    public class UpdateServiceRequestViewModel : NewServiceRequestViewModel 
    {
        public string RowKey { get; set; }
        public string PartitionKey { get; set; }
        
        [Required]
        [Display(Name = "Service Engineer")]
        public string ServiceEngineer { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; }

    }
}
