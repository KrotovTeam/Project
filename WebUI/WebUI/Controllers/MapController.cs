﻿using System.Collections.Generic;
using System.Web.Mvc;
using BusinessLogic.Dtos;

namespace WebUI.Controllers
{
    public class MapController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Snapshots(IEnumerable<GeographicalPoint> points)
        {
            return Json(points);
        }
    }
}