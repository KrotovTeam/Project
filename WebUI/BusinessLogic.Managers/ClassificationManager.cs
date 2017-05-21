using System;
using System.Collections.Generic;
using System.Linq;
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
        private int _clustersCount = 12;

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
        private int _l = 2;

        /// <summary>
        /// Допустимое число циклов итерации
        /// </summary>
        private int _i = 1;

        /// <summary>
        /// Кластеры
        /// </summary>
        private IEnumerable<Cluster> _z;

        #endregion

        /// <summary>
        /// Кластеризация по алгоритму Isodata
        /// </summary>
        /// <param name="points">Исходные точки</param>
        /// <returns></returns>
        public IEnumerable<Cluster> Clustering(IEnumerable<Point> points)
        {
            if (points == null)
            {
                throw new Exception("Точки для кластеризации не заданы.");
            }
            //Шаг 1 алгоритма
            _z = Init(points);

            //Шаг 2 алгоритма
            foreach (var point in points)
            {
                var min = float.MaxValue;
                var perfectCluster = _z.ElementAt(0);
                foreach (var cluster in _z.Skip(1))
                {
                    var tmpValue = Math.Abs(point.Value - cluster.CenterCluster.Value);
                    if (tmpValue < min)
                    {
                        min = tmpValue;
                        perfectCluster = cluster;
                    }
                }
                ((List<Point>)perfectCluster.Points).Add(point);
            }
            return _z;
        }

        /// <summary>
        /// Реализация первого шага алгоритма Isodata.
        /// Определение начальных центров кластеров.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private IEnumerable<Cluster> Init(IEnumerable<Point> points)
        {
            var step = points.Count()/_clustersCount;
            var result = new List<Cluster>();
            for (var i = step; i < points.Count(); i += step)
            {
                result.Add(new Cluster
                {
                    CenterCluster = points.ElementAt(i),
                    Points = new List<Point>()
                });
            }
            return result;
        }
    }
}