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
        /// <param name="keys">Ключи по которым происходит классификация</param>
        /// <param name="profile">Профайл с параметрами кластеризации</param>
        /// <returns></returns>
        IEnumerable<Cluster> Clustering(IEnumerable<ResultingPoint> points, IEnumerable<CoordinateSystemEnum> keys, ClusteringProfile profile = null);

        /// <summary>
        /// Метод определяет изменения значения вегетационного индекса на снимке c прошлого по текущий год
        /// </summary>
        /// <param name="lastYearClusters"></param>
        /// <param name="currentYearClusters"></param>
        /// <returns></returns>
        IList<ResultingPoint> Compare(IEnumerable<Cluster> lastYearClusters, IEnumerable<Cluster> currentYearClusters);

        /// <summary>
        /// Определение динамики по снимкам
        /// </summary>
        /// <param name="lastYearPoints">Точки для прошлого года</param>
        /// <param name="currentYearPoints">Актуальные точки</param>
        /// <returns></returns>
        IList<ResultingPoint> DeterminationDinamics(IList<ClusterPoint> lastYearPoints, IList<ClusterPoint> currentYearPoints);
    }
}
