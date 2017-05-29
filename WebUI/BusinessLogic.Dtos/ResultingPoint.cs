using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Dtos
{
    /// <summary>
    /// Результирующая точка после сравнения снимков за текущий год со снимками за предыдущие года
    /// </summary>
    public class ResultingPoint
    {
        /// <summary>
        /// Координата по X
        /// </summary>
        public double CoordX { get; set; }

        /// <summary>
        /// Координата по Y
        /// </summary>
        public double CoordY { get; set; }

        /// <summary>
        /// Результирующий вегетационный индекс после сравнения снимков за текущий и предыдущие года.
        /// </summary>
        public double Ndvi { get; set; }

        /// <summary>
        /// Флаг, означающий, что на снимке за текущий год по сравнению с прошлыми обнаружились изменения в данной точке
        /// </summary>
        public bool IsChanged { get; set; }

    }
}
