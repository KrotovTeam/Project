using System.Collections.Generic;
using Common.Constants;

namespace BusinessLogic.Dtos
{
    /// <summary>
    /// Сырые данные для кластеризации
    /// </summary>
    public class RawData
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
        /// Значения по каналам
        /// </summary>
        public Dictionary<ChannelEnum, float> Values { get; set; }

        public RawData()
        {
            Values = new Dictionary<ChannelEnum, float>();
        }
    }
}
