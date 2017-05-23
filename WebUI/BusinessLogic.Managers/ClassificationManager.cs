using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using BusinessLogic.Abstraction;
using BusinessLogic.Dtos;
using Common.Constants;

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
        private int _i = 2;

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

        /// <summary>
        /// Вектор среднеквадратичного отколнения
        /// </summary>
        private IList<float> _sigmaj;

        #endregion

        /// <summary>
        /// Кластеризация по алгоритму Isodata
        /// </summary>
        /// <param name="points">Исходные точки</param>
        /// <returns></returns>
        public IEnumerable<Cluster> Clustering(IEnumerable<RawData> points, IEnumerable<ChannelEnum> channels)
        {
            if (points == null)
            {
                throw new Exception("Точки для кластеризации не заданы.");
            }
            //Шаг 1 алгоритма
            _z = Init(points, channels);

            for (var i = 1; i < _i; i++)
            {
                //Шаг 2 алгоритма
                foreach (var point in points)
                {
                    var min = float.MaxValue;
                    var perfectCluster = _z.ElementAt(0);
                    foreach (var cluster in _z)
                    {
                        var tmpValue = EuclideanDistance(point, cluster.CenterCluster);
                        if (tmpValue < min)
                        {
                            min = tmpValue;
                            perfectCluster = cluster;
                        }
                    }
                    ((List<RawData>) perfectCluster.Points).Add(point);
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
                    var dictionary = new Dictionary<ChannelEnum, float>();
                    foreach (var channel in channels)
                    {
                        dictionary.Add(channel,0f);
                        foreach (var point in cluster.Points)
                        {
                            dictionary[channel] += point.Values[channel];
                        }
                        dictionary[channel] /= cluster.Points.Count();
                    }
                    cluster.CenterCluster = dictionary;
                }
                
                //Шаг 5 алгоритма
                _dj = new List<float>();
                foreach (var cluster in _z)
                {
                    var value = cluster.Points.Sum(point => EuclideanDistance(point, cluster.CenterCluster)) / cluster.Points.Count();
                    _dj.Add(value);
                }

                
                //Шаг 6 алгоритма
                _d = 0f;
                for (var k = 0; k < _z.Count; k++)
                {
                    _d += _z[k].Points.Count()*_dj[k];
                }
                _d /= points.Count();

                /*
                //Шаг 7 алгоритма
                if (i == _i)
                {
                    _tettaC = 0;
                    Step11();
                }
                else if (_z.Count > 2*_clustersCount || i%2 == 0)
                {
                    Step11();
                }
                else
                {
                    Step8();
                }*/
            }

            return _z;
        }

        /// <summary>
        /// 8-й шаг алгоритма
        /// </summary>
        private void Step8()
        {
        }

        /// <summary>
        /// 11-й шаг алгоритма
        /// </summary>
        private void Step11()
        {
        }

        private float EuclideanDistance(RawData point, Dictionary<ChannelEnum, float> center)
        {
            var keys = point.Values.Keys;
            var result = keys.Sum(key => (float) Math.Pow((point.Values[key] - center[key]), 2));
            return (float)Math.Sqrt(result);
        }

        /// <summary>
        /// Реализация первого шага алгоритма Isodata.
        /// Определение начальных центров кластеров.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private IList<Cluster> Init(IEnumerable<RawData> points, IEnumerable<ChannelEnum> channels)
        {
            var step = points.Count()/_clustersCount;
            var result = new List<Cluster>();
            for (var i = step; i < points.Count(); i += step)
            {
                var dictinary = new Dictionary<ChannelEnum, float>();
                for (var j = 0; j < channels.Count(); j++)
                {
                    var channel = channels.ElementAt(j);
                    dictinary.Add(channel, points.ElementAt(j).Values[channel]);
                }
                result.Add(new Cluster
                {
                    CenterCluster = dictinary,
                    Points = new List<RawData>()
                });
            }
            return result;
        }
    }
}