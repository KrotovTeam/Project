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
        private int _tettaN = 1000;

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
        private IList<Cluster> _z;

        /// <summary>
        /// Среднее расстояние между объектами входящих в кластер
        /// </summary>
        private IList<float> _dj;

        /// <summary>
        /// Обобщенное среднее расстояние между объектами, находящимися в отдельных кластерах, и соответствующими
        /// центрами кластеров
        /// </summary>
        private float _d;

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
                    var tmpValue = Math.Abs(point.Value - cluster.CenterCluster);
                    if (tmpValue < min)
                    {
                        min = tmpValue;
                        perfectCluster = cluster;
                    }
                }
                ((List<Point>)perfectCluster.Points).Add(point);
            }

            //Шаг 3 алгоритма
            var clustersToDelete = _z.Where(p => p.Points.Count() < _tettaN).ToList();
            foreach (var cluster in clustersToDelete)
            {
                _z.Remove(cluster);
            }

            //Шаг 4 алгоритма
            foreach (var cluster in _z)
            {
                cluster.CenterCluster = cluster.Points.Sum(p => p.Value) / cluster.Points.Count();
            }

            //Шаг 5 алгоритма
            _dj = new List<float>();
            foreach (var cluster in _z)
            {
                _dj.Add(cluster.Points.Sum(p => Math.Abs(p.Value - cluster.CenterCluster) / cluster.Points.Count()));
            }

            //Шаг 6 алгоритма
            _d = 0;
            for (var i = 0; i < _z.Count; i++)
            {
                _d += _z[i].Points.Count()*_dj[i];
            }
            _d /= points.Count();

            return _z;
        }

        /// <summary>
        /// Реализация первого шага алгоритма Isodata.
        /// Определение начальных центров кластеров.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private IList<Cluster> Init(IEnumerable<Point> points)
        {
            var step = points.Count()/_clustersCount;
            var result = new List<Cluster>();
            for (var i = step; i < points.Count(); i += step)
            {
                result.Add(new Cluster
                {
                    CenterCluster = points.ElementAt(i).Value,
                    Points = new List<Point>()
                });
            }
            return result;
        }
    }
}