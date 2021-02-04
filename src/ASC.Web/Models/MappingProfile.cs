using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Models.Models;
using ASC.Web.Models.MasterDataViewModels;
using ASC.Web.Models.ServiceRequestViewModels;
using AutoMapper;

namespace ASC.Web.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MasterDataKey, MasterDataKeyViewModel>();
            CreateMap<MasterDataKeyViewModel, MasterDataKey>();
            CreateMap<MasterDataValue, MasterDataValueViewModel>();
            CreateMap<MasterDataValueViewModel, MasterDataValue>();

            // Serivces Requests 
            CreateMap<ServiceRequest, NewServiceRequestViewModel>();
            CreateMap<NewServiceRequestViewModel, ServiceRequest>();
            CreateMap<ServiceRequest, UpdateServiceRequestViewModel>();
            CreateMap<UpdateServiceRequestViewModel, ServiceRequest>();
        }
    }
}
