namespace BusinessLogic.Dtos
{
    public class ClusteringProfile
    {
        /// <summary>
        /// Необходимое число кластеров
        /// </summary>
        public int СlustersCount { get; set; }

        /// <summary>
        /// Параметр, с которым сравнивается количество выборочных образов, вошедших в кластер
        /// </summary>
        public int TettaN { get; set; }

        /// <summary>
        /// Параметр, характеризующий среднеквадратическое отклонение
        /// </summary>
        public double TettaS { get; set; }

        /// <summary>
        /// Параметр, характеризующий компактность
        /// </summary>
        public double TettaC { get; set; }

        /// <summary>
        /// Максимальное количество пар центров кластеров, которые можно объединить
        /// </summary>
        public int L { get; set; }

        /// <summary>
        /// Допустимое число циклов итерации
        /// </summary>
        public int I { get; set; }

        /// <summary>
        /// Коэффициент при высчитывании gammaj
        /// </summary>
        public double Coefficient { get; set; }

        /// <summary>
        /// Профайл со стандартными настройками
        /// </summary>
        /// <returns></returns>
        public static ClusteringProfile DefaultProfile()
        {
            return new ClusteringProfile
            {
                СlustersCount = 10,
                TettaN = 1000,
                TettaS = 30,
                TettaC = 150,
                L = 2,
                I = 7
            };
        }
    }
}
