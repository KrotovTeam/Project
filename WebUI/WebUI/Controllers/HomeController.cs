using System.Collections.Generic;
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
        public IDrawManager _drawManager { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> About()
        {
            var pathToFile1 = Server.MapPath("~/Files/TestChannel4.tif");
            var pathToFile2 = Server.MapPath("~/Files/TestChannel5.tif");
            var pathToFile3 = Server.MapPath("~/Files/TestChannel4New.tif");
            var pathToFile4 = Server.MapPath("~/Files/TestChannel5New.tif");

            var channel4 = _convertManager.ConvertSnapshotAsync(pathToFile1);
            var channel5 = _convertManager.ConvertSnapshotAsync(pathToFile2);
            var channel4New = _convertManager.ConvertSnapshotAsync(pathToFile3);
            var chennel5New = _convertManager.ConvertSnapshotAsync(pathToFile4);

            await Task.WhenAll(channel4, channel5, channel4New, chennel5New);

            var channels = new List<ChannelEnum> {ChannelEnum.Channel4, ChannelEnum.Channel5};
            var clusterPoints =_convertManager.ConvertListsPoints(new List<IList<Point>> {channel4.Result, channel5.Result}, channels);
            var newClusterPoints = _convertManager.ConvertListsPoints(new List<IList<Point>> { channel4New.Result, chennel5New.Result }, channels);

            //_drawManager.DrawProcessedSnapshot(clusterPoints, Server.MapPath("~/Files/Old.bmp"), 999, 999);
            //_drawManager.DrawProcessedSnapshot(newClusterPoints, Server.MapPath("~/Files/New.bmp"), 999, 999);
            var clusteringProfile = BusinessLogic.Dtos.ClusteringProfile.DefaultProfile();
            var clusters = _classificationManager.Clustering(clusterPoints, channels, clusteringProfile);
            _drawManager.DrawProcessedSnapshot(clusterPoints, Server.MapPath("~/Files/Old1111.bmp"), 999, 999);
            var resultingPoints = _classificationManager.DeterminationDinamics(clusterPoints, newClusterPoints);

            _drawManager.DrawDinamics(resultingPoints, Server.MapPath("~/Files/Dinamics.bmp"), 999, 999);

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