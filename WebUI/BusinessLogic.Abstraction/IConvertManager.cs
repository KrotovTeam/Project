using System.Collections.Generic;
using System.Threading.Tasks;
using BusinessLogic.Dtos;
using Common.Constants;

namespace BusinessLogic.Abstraction
{
    public interface IConvertManager
    {
        /// <summary>
        /// Асинхронное преобразование снимка в точки
        /// </summary>
        /// <param name="fileName">Путь к файлу</param>
        /// <returns></returns>
        Task<IList<Point>> ConvertSnapshotAsync(string fileName);

        /// <summary>
        /// Преобразование списков точек из снимка в точки для кластеризации
        /// </summary>
        /// <param name="points">Списки с данными</param>
        /// <param name="channels">Список каналов</param>
        /// <returns></returns>
        IList<ClusterPoint> ConvertListsPoints(IList<IList<Point>> points, IList<ChannelEnum> channels);
    }
}
