using System;

namespace Common.Constants
{
    /// <summary>
    /// Коэффициенты для расчетов
    /// </summary>
    public class Coefficients
    {
        private const int _byte = 255;

        /// <summary>
        /// Коэффициент для канала 1
        /// </summary>
        public const float Ch1 = (Ch1High - Ch1Low)/_byte;
        public const float Ch1Low = 8915f;
        public const float Ch1High = 9685f;

        /// <summary>
        /// Коэффициент для канала 2
        /// </summary>
        public const float Ch2 = (Ch2High - Ch2Low)/_byte;
        public const float Ch2Low = 8073.52f;
        public const float Ch2High = 9027.85f;
        /// <summary>
        /// Коэффициент для канала 3
        /// </summary>
        public const float Ch3 = (Ch3High - Ch3Low)/_byte;
        public const float Ch3Low = 7394.89f;
        public const float Ch3High = 8889.66f;

        /// <summary>
        /// Коэффициент для канала 4
        /// </summary>
        public const float Ch4 = (Ch4High - Ch4Low)/_byte;
        public const float Ch4Low = 6442.92f;
        public const float Ch4High = 8246.47f;

        /// <summary>
        /// Коэффициент для канала 5
        /// </summary>
        public const float Ch5 = (Ch5High - Ch5Low)/_byte;
        public const float Ch5Low = 11572.7f;
        public const float Ch5High = 21692.3f;

        /// <summary>
        /// Коэффициент для канала 6
        /// </summary>
        public const float Ch6 = (Ch6High - Ch6Low)/_byte;
        public const float Ch6Low = 8170.02f;
        public const float Ch6High = 13780.8f;

        /// <summary>
        /// Коэффициент для канала 7
        /// </summary>
        public const float Ch7 = (Ch7High - Ch7Low)/_byte;
        public const float Ch7Low = 6487.5f;
        public const float Ch7High = 10046.5f;

        public static Tuple<float, float> GetCoefficientForConvert(ChannelEnum channel)
        {
            switch (channel)
            {
                case ChannelEnum.Channel1:
                    return new Tuple<float, float>(Ch1, Ch1Low);
                case ChannelEnum.Channel2:
                    return new Tuple<float, float>(Ch2, Ch2Low);
                case ChannelEnum.Channel3:
                    return new Tuple<float, float>(Ch3, Ch3Low);
                case ChannelEnum.Channel4:
                    return new Tuple<float, float>(Ch4, Ch4Low);
                case ChannelEnum.Channel5:
                    return new Tuple<float, float>(Ch5, Ch5Low);
                case ChannelEnum.Channel6:
                    return new Tuple<float, float>(Ch6, Ch6Low);
                case ChannelEnum.Channel7:
                    return new Tuple<float, float>(Ch7, Ch7Low);
                default:
                    throw new Exception("Не верно задан канал");
            }
        }

        public static Tuple<float, float> GetRangeForClassification(ChannelEnum channel)
        {
            switch (channel)
            {
                case ChannelEnum.Channel1:
                    return new Tuple<float, float>(Ch1High, Ch1Low);
                case ChannelEnum.Channel2:
                    return new Tuple<float, float>(Ch2High, Ch2Low);
                case ChannelEnum.Channel3:
                    return new Tuple<float, float>(Ch3High, Ch3Low);
                case ChannelEnum.Channel4:
                    return new Tuple<float, float>(Ch4High, Ch4Low);
                case ChannelEnum.Channel5:
                    return new Tuple<float, float>(Ch5High, Ch5Low);
                case ChannelEnum.Channel6:
                    return new Tuple<float, float>(Ch6High, Ch6Low);
                case ChannelEnum.Channel7:
                    return new Tuple<float, float>(Ch7High, Ch7Low);
                default:
                    throw new Exception("Не верно задан канал");
            }
        }
    }
}