using System;
using System.Collections.Generic;
using System.Drawing;
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
        private int _clustersCount;

        /// <summary>
        /// Параметр, с которым сравнивается количество выборочных образов, вошедших в кластер
        /// </summary>
        private int _tettaN;

        /// <summary>
        /// Параметр, характеризующий среднеквадратическое отклонение
        /// </summary>
        private float _tettaS;

        /// <summary>
        /// Параметр, характеризующий компактность
        /// </summary>
        private float _tettaC;

        /// <summary>
        /// Максимальное количество пар центров кластеров, которые можно объединить
        /// </summary>
        private int _l;

        /// <summary>
        /// Допустимое число циклов итерации
        /// </summary>
        private int _i;

        /// <summary>
        /// Кластеры
        /// </summary>
        private IList<Cluster> _z;

        /// <summary>
        /// Среднее расстояние между объектами входящих в кластер
        /// </summary>
        private readonly IList<float> _dj = new List<float>();

        /// <summary>
        /// Обобщенное среднее расстояние между объектами, находящимися в отдельных кластерах, и соответствующими
        /// центрами кластеров
        /// </summary>
        private float _d;

        /// <summary>
        /// Вектор среднеквадратичного отколнения
        /// </summary>
        private readonly IList<Dictionary<ChannelEnum, float>> _sigmaj = new List<Dictionary<ChannelEnum, float>>();

        /// <summary>
        /// Максимальная компонента в векторе среднеквадратичного отклонения
        /// </summary>
        private readonly IList<Tuple<ChannelEnum, float>> _sigmajMax = new List<Tuple<ChannelEnum, float>>();

        /// <summary>
        /// Коэффициент при высчитывании gammaj
        /// </summary>
        private float _coefficient;

        /// <summary>
        /// Расстояния между всеми парами кластеров
        /// </summary>
        private readonly IList<Tuple<Cluster, Cluster, float>> _dij = new List<Tuple<Cluster, Cluster, float>>();

        #endregion

        /// <summary>
        /// Кластеризация данных методом Isodata
        /// </summary>
        /// <param name="points">Входные данные</param>
        /// <param name="channels">Каналы по которым происходит классификация</param>
        /// <param name="profile">Профайл с параметрами кластеризации</param>
        /// <returns></returns>
        public IEnumerable<Cluster> Clustering(IEnumerable<ClusterPoint> points, IEnumerable<ChannelEnum> channels, ClusteringProfile profile = null)
        {
            if (points == null)
            {
                throw new Exception("Точки для кластеризации не заданы.");
            }

            if (points.ElementAt(0).Values.Keys.Count != channels.Count())
            {
                throw new Exception("Количество каналов значений точки не совпадет количеству каналов для кластеризации");
            }

            if (profile == null)
            {
                profile = ClusteringProfile.DefaultProfile();
            }
            SetProfile(profile);

            //Шаг 1 алгоритма
            _z = Init(points, channels);

            for (var i = 1; i <= _i; i++)
            {
                foreach (var cluster in _z)
                {
                    ((List<ClusterPoint>)cluster.Points).Clear();
                }

                //Шаг 2 алгоритма
                foreach (var point in points)
                {
                    var perfectCluster = _z.OrderBy(cluster => EuclideanDistance(point.Values, cluster.CenterCluster)).First();
                    ((List<ClusterPoint>)perfectCluster.Points).Add(point);
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
                    cluster.CenterCluster = new Dictionary<ChannelEnum, float>();
                    foreach (var channel in channels)
                    {
                        cluster.CenterCluster.Add(channel,0f);
                        foreach (var point in cluster.Points)
                        {
                            cluster.CenterCluster[channel] += point.Values[channel];
                        }
                        cluster.CenterCluster[channel] /= cluster.Points.Count();
                    }
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
                    ((List<ClusterPoint>)_z[i].Points).Clear();
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

                    ((List<ClusterPoint>)newCluster.Points).AddRange(cluster.Item1.Points);
                    ((List<ClusterPoint>)newCluster.Points).AddRange(cluster.Item2.Points);

                    _z.Add(newCluster);
                    _z.Remove(cluster.Item1);
                    _z.Remove(cluster.Item2);
                }
            }
        }

        /// <summary>
        /// Рассчитывание Евклидового расстояния между точками
        /// </summary>
        /// <param name="pointA">Значения точки А</param>
        /// <param name="pointB">Значения точки B</param>
        /// <returns></returns>
        private float EuclideanDistance(Dictionary<ChannelEnum, float> pointA, Dictionary<ChannelEnum, float> pointB)
        {
            var keys = pointA.Keys;
            var result = keys.Sum(key => (float) Math.Pow((pointA[key] - pointB[key]), 2));
            return (float)Math.Sqrt(result);
        }

        /// <summary>
        /// Реализация первого шага алгоритма Isodata.
        /// Определение начальных центров кластеров.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private IList<Cluster> Init(IEnumerable<ClusterPoint> points, IEnumerable<ChannelEnum> channels)
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

        /// <summary>
        /// Установка профайла
        /// </summary>
        /// <param name="profile">Профайл</param>
        private void SetProfile(ClusteringProfile profile)
        {
            _clustersCount = profile.СlustersCount == 0 ? 10 : profile.СlustersCount;
            _tettaN = profile.TettaN == 0 ? 1000 : profile.TettaN;
            _tettaS = profile.TettaS == 0 ? 30f : profile.TettaS;
            _tettaC = profile.TettaC == 0 ? 150f : profile.TettaC;
            _l = profile.L == 0 ? 2 : profile.L;
            _i = profile.I == 0 ? 7 : profile.I;
            _coefficient = profile.Coefficient == 0 ? 0.5f : profile.Coefficient;
        }

        /// <summary>
        /// Установка вегетационного индекса кластерам(NDVI) и определение цвета кластера
        /// </summary>
        /// <param name="clusters">Входные кластеры</param>
        public void SetNdviForClusters(IList<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                var operand1 = cluster.CenterCluster[ChannelEnum.Channel5] - cluster.CenterCluster[ChannelEnum.Channel4];
                var operand2 = cluster.CenterCluster[ChannelEnum.Channel5] + cluster.CenterCluster[ChannelEnum.Channel4];

                cluster.Ndvi = operand1 / operand2;
                cluster.ClusterColor = GetColorFromNdvi(cluster.Ndvi);

            }
        }

        private Color GetColorFromNdvi(float ndvi)
        {
            Color color = new Color();

            if (ndvi >= 0.9)
            {
                color = Color.FromArgb(022802);
            }
            else if (ndvi >= 0.8)
            {
                color = Color.DarkGreen;
            }
            else if (ndvi >= 0.7)
            {
                color = Color.Green;
            }
            else if (ndvi >= 0.6)
            {
                color = Color.ForestGreen;
            }
            else if (ndvi >= 0.5)
            {
                color = Color.LimeGreen;
            }
            else if (ndvi >= 0.4)
            {
                color = Color.LawnGreen;
            }
            else if (ndvi >= 0.3)
            {
                color = Color.LawnGreen;
            }
            else if (ndvi >= 0.2)
            {
                color = Color.YellowGreen;
            }
            else if (ndvi >= 0.1)
            {
                color = Color.Tan;
            }
            else if (ndvi >= 0.0)
            {
                color = Color.LightGray;
            }
            else if (ndvi >= -1)
            {
                color = Color.MidnightBlue;
            }
            return color;
        }
    }
}