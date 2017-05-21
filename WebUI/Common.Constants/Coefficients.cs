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
        public const float Ch1 = (9685f - Ch1Low)/_byte;
        public const float Ch1Low = 8915f;

        /// <summary>
        /// Коэффициент для канала 2
        /// </summary>
        public const float Ch2 = (9027.85f - Ch2Low)/_byte;
        public const float Ch2Low = 8073.52f;
        /// <summary>
        /// Коэффициент для канала 3
        /// </summary>
        public const float Ch3 = (8889.66f - Ch3Low)/_byte;
        public const float Ch3Low = 7394.89f;

        /// <summary>
        /// Коэффициент для канала 4
        /// </summary>
        public const float Ch4 = (8246.47f - Ch4Low)/_byte;
        public const float Ch4Low = 6442.92f;

        /// <summary>
        /// Коэффициент для канала 5
        /// </summary>
        public const float Ch5 = (21692.3f - Ch5Low)/_byte;
        public const float Ch5Low = 11572.7f;

        /// <summary>
        /// Коэффициент для канала 6
        /// </summary>
        public const float Ch6 = (13780.8f - Ch6Low)/_byte;
        public const float Ch6Low = 8170.02f;

        /// <summary>
        /// Коэффициент для канала 7
        /// </summary>
        public const float Ch7 = (10046.5f - Ch7Low)/_byte;
        public const float Ch7Low = 6487.5f;
    }
}