using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using BusinessLogic.Abstraction;
using BusinessLogic.Dtos;
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
            var clusterPoints =_convertManager.ConvertListsPoints(new List<IEnumerable<Point>> {channel4.Result, channel5.Result}, channels);

            var clusters = _classificationManager.Clustering(clusterPoints, channels,new ClusteringProfile{I = 7, TettaS = 2.5, TettaN = 300, TettaC = 10, Coefficient = 0.5, L = 1, СlustersCount = 20});
            _classificationManager.SetNdviForClusters(clusters.ToList());

            using (var bitmap = new Bitmap(999, 999))
            {
                
                foreach (var cluster in clusters)
                {
                    foreach (var point in cluster.Points)
                    {
                        bitmap.SetPixel((int)point.CoordX, (int)point.CoordY, cluster.ClusterColor);
                    }
                }

                bitmap.Save(Server.MapPath("~/Files/result.bmp"), ImageFormat.Bmp);
            }
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