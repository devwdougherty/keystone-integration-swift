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
        KeyStone keyStoneService = new KeyStone();

        SwiftObjectStorage swiftObjectStorage = new SwiftObjectStorage();

        public IActionResult Index()
        {
            SwiftConfig swiftConfig = keyStoneService.Authenticate();

            swiftObjectStorage.CreateObject(swiftConfig, "objectName", "C:/Temp");

            return View();
        }
    }
}