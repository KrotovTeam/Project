using System.Web.Mvc;
using BusinessLogic.Abstraction;
using Common.Constants;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IConvertManager _convertManager { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            var path = Server.MapPath("../Files/test.tif");
            var kek = _convertManager.ConvertSnapshot(path, ChannelEnum.Channel1);
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}