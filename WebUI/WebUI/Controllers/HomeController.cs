using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Abstraction;
using Common.Constants;

namespace WebUI.Controllers
{
    public class HomeController : Controller
    {
        public IConvertManager _convertManager { get; set; }
        public IClassificationManager _classificationManager { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var path = Server.MapPath("../Files/test.tif");
            var kek = await _convertManager.ConvertSnapshotAsync(path, ChannelEnum.Channel1);
            var lol = _classificationManager.Clustering(kek);
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