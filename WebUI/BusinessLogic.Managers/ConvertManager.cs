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
        /// <param name="channel">Канал</param>
        /// <returns></returns>
        public Task<IEnumerable<Point>> ConvertSnapshotAsync(string fileName, ChannelEnum channel)
        {
            return Task.Run(() =>
            {
                using (var img = new Bitmap(fileName))
                {
                    var result = new List<Point>();
                    var coefficient = Coefficients.GetCoefficientForConvert(channel);
                    for (var i = 0; i < img.Width; i++)
                    {
                        for (var j = 0; j < img.Height; j++)
                        {
                            result.Add(new Point
                            {
                                CoordX = i,
                                CoordY = j,
                                Value = coefficient.Item1 * img.GetPixel(i, j).R + coefficient.Item2
                            });
                        }
                    }
                    return (IEnumerable<Point>)result;
                }
            });
        }

        /// <summary>
        /// Преобразование точек из снимка в данные для кластеризации
        /// </summary>
        /// <param name="dataList">Список с данными</param>
        /// <param name="channels">Каналы</param>
        /// <returns></returns>
        public IEnumerable<RawData> ConvertPointsToRawData(IEnumerable<IEnumerable<Point>> dataList, IEnumerable<ChannelEnum> channels)
        {
            if (dataList.Count() != channels.Count())
            {
                throw new Exception("Количество списков точек не соответствует количеству каналов");
            }
            var count = dataList.ElementAt(0).Count();
            if (dataList.Skip(1).Any(list => list.Count() != count))
            {
                throw new Exception("Списки точек содержат разные количества точек");
            }
            var result = new List<RawData>();
            var listCount = dataList.Count();
            for (var i = 0; i < count; i++)
            {
                var item = dataList.ElementAt(0).ElementAt(i);
                var rawData = new RawData {CoordX = item.CoordX, CoordY = item.CoordY};
                for (var j = 0; j < listCount; j++)
                {
                    rawData.Values.Add(channels.ElementAt(j), dataList.ElementAt(j).ElementAt(i).Value);
                }
                result.Add(rawData);
            }
            return result;
        }
    }
}
