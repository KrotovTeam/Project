using System;
using System.Drawing;
using System.Threading.Tasks;
using BusinessLogic.Abstraction;
using Common.Constants;

namespace BusinessLogic.Managers
{
    public class ConvertManager : IConvertManager
    {
        public async Task<float[,]> ConvertSnapshot(string fileName, ChannelEnum channel)
        {
            return await Task.Run(() =>
            {
                var img = new Bitmap(fileName);
                var result = new float[img.Width, img.Height];
                var coefficient = GetCoefficient(channel);
                for (var i = 0; i < img.Width; i++)
                {
                    for (var j = 0; j < img.Height; j++)
                    {
                        result[i, j] = coefficient.Item1 * img.GetPixel(i, j).R + coefficient.Item2;
                    }
                }
                return result;
            });
        }

        private Tuple<float, float> GetCoefficient(ChannelEnum channel)
        {
            switch (channel)
            {
                case ChannelEnum.Channel1:
                    return new Tuple<float, float>(Coefficients.Ch1, Coefficients.Ch1Low);
                case ChannelEnum.Channel2:
                    return new Tuple<float, float>(Coefficients.Ch2, Coefficients.Ch2Low);
                case ChannelEnum.Channel3:
                    return new Tuple<float, float>(Coefficients.Ch3, Coefficients.Ch3Low);
                case ChannelEnum.Channel4:
                    return new Tuple<float, float>(Coefficients.Ch4, Coefficients.Ch4Low);
                case ChannelEnum.Channel5:
                    return new Tuple<float, float>(Coefficients.Ch5, Coefficients.Ch5Low);
                case ChannelEnum.Channel6:
                    return new Tuple<float, float>(Coefficients.Ch6, Coefficients.Ch6Low);
                case ChannelEnum.Channel7:
                    return new Tuple<float, float>(Coefficients.Ch7, Coefficients.Ch7Low);
                default:
                    return new Tuple<float, float>(0, 0);
            }
        }
    }
}
