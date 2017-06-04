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
        public ActionResult Snapshots(GeographicalPoint upperRight)
        {
            return View();
        }
    }
}