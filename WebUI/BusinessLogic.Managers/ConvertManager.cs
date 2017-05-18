using BusinessLogic.Abstraction;
using Common.Constants;
using OpenCvSharp;

namespace BusinessLogic.Managers
{
    public class ConvertManager : IConvertManager
    {
        public double[,] ConvertSnapshot(string fileName, ChannelEnum channel)
        {
            var img = new Mat(fileName, ImreadModes.GrayScale);
            //var img = OpenCvSharp.Cv2.LoadImage(fileName, LoadMode.GrayScale);
            var result = new double[img.Height, img.Width];
            var coefficient = GetCoefficient(channel);
            var coefficientLow = GetCoefficientLow(channel);
            for (var i = 0; i < img.Height; i++)
            {
                for (var j = 0; j < img.Width; j++)
                {
                    //result[i, j] = coefficient * img[i, j][0] + coefficientLow;
                }
            }
            return result;
        }

        private double GetCoefficient(ChannelEnum channel)
        {
            switch (channel)
            {
                case ChannelEnum.Channel1:
                    return Coefficients.Ch1;
                case ChannelEnum.Channel2:
                    return Coefficients.Ch2;
                case ChannelEnum.Channel3:
                    return Coefficients.Ch3;
                case ChannelEnum.Channel4:
                    return Coefficients.Ch4;
                case ChannelEnum.Channel5:
                    return Coefficients.Ch5;
                case ChannelEnum.Channel6:
                    return Coefficients.Ch6;
                case ChannelEnum.Channel7:
                    return Coefficients.Ch7;
                default:
                    return 0;
            }
        }

        private double GetCoefficientLow(ChannelEnum channel)
        {
            switch (channel)
            {
                case ChannelEnum.Channel1:
                    return Coefficients.Ch1Low;
                case ChannelEnum.Channel2:
                    return Coefficients.Ch2Low;
                case ChannelEnum.Channel3:
                    return Coefficients.Ch3Low;
                case ChannelEnum.Channel4:
                    return Coefficients.Ch4Low;
                case ChannelEnum.Channel5:
                    return Coefficients.Ch5Low;
                case ChannelEnum.Channel6:
                    return Coefficients.Ch6Low;
                case ChannelEnum.Channel7:
                    return Coefficients.Ch7Low;
                default:
                    return 0;
            }
        }
    }
}
