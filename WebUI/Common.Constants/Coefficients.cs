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
        public const double Ch1 = (Ch1High - Ch1Low)/_byte;
        public const double Ch1Low = 8915f;
        public const double Ch1High = 9685f;

        /// <summary>
        /// Коэффициент для канала 2
        /// </summary>
        public const double Ch2 = (Ch2High - Ch2Low)/_byte;
        public const double Ch2Low = 8073.52f;
        public const double Ch2High = 9027.85f;

        /// <summary>
        /// Коэффициент для канала 3
        /// </summary>
        public const double Ch3 = (Ch3High - Ch3Low)/_byte;
        public const double Ch3Low = 7394.89f;
        public const double Ch3High = 8889.66f;

        /// <summary>
        /// Коэффициент для канала 4
        /// </summary>
        public const double Ch4 = (Ch4High - Ch4Low)/_byte;
        //        public const float Ch4Low = 6442.92f;
        //        public const float Ch4High = 8246.47f;
        public const double Ch4Low = 6300.0f;
        public const double Ch4High = 6800.0f;

        /// <summary>
        /// Коэффициент для канала 5
        /// </summary>
        public const double Ch5 = (Ch5High - Ch5Low)/_byte;
        //        public const float Ch5Low = 11572.7f;
        //        public const float Ch5High = 21692.3f;
        public const double Ch5Low = 8450.0f;
        public const double Ch5High = 8850.0f;

        /// <summary>
        /// Коэффициент для канала 6
        /// </summary>
        public const double Ch6 = (Ch6High - Ch6Low)/_byte;
        public const double Ch6Low = 8170.02f;
        public const double Ch6High = 13780.8f;

        /// <summary>
        /// Коэффициент для канала 7
        /// </summary>
        public const double Ch7 = (Ch7High - Ch7Low)/_byte;
        public const double Ch7Low = 6487.5f;
        public const double Ch7High = 10046.5f;

        public static Tuple<double, double> GetCoefficientForConvert(ChannelEnum channel)
        {
            switch (channel)
            {
                case ChannelEnum.Channel1:
                    return new Tuple<double, double>(Ch1, Ch1Low);
                case ChannelEnum.Channel2:
                    return new Tuple<double, double>(Ch2, Ch2Low);
                case ChannelEnum.Channel3:
                    return new Tuple<double, double>(Ch3, Ch3Low);
                case ChannelEnum.Channel4:
                    return new Tuple<double, double>(Ch4, Ch4Low);
                case ChannelEnum.Channel5:
                    return new Tuple<double, double>(Ch5, Ch5Low);
                case ChannelEnum.Channel6:
                    return new Tuple<double, double>(Ch6, Ch6Low);
                case ChannelEnum.Channel7:
                    return new Tuple<double, double>(Ch7, Ch7Low);
                default:
                    throw new Exception("Не верно задан канал");
            }
        }
    }
}