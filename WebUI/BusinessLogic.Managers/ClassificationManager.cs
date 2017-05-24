using System;
using System.Collections.Generic;
using System.Linq;
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
        private int _tettaS = 50;

        /// <summary>
        /// Параметр, характеризующий компактность
        /// </summary>
        private int _tettaC = 200;

        /// <summary>
        /// Максимальное количество пар центров кластеров, которые можно объединить
        /// </summary>
        private int _l = 2;

        /// <summary>
        /// Допустимое число циклов итерации
        /// </summary>
        private int _i = 4;

        /// <summary>
        /// Кластеры
        /// </summary>
        private IList<Cluster> _z;

        /// <summary>
        /// Среднее расстояние между объектами входящих в кластер
        /// </summary>
        private IList<float> _dj = new List<float>();

        /// <summary>
        /// Обобщенное среднее расстояние между объектами, находящимися в отдельных кластерах, и соответствующими
        /// центрами кластеров
        /// </summary>
        private float _d;

        /// <summary>
        /// Вектор среднеквадратичного отколнения
        /// </summary>
        private IList<Dictionary<ChannelEnum, float>> _sigmaj = new List<Dictionary<ChannelEnum, float>>();

        /// <summary>
        /// Максимальная компонента в векторе среднеквадратичного отклонения
        /// </summary>
        private IList<Tuple<ChannelEnum, float>> _sigmajMax = new List<Tuple<ChannelEnum, float>>();

        /// <summary>
        /// Коэффициент при высчитывании gammaj
        /// </summary>
        private float _coefficient = 0.5f;

        /// <summary>
        /// Расстояния между всеми парами кластеров
        /// </summary>
        private IList<Tuple<Cluster, Cluster, float>> _dij = new List<Tuple<Cluster, Cluster, float>>();

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

            for (var i = 1; i <= _i; i++)
            {
                foreach (var cluster in _z)
                {
                    ((List<RawData>)cluster.Points).Clear();
                }
                //Шаг 2 алгоритма
                foreach (var point in points)
                {
                    var min = float.MaxValue;
                    var perfectCluster = _z.ElementAt(0);
                    foreach (var cluster in _z)
                    {
                        var tmpValue = EuclideanDistance(point.Values, cluster.CenterCluster);
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
                _dj.Clear();
                foreach (var cluster in _z)
                {
                    var value = cluster.Points.Sum(point => EuclideanDistance(point.Values, cluster.CenterCluster)) / cluster.Points.Count();
                    _dj.Add(value);
                }

                //Шаг 6 алгоритма
                _d = 0f;
                for (var k = 0; k < _z.Count; k++)
                {
                    _d += _z[k].Points.Count()*_dj[k];
                }
                _d /= points.Count();

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
                    if (Step8(channels))
                    {
                        continue;
                    }
                    Step11();
                }
            }

            return _z;
        }

        /// <summary>
        /// 8-й шаг алгоритма
        /// </summary>
        private bool Step8(IEnumerable<ChannelEnum> channels)
        {
            _sigmaj.Clear();
            _sigmajMax.Clear();

            //Шаг 8
            foreach (var cluster in _z)
            {
                var dictionary = new Dictionary<ChannelEnum, float>();
                foreach (var channel in channels)
                {
                    var value = cluster.Points.Sum(point => (float) Math.Pow((point.Values[channel] - cluster.CenterCluster[channel]), 2));
                    value = (float) Math.Sqrt(value/cluster.Points.Count());
                    dictionary.Add(channel,value);
                }
                _sigmaj.Add(dictionary);
            }

            //Шаг 9
            for (var i = 0; i < _z.Count; i++)
            {
                var max = float.MinValue;
                var channel = ChannelEnum.Unknown;
                foreach (var key in _sigmaj.ElementAt(i).Keys)
                {
                    if (_sigmaj.ElementAt(i)[key] > max)
                    {
                        max = _sigmaj.ElementAt(i)[key];
                        channel = key;
                    }
                }
                _sigmajMax.Add(new Tuple<ChannelEnum, float>(channel, max));
            }

            //Шаг 10
            for (var i = 0; i < _z.Count; i++)
            {
                if (_sigmajMax[i].Item2 > _tettaS && ((_dj[i] > _d && _z[i].Points.Count() > (2*(_tettaN + 1))) ||
                    (_z.Count <= _clustersCount/2)))
                {
                    var gammaj = _coefficient*_sigmajMax[i].Item2;

                    var newCluster = new Cluster();
                    foreach (var key in _z[i].CenterCluster.Keys)
                    {
                        newCluster.CenterCluster.Add(key, _z[i].CenterCluster[key]);
                    }

                    //zj+
                    ((List<RawData>)_z[i].Points).Clear();
                    _z[i].CenterCluster[_sigmajMax[i].Item1] = _z[i].CenterCluster[_sigmajMax[i].Item1] + gammaj;

                    //zj-
                    newCluster.CenterCluster[_sigmajMax[i].Item1] = newCluster.CenterCluster[_sigmajMax[i].Item1] - gammaj;
                    _z.Add(newCluster);

                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 11-й шаг алгоритма
        /// </summary>
        private void Step11()
        {
            //Шаг 11
            for (var i = 0; i < _z.Count - 1; i++)
            {
                var tuple = new Tuple<Cluster, Cluster, float>(_z[i], _z[i + 1], EuclideanDistance(_z[i].CenterCluster, _z[i + 1].CenterCluster));
                _dij.Add(tuple);

                _z[i].IsJoined = false;
            }

            //Шаг 12
            var clustersForJoin = _dij.Where(p => p.Item3 < _tettaC).OrderBy(p => p.Item3).Take(_l);

            //Шаг 13
            foreach (var cluster in clustersForJoin)
            {
                if (!cluster.Item1.IsJoined && !cluster.Item2.IsJoined)
                {
                    cluster.Item1.IsJoined = cluster.Item2.IsJoined = true;
                    var newCluster = new Cluster();
                    foreach (var key in cluster.Item1.CenterCluster.Keys)
                    {
                        var value1 = cluster.Item1.CenterCluster[key] * cluster.Item1.Points.Count();
                        var value2 = cluster.Item2.CenterCluster[key] * cluster.Item2.Points.Count();
                        newCluster.CenterCluster[key] = (value1 + value2) / (cluster.Item1.Points.Count() + cluster.Item2.Points.Count());
                    }
                    _z.Add(newCluster);
                    _z.Remove(cluster.Item1);
                    _z.Remove(cluster.Item2);
                }
            }
        }

        private float EuclideanDistance(Dictionary<ChannelEnum, float> point, Dictionary<ChannelEnum, float> center)
        {
            var keys = point.Keys;
            var result = keys.Sum(key => (float) Math.Pow((point[key] - center[key]), 2));
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
                    CenterCluster = dictinary
                });
            }
            return result;
        }
    }
}