﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ASC.Web.Controllers.Base
{
    [Authorize]
    //[UserActivityFilter]
    [ServiceFilter(typeof(UserActivityFilter))]
    public class BaseController : Controller
    {
    
    }
}