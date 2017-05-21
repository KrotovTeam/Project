using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using BusinessLogic.Abstraction;
using Common.Constants;
using Point = BusinessLogic.Dtos.Point;

namespace BusinessLogic.Managers
{
    public class ConvertManager : IConvertManager
    {
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
    }
}
