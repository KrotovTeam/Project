using System;
using System.Collections.Generic;
using BusinessLogic.Abstraction;
using BusinessLogic.Dtos;

namespace BusinessLogic.Managers
{
    public class ClassificationManager : IClassificationManager
    {

        #region Fields

        /// <summary>
        /// Необходимое число кластеров
        /// </summary>
        private int _clustersCount = 3;

        /// <summary>
        /// Параметр, с которым сравнивается количество выборочных образов, вошедших в кластер
        /// </summary>
        private int _tettaN = 100;

        /// <summary>
        /// Параметр, характеризующий среднеквадратическое отклонение
        /// </summary>
        private int _tettaS = 2;

        /// <summary>
        /// Параметр, характеризующий компактность
        /// </summary>
        private int _tettaC = 2;

        /// <summary>
        /// Максимальное количество пар центров кластеров, которые можно объединить
        /// </summary>
        private int _l = 0;

        /// <summary>
        /// Допустимое число циклов итерации
        /// </summary>
        private int _i = 1;

        #endregion

        public IEnumerable<Cluster> Clustering(IEnumerable<Point> points)
        {
            //Шаг 1 алгоритма
            var minNumberOfCluster = 0;

            var clusterCenters = GetClustersCenters(_clustersCount);

            var Clusters = InitClusters(_clustersCount, clusterCenters);

            //Шаг 2 алгоритма
            foreach (var point in points)
            {
                var minValue = 0f;
                for (var i = 0; i < Clusters.Count - 2; i++)
                {

                    var currentValue = Math.Abs(point.Value - Clusters[i].CenterCluster.Value);

                    if (currentValue < Math.Abs(point.Value - Clusters[i + 1].CenterCluster.Value) && currentValue < minValue)
                    {
                        minNumberOfCluster = i;
                        minValue = Math.Abs(point.Value - Clusters[i].CenterCluster.Value);
                    }
                }

                ((List<Point>) Clusters[minNumberOfCluster].Points).Add(point);
                
            }


        }

        private List<Cluster> InitClusters(int clusterCount, Point[] clustersCenters)
        {
            Cluster cluster = new Cluster();
            List<Cluster> clusters= new List<Cluster>();

            for (int i = 0; i < clusterCount; i++)
            {
                 clusters.Add(cluster);
                 clusters[i].CenterCluster = clustersCenters[i];
                 clusters[i].Points = new List<Point>();
            }

            return clusters;

        }
        private Point[] GetClustersCenters(int clusterCount)
        {
            throw new NotImplementedException();
        }
    }
}
