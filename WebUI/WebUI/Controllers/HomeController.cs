using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
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
            var path = Server.MapPath("~/Files/test.tif");
            var kek = await _convertManager.ConvertSnapshotAsync(path, ChannelEnum.Channel1);
            var lol = _classificationManager.Clustering(kek);
            //using (var bitmap = new Bitmap(605, 601))
            //{
            //    foreach (var point in lol.ElementAt(0).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.Crimson);
            //    }
            //    foreach (var point in lol.ElementAt(1).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.DarkBlue);
            //    }
            //    foreach (var point in lol.ElementAt(2).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.DarkGreen);
            //    }
            //    bitmap.Save(Server.MapPath("~/Files/result.tif"), ImageFormat.Bmp);
            //}
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