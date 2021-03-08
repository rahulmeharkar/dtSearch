using BAL.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAPI.Controllers;
using BAL;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateIndex(IndexModel mod,HttpPostedFileBase path)
        {
            string t = Convert.ToString(path);
            int ls = t.LastIndexOf('.');
            int l = t.Length;
            t = t.Remove(ls, l - ls);
            t = t.Replace("\"", ".");
            int lss = t.LastIndexOf('.');
            t = t.Remove(lss, l - lss);
            t = t.Replace(".", "\"");
            mod.path = t;
            DtCrud d = new DtCrud();           
            var result = d.CreateIndex(mod.indexname, mod.path);
            return View();
        }
    }
}