using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KeyStoneSwiftIntegration.Models;
using KeyStoneSwiftIntegration.Services;
using Microsoft.AspNetCore.Mvc;

namespace KeyStoneSwiftIntegration.Controllers
{
    public class ObjectStorageController : Controller
    {
        KeyStoneService keyStoneService = new KeyStoneService();

        SwiftObjectStorage swiftObjectStorage = new SwiftObjectStorage();

        public IActionResult Index()
        {
            SwiftConfig swiftConfig = keyStoneService.Authenticate();

            

            return View();
        }
    }
}