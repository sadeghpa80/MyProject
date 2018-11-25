using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using DataLayer;

namespace ArianWebsiteMVC.Controllers
{
    public class AllPagesController : Controller
    {
        private static string GetIp() //get IP address
        {
            var ipHost = Dns.Resolve(Dns.GetHostName());
            var ipAddr = ipHost.AddressList[0];
            return ipAddr.ToString();
        }

        // GET: AllPages
        [Route("AboutUs")]
        public ActionResult AboutUs()
        {
            return View();
        }

        [Route("BarMill1")]
        public ActionResult BarMill1()
        {
            return View();
        }

        [Route("BarMill2")]
        public ActionResult BarMill2()
        {
            return View();
        }

        [Route("MeltShop1")]
        public ActionResult MeltShop1()
        {
            return View();
        }

        [Route("MeltShop2")]
        public ActionResult MeltShop2()
        {
            return View();
        }

        [Route("RollingMill1")]
        public ActionResult RollingMill1()
        {
            return View();
        }

        [Route("RollingMill2")]
        public ActionResult RollingMill2()
        {
            return View();
        }

        [Route("OxygenPlant1")]
        public ActionResult OxygenPlant1()
        {
            return View();
        }

        [Route("OxygenPlant2")]
        public ActionResult OxygenPlant2()
        {
            return View();
        }

        [Route("ProductsList")]
        public ActionResult ProductsList()
        {
            return View();
        }
        [Route("Certificates")]
        public ActionResult Certificates()
        {
            return View();
        }
        [Route("AllProductLine")]
        public ActionResult AllProductLine()
        {
            return View();
        }

    }
}