using System.Collections.Generic;
using Common.Constants;

namespace BusinessLogic.Dtos
{
    /// <summary>
    /// Результирующая точка после сравнения снимков за текущий год со снимками за предыдущие года
    /// </summary>
    public class ResultingPoint
    {
        public Dictionary<CoordinateSystemEnum, int> Values { get; set; }

        /// <summary>
        /// Результирующий вегетационный индекс после сравнения снимков за текущий и предыдущие года.
        /// </summary>
        public double Ndvi { get; set; }

        public ResultingPoint()
        {
            Values = new Dictionary<CoordinateSystemEnum, int>();
        }
    }
}
