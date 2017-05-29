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
        /// <param name="profile">Профайл с параметрами кластеризации</param>
        /// <returns></returns>
        IEnumerable<Cluster> Clustering(IEnumerable<ClusterPoint> points, IEnumerable<ChannelEnum> channels, ClusteringProfile profile = null);
        
        /// <summary>
        /// Установка вегетационного индекса кластерам(NDVI) и определение цвета кластера
        /// </summary>
        /// <param name="clusters">Входные кластеры</param>
        void SetNdviForClusters(IList<Cluster> clusters);

        /// <summary>
        /// Метод определяет изменения значения вегетационного индекса на снимке c прошлого по текущий год
        /// </summary>
        /// <param name="lastYearPoints"></param>
        /// <param name="currentYearPoints"></param>
        /// <returns></returns>
        IList<ResultingPoint> Compare(IEnumerable<Cluster> lastYearClusters, IEnumerable<Cluster> currentYearClusters);
    }
}
