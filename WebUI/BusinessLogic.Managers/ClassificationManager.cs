using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using BusinessLogic.Abstraction;
using BusinessLogic.Dtos;
using Common.Constants;

namespace BusinessLogic.Managers
{
    public class ClassificationManager : IClassificationManager
    {
        public IEnumerable<Cluster> Classify(IEnumerable<Point> points, ChannelEnum channel)
        {
            //Шаг 1 алгоритма
            var clusterCount = 3;
            var tettaN = 100;
            var tettaS = 2;
            var tettaC = 2;
            var L = 0;
            var I = 1;
            var minNumberOfCluster = 0;

            var clusterCenters = GetClustersCenters(clusterCount);

            var Clusters = InitClusters(clusterCount, clusterCenters);

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
