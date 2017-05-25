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
        /// <param name="channel">Канал</param>
        /// <returns></returns>
        Task<IEnumerable<Point>> ConvertSnapshotAsync(string fileName, ChannelEnum channel);

        /// <summary>
        /// Преобразование списков точек из снимка в точки для кластеризации
        /// </summary>
        /// <param name="points">Списки с данными</param>
        /// <param name="channels">Список каналов</param>
        /// <returns></returns>
        IEnumerable<ClusterPoint> ConvertListsPoints(IEnumerable<IEnumerable<Point>> points, IEnumerable<ChannelEnum> channels);
    }
}
