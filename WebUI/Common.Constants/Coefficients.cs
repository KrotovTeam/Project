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
        public const double Ch1 = (9685 - Ch1Low)/_byte;
        public const double Ch1Low = 8915;

        /// <summary>
        /// Коэффициент для канала 2
        /// </summary>
        public const double Ch2 = (9027.85 - Ch2Low)/_byte;
        public const double Ch2Low = 8073.52;
        /// <summary>
        /// Коэффициент для канала 3
        /// </summary>
        public const double Ch3 = (8889.66 - Ch3Low)/_byte;
        public const double Ch3Low = 7394.89;

        /// <summary>
        /// Коэффициент для канала 4
        /// </summary>
        public const double Ch4 = (8246.47 - Ch4Low)/_byte;
        public const double Ch4Low = 6442.92;

        /// <summary>
        /// Коэффициент для канала 5
        /// </summary>
        public const double Ch5 = (21692.3 - Ch5Low)/_byte;
        public const double Ch5Low = 11572.7;

        /// <summary>
        /// Коэффициент для канала 6
        /// </summary>
        public const double Ch6 = (13780.8 - Ch6Low)/_byte;
        public const double Ch6Low = 8170.02;

        /// <summary>
        /// Коэффициент для канала 7
        /// </summary>
        public const double Ch7 = (10046.5 - Ch7Low)/_byte;
        public const double Ch7Low = 6487.5;
    }
}