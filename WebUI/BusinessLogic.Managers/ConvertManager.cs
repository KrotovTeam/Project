using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Abstraction;
using BusinessLogic.Dtos;
using Common.Constants;
using Point = BusinessLogic.Dtos.Point;

namespace BusinessLogic.Managers
{
    public class ConvertManager : IConvertManager
    {
        /// <summary>
        /// Асинхронное преобразование снимка в точки
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        /// <returns></returns>
        public Task<IList<Point>> ConvertSnapshotAsync(string fileName)
        {
            return Task.Run(() =>
            {
                using (var img = new Bitmap(fileName))
                {
                    var result = new List<Point>();
                    for (var i = 0; i < img.Width; i++)
                    {
                        for (var j = 0; j < img.Height; j++)
                        {
                            result.Add(new Point
                            {
                                Latitude = i,
                                Longitude = j,
                                Value = img.GetPixel(i, j).R
                            });
                        }
                    }
                    return (IList<Point>)result;
                }
            });
        }

        /// <summary>
        /// Преобразование списков точек из снимка в точки для кластеризации
        /// </summary>
        /// <param name="points">Списки с данными</param>
        /// <param name="channels">Список каналов</param>
        /// <returns></returns>
        public IList<ClusterPoint> ConvertListsPoints(IList<IList<Point>> points, IList<ChannelEnum> channels)
        {
            if (points.Count() != channels.Count())
            {
                throw new Exception("Количество списков точек не соответствует количеству каналов");
            }

            var count = points.ElementAt(0).Count();
            if (points.Skip(1).Any(list => list.Count() != count))
            {
                throw new Exception("Списки точек содержат разные количества точек");
            }

            var result = new List<ClusterPoint>();
            for (var i = 0; i < count; i++)
            {
                var item = points.ElementAt(0).ElementAt(i);
                var clusterPoint = new ClusterPoint {Latitude = item.Latitude, Longitude = item.Longitude};
                for (var j = 0; j < points.Count(); j++)
                {
                    clusterPoint.Values.Add(channels.ElementAt(j), points.ElementAt(j).ElementAt(i).Value);
                }
                result.Add(clusterPoint);
            }
            return result;
        }
    }
}
