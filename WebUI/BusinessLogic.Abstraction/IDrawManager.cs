using System.Collections.Generic;
using BusinessLogic.Dtos;

namespace BusinessLogic.Abstraction
{
    /// <summary>
    /// Менеджер для отображения результатов
    /// </summary>
    public interface IDrawManager
    {
        /// <summary>
        /// Отображение динамики
        /// </summary>
        /// <param name="clusterPoints"></param>
        /// <param name="boundaryPoints"></param>
        /// <param name="pathFile">Путь к файлу</param>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        void DrawDinamics(IList<ResultingPoint> boundaryPoints, IList<ClusterPoint> clusterPoints, string pathFile, int width, int height);

        /// <summary>
        /// Отображение обработанного снимка
        /// </summary>
        /// <param name="points">Точки</param>
        /// <param name="pathFile">Путь к файлу</param>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        void DrawProcessedSnapshot(IList<ClusterPoint> points, string pathFile,int width, int height);
    }
}
