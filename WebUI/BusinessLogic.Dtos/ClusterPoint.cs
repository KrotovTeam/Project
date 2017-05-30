using System.Collections.Generic;
using Common.Constants;

namespace BusinessLogic.Dtos
{
    /// <summary>
    /// Сырые данные для кластеризации
    /// </summary>
    public class ClusterPoint : GeographicalPoint
    {
        /// <summary>
        /// Значения по каналам
        /// </summary>
        public Dictionary<ChannelEnum, double> Values { get; set; }

        public ClusterPoint()
        {
            Values = new Dictionary<ChannelEnum, double>();
        }
    }
}
