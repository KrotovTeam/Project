using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Abstraction;
using Common.Constants;
using Point = BusinessLogic.Dtos.Point;

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
            var pathToFile1 = Server.MapPath("~/Files/TestChannel4.tif");
            var pathToFile2 = Server.MapPath("~/Files/TestChannel5.tif");
            var channel4 = _convertManager.ConvertSnapshotAsync(pathToFile1, ChannelEnum.Channel4);
            var channel5 = _convertManager.ConvertSnapshotAsync(pathToFile2, ChannelEnum.Channel5);
            await Task.WhenAll(channel4, channel5);

            var channels = new List<ChannelEnum> {ChannelEnum.Channel4, ChannelEnum.Channel5};
            var rawData =_convertManager.ConvertListsPoints(new List<IEnumerable<Point>> {channel4.Result, channel5.Result}, channels);

            var clusters = _classificationManager.Clustering(rawData, channels);

            //using (var bitmap = new Bitmap(999, 999))
            //{
            //    foreach (var point in clusters.ElementAt(0).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.Crimson);
            //    }
            //    foreach (var point in clusters.ElementAt(1).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.DarkBlue);
            //    }
            //    foreach (var point in clusters.ElementAt(2).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.DarkGreen);
            //    }
            //    foreach (var point in clusters.ElementAt(3).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.Chocolate);
            //    }
            //    foreach (var point in clusters.ElementAt(4).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.Aqua);
            //    }
            //    foreach (var point in clusters.ElementAt(5).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.BlueViolet);
            //    }
            //    foreach (var point in clusters.ElementAt(6).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.DarkGoldenrod);
            //    }
            //    foreach (var point in clusters.ElementAt(7).Points)
            //    {
            //        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, Color.AliceBlue);
            //    }
            //    bitmap.Save(Server.MapPath("~/Files/result.bmp"), ImageFormat.Bmp);
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