using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASC.Business;
using ASC.Business.Interfaces;
using ASC.Utilities;

namespace ASC.Web.Filters
{
    public class UserActivityFilter : ActionFilterAttribute
    {
        private readonly ILogDataOperations _logDataOperations;


        public UserActivityFilter(ILogDataOperations logDataOperations)
        {
            _logDataOperations = logDataOperations;
        }

        /// <summary>
        /// After the action execution, we will get an instance of ILogDataOperations 
        /// from RequestServices of HttpContext and call the CreateUserActivityAsync method, 
        /// which will create the user activity log at Azure Table storage     
        /// </summary>      
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            // Criar instancia manualmente 
            // var logger = context.HttpContext.RequestServices.GetService(typeof(ILogDataOperations)) as ILogDataOperations;

            await _logDataOperations
                .CreateUserActivityAsync(
                    context.HttpContext.User.GetCurrentUserDetails().Email,
                    context.HttpContext.Request.Path);

            //base.OnActionExecutionAsync(context, next);
        }
    }
}
