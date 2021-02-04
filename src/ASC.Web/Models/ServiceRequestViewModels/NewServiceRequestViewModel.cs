using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ASC.Utilities.ValidationAttributes;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Models.ServiceRequestViewModels
{
    public class NewServiceRequestViewModel
    {
        [Required]
        [Display(Name = "Vehicle Name")]
        public string VehicleName { get; set; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Required]
        [Display(Name = "Requested Services")]
        public string RequestedServices { get; set; }

        [Required]
        [FutureDate(90, ErrorMessage = "Data inválida")]
        // [Remote(action: "CheckDenialService", controller: "ServiceRequest")]
        [Display(Name = "Requested Date")]
        public DateTime? RequestedDate { get; set; }
    }
}


