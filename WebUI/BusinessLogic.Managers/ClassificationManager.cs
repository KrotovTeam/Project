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
        private int _clustersCount;

        /// <summary>
        /// Параметр, с которым сравнивается количество выборочных образов, вошедших в кластер
        /// </summary>
        private int _tettaN;

        /// <summary>
        /// Параметр, характеризующий среднеквадратическое отклонение
        /// </summary>
        private double _tettaS;

        /// <summary>
        /// Параметр, характеризующий компактность
        /// </summary>
        private double _tettaC;

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
        private readonly IList<double> _dj = new List<double>();

        /// <summary>
        /// Обобщенное среднее расстояние между объектами, находящимися в отдельных кластерах, и соответствующими
        /// центрами кластеров
        /// </summary>
        private double _d;

        /// <summary>
        /// Вектор среднеквадратичного отколнения
        /// </summary>
        private readonly IList<Dictionary<CoordinateSystemEnum, double>> _sigmaj = new List<Dictionary<CoordinateSystemEnum, double>>();

        /// <summary>
        /// Максимальная компонента в векторе среднеквадратичного отклонения
        /// </summary>
        private readonly IList<Tuple<CoordinateSystemEnum, double>> _sigmajMax = new List<Tuple<CoordinateSystemEnum, double>>();

        /// <summary>
        /// Коэффициент при высчитывании gammaj
        /// </summary>
        private double _coefficient;

        /// <summary>
        /// Расстояния между всеми парами кластеров
        /// </summary>
        private readonly IList<Tuple<Cluster, Cluster, double>> _dij = new List<Tuple<Cluster, Cluster, double>>();

        #endregion

        #region Clustering

        /// <summary>
        /// Кластеризация данных методом Isodata
        /// </summary>
        /// <param name="points">Входные данные</param>
        /// <param name="keys">Ключи по которым происходит классификация</param>
        /// <param name="profile">Профайл с параметрами кластеризации</param>
        /// <returns></returns>
        public IEnumerable<Cluster> Clustering(IEnumerable<ResultingPoint> points, IEnumerable<CoordinateSystemEnum> keys, ClusteringProfile profile = null)
        {
            if (points == null)
            {
                throw new Exception("Точки для кластеризации не заданы.");
            }

            if (points.ElementAt(0).Values.Keys.Count != keys.Count())
            {
                throw new Exception("Количество каналов значений точки не совпадет количеству каналов для кластеризации");
            }

            if (profile == null)
            {
                profile = ClusteringProfile.DefaultProfile();
            }
            SetProfile(profile);

            //Шаг 1 алгоритма
            _z = Init(points, keys);

            for (var i = 1; i <= _i; i++)
            {
                foreach (var cluster in _z)
                {
                    ((List<ResultingPoint>)cluster.Points).Clear();
                }

                //Шаг 2 алгоритма
                foreach (var point in points)
                {
                    var perfectCluster = _z.OrderBy(cluster => EuclideanDistance(point.Values, cluster.CenterCluster)).First();
                    ((List<ResultingPoint>)perfectCluster.Points).Add(point);
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
                    cluster.CenterCluster = new Dictionary<CoordinateSystemEnum, int>();
                    foreach (var channel in keys)
                    {
                        cluster.CenterCluster.Add(channel,0);
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
                    if (Step8(keys))
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
        private bool Step8(IEnumerable<CoordinateSystemEnum> keys)
        {
            _sigmaj.Clear();
            _sigmajMax.Clear();

            //Шаг 8
            foreach (var cluster in _z)
            {
                var dictionary = new Dictionary<CoordinateSystemEnum, double>();
                foreach (var key in keys)
                {
                    var value = cluster.Points.Sum(point => Math.Pow((point.Values[key] - cluster.CenterCluster[key]), 2));
                    value = Math.Sqrt(value/cluster.Points.Count());
                    dictionary.Add(key,value);
                }
                _sigmaj.Add(dictionary);
            }

            //Шаг 9
            for (var i = 0; i < _z.Count; i++)
            {
                var max = double.MinValue;
                var channel = CoordinateSystemEnum.Unknown;
                foreach (var key in _sigmaj.ElementAt(i).Keys)
                {
                    if (_sigmaj.ElementAt(i)[key] > max)
                    {
                        max = _sigmaj.ElementAt(i)[key];
                        channel = key;
                    }
                }
                _sigmajMax.Add(new Tuple<CoordinateSystemEnum, double>(channel, max));
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
                    ((List<ResultingPoint>)_z[i].Points).Clear();
                    _z[i].CenterCluster[_sigmajMax[i].Item1] = Convert.ToInt32(_z[i].CenterCluster[_sigmajMax[i].Item1] + gammaj);

                    //zj-
                    newCluster.CenterCluster[_sigmajMax[i].Item1] = Convert.ToInt32(newCluster.CenterCluster[_sigmajMax[i].Item1] - gammaj);
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
                var tuple = new Tuple<Cluster, Cluster, double>(_z[i], _z[i + 1], EuclideanDistance(_z[i].CenterCluster, _z[i + 1].CenterCluster));
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

                    ((List<ResultingPoint>)newCluster.Points).AddRange(cluster.Item1.Points);
                    ((List<ResultingPoint>)newCluster.Points).AddRange(cluster.Item2.Points);

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
        private double EuclideanDistance(Dictionary<CoordinateSystemEnum, int> pointA, Dictionary<CoordinateSystemEnum, int> pointB)
        {
            var keys = pointA.Keys;
            var result = keys.Sum(key => Math.Pow((pointA[key] - pointB[key]), 2));
            return Math.Sqrt(result);
        }

        /// <summary>
        /// Реализация первого шага алгоритма Isodata.
        /// Определение начальных центров кластеров.
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        private IList<Cluster> Init(IEnumerable<ResultingPoint> points, IEnumerable<CoordinateSystemEnum> keys)
        {
            var step = points.Count()/_clustersCount;
            var result = new List<Cluster>();
            for (var i = step; i < points.Count(); i += step)
            {
                var dictinary = new Dictionary<CoordinateSystemEnum, int>();
                for (var j = 0; j < keys.Count(); j++)
                {
                    var key = keys.ElementAt(j);
                    dictinary.Add(key, points.ElementAt(j).Values[key]);
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

        #endregion

        /// <summary>
        /// Метод определяет изменения значения вегетационного индекса на снимке c прошлого по текущий год
        /// </summary>
        /// <param name="lastYearClusters"></param>
        /// <param name="currentYearClusters"></param>
        /// <returns></returns>
        public IList<ResultingPoint> Compare(IEnumerable<Cluster> lastYearClusters, IEnumerable<Cluster> currentYearClusters)
        {
            //List<ClusterPoint> currentYearPoints = new List<ClusterPoint>();
            //List<ClusterPoint> lastYearPoints = new List<ClusterPoint>();
            //foreach (var cluster in lastYearClusters)
            //{
            //    lastYearPoints.AddRange((List<ClusterPoint>)(cluster.Points).GroupBy(point => new {point.Latitude, point.Longitude}));
            //}
            //foreach (var cluster in currentYearClusters)
            //{
            //    currentYearPoints.AddRange((List<ClusterPoint>) (cluster.Points).GroupBy(point => new {point.Latitude, point.Longitude}));
            //}


            //IList<ResultingPoint> resultingPoints = new List<ResultingPoint>();
            //for (var i = 0; i < currentYearPoints.Count(); i++)
            //{
            //    var operand1 = currentYearPoints[i].Values[ChannelEnum.Channel5] - currentYearPoints[i].Values[ChannelEnum.Channel4];
            //    var operand2 = currentYearPoints[i].Values[ChannelEnum.Channel5] + currentYearPoints[i].Values[ChannelEnum.Channel4];

            //    var ndviForCurrentYearPoint = operand1 / operand2;

            //    var operand3 = lastYearPoints[i].Values[ChannelEnum.Channel5] - lastYearPoints[i].Values[ChannelEnum.Channel4];
            //    var operand4 = lastYearPoints[i].Values[ChannelEnum.Channel5] + lastYearPoints[i].Values[ChannelEnum.Channel4];

            //    var ndviForLastYearPoint = operand3 / operand4;

            //    var ndviChanging = Math.Abs(ndviForLastYearPoint - ndviForCurrentYearPoint) * 100.0;
            //    var isChangeExist = ndviChanging >= 30;
            //    var resultingPoint = new ResultingPoint
            //    {
            //        Latitude = currentYearPoints[i].Latitude,
            //        Longitude = currentYearPoints[i].Longitude,
            //        Ndvi = ndviForCurrentYearPoint,
            //        IsChanged = isChangeExist
            //    };
            //    resultingPoints.Add(resultingPoint);
            //}
            //return resultingPoints;
            return null;
        }

        /// <summary>
        /// Определение динамики по снимкам
        /// </summary>
        /// <param name="lastYearPoints">Точки для прошлого года</param>
        /// <param name="currentYearPoints">Актуальные точки</param>
        /// <returns></returns>
        public IList<ResultingPoint> DeterminationDinamics(IList<ClusterPoint> lastYearPoints, IList<ClusterPoint> currentYearPoints)
        {
            IList<ResultingPoint> resultingPoints = new List<ResultingPoint>();
            for (var i = 0; i < currentYearPoints.Count; i++)
            {
                var operand1 = currentYearPoints[i].Values[ChannelEnum.Channel5] - currentYearPoints[i].Values[ChannelEnum.Channel4];
                var operand2 = currentYearPoints[i].Values[ChannelEnum.Channel5] + currentYearPoints[i].Values[ChannelEnum.Channel4];

                var ndviForCurrentYearPoint = operand1 / operand2;

                var operand3 = lastYearPoints[i].Values[ChannelEnum.Channel5] - lastYearPoints[i].Values[ChannelEnum.Channel4];
                var operand4 = lastYearPoints[i].Values[ChannelEnum.Channel5] + lastYearPoints[i].Values[ChannelEnum.Channel4];

                var ndviForLastYearPoint = operand3 / operand4;

                var ndviChanging = Math.Abs(ndviForLastYearPoint - ndviForCurrentYearPoint) * 100.0;
                if (ndviChanging >= 30)
                {
                    resultingPoints.Add(new ResultingPoint
                    {
                        Ndvi = ndviForCurrentYearPoint,
                        Values = new Dictionary<CoordinateSystemEnum, int>
                        {
                            [CoordinateSystemEnum.Latitude] = Convert.ToInt32(currentYearPoints[i].Latitude),
                            [CoordinateSystemEnum.Longitude] = Convert.ToInt32(currentYearPoints[i].Longitude)
                        }
                    });
                }
            }
            return resultingPoints;
        }

        public IList<ResultingPoint> GetBoundaryPoints(IList<Cluster> clusters, int width, int height)
        {
            var result = new List<ResultingPoint>();
            for (var i = 0; i < width; i++)
            {
                foreach (var cluster in clusters)
                {
                    var overridingPoints = cluster.Points.Where(p => p.Values[CoordinateSystemEnum.Latitude] == i).ToList();
                    if (overridingPoints.Count() != 0)
                    {
                        var max = overridingPoints.Max(p => p.Values[CoordinateSystemEnum.Longitude]);
                        var min = overridingPoints.Min(p => p.Values[CoordinateSystemEnum.Longitude]);
                        if (max != min)
                        {
                            result.Add(overridingPoints.First(k => k.Values[CoordinateSystemEnum.Longitude] == max));
                            result.Add(overridingPoints.First(k => k.Values[CoordinateSystemEnum.Longitude] == min));
                        }
                        else
                        {
                            result.Add(overridingPoints.First(k => k.Values[CoordinateSystemEnum.Longitude] == max));
                        }
                    }
                }
            }
            for (var i = 0; i < height; i++)
            {
                foreach (var cluster in clusters)
                {
                    var overridingPoints = cluster.Points.Where(p => p.Values[CoordinateSystemEnum.Longitude] == i);
                    if (overridingPoints.Count() != 0)
                    {
                        var max = overridingPoints.Max(p => p.Values[CoordinateSystemEnum.Latitude]);
                        var min = overridingPoints.Min(p => p.Values[CoordinateSystemEnum.Latitude]);
                        if (max != min)
                        {
                            result.Add(overridingPoints.First(k => k.Values[CoordinateSystemEnum.Latitude] == max));
                            result.Add(overridingPoints.First(k => k.Values[CoordinateSystemEnum.Latitude] == min));
                        }
                        else
                        {
                            result.Add(overridingPoints.First(k => k.Values[CoordinateSystemEnum.Latitude] == max));
                        }
                    }
                }
            }
            return result;
        }
    }
}