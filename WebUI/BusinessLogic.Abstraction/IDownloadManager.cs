using System;
using System.Collections.Generic;
using BusinessLogic.Dtos;
using Common.Constants;

namespace BusinessLogic.Abstraction
{
    /// <summary>
    /// Менеджер для скачивания снимков
    /// </summary>
    public interface IDownloadManager
    {
        /// <summary>
        /// Скачивание снимков
        /// </summary>
        /// <param name="point1">Точка 1</param>
        /// <param name="point2">Точка 2</param>
        /// <param name="point3">Точка 3</param>
        /// <param name="point4">Точка 4</param>
        /// <param name="date">Дата</param>
        /// <param name="channels">Необходимые каналы</param>
        /// <returns>Пути к сохраненным снимкам</returns>
        IList<Snapshot> DownloadSnapshots(GeographicalPoint point1, GeographicalPoint point2, GeographicalPoint point3,
            GeographicalPoint point4, DateTime date, IList<ChannelEnum> channels);
    }
}
