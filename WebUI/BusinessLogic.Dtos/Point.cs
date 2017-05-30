namespace BusinessLogic.Dtos
{
    /// <summary>
    /// Точка снимка
    /// </summary>
    public class Point : GeographicalPoint
    {
        /// <summary>
        /// Значение в точке снимка
        /// </summary>
        public double Value { get; set; }
    }
}
