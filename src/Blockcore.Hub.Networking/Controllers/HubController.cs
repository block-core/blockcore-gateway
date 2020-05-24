using System;
using System.Collections.Generic;
using System.Text;
using Blockcore.Hub.Networking.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blockcore.Hub.Networking.Controllers
{
   [ApiController]
   [Route("api/data")]
   public class HubController : Controller
   {
      private readonly AvailableServices availableServices;

      public HubController(AvailableServices availableServices)
      {
         this.availableServices = availableServices;
      }

      [HttpGet]
      public IActionResult AvailableServices()
      {
         return Ok(availableServices);
      }
   }
}
