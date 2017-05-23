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
        /// Преобразование точек из снимка в данные для кластеризации
        /// </summary>
        /// <param name="dataList">Список с данными</param>
        /// <param name="channels">Каналы</param>
        /// <returns></returns>
        IEnumerable<RawData> ConvertPointsToRawData(IEnumerable<IEnumerable<Point>> dataList, IEnumerable<ChannelEnum> channels);
    }
}
