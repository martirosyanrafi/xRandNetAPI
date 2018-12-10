using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ResearchController : Controller
    {
        [HttpPost]
        public string Start([System.Web.Http.FromBody] ResearchBody content)
        {

            return "Home Page";
        }
    }
}
