using System.Collections.Generic;
using BusinessLogic.Dtos;
using Common.Constants;

namespace BusinessLogic.Abstraction
{
    public interface IClassificationManager
    {
        /// <summary>
        /// Кластеризация данных методом Isodata
        /// </summary>
        /// <param name="points">Входные данные</param>
        /// <param name="channels">Каналы по которым происходит классификация</param>
        /// <returns></returns>
        IEnumerable<Cluster> Clustering(IEnumerable<ClusterPoint> points, IEnumerable<ChannelEnum> channels);
    }
}
