using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASC.Web.Models;
using ASC.Web.Configuration;
using Microsoft.Extensions.Options;
using ASC.Utilities;
using ASC.Web.Controllers.Base;
using ASC.Business.Interfaces;

namespace ASC.Web.Controllers
{
    public class HomeController : AnonymousController
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationSettings _settings;
        private readonly IMasterDataOperations _masterDataOperations;

        public HomeController(ILogger<HomeController> logger, IOptions<ApplicationSettings> settings, IMasterDataOperations masterDataOperations)
        {
            _logger = logger;
            _settings = settings.Value;
            _masterDataOperations = masterDataOperations ?? throw new ArgumentNullException();
        }


        [HttpGet]
        public IActionResult Index()
        {

            var teste = _masterDataOperations.GetAllMasterKeysAsync().Result;
            // set session
            HttpContext.Session.SetSession("Test", _settings);
            // Get Session
            var settings = HttpContext.Session.GetSession<ApplicationSettings>("Test");

            ViewData["Title"] = _settings.ApplicationTitle;
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }

        //[HttpGet]
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}

        [HttpGet]
        public IActionResult Error(string id)
        {
            if (id == "404")
                return View("NotFound");
            if (id == "401" && User.Identity.IsAuthenticated)
                return View("AccessDenied");
            else
                return RedirectToAction("Login", "Account");
            
        }
    }
}
