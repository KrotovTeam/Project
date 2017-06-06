using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using BusinessLogic.Abstraction;
using BusinessLogic.Dtos;
using Common.Constants;

namespace BusinessLogic.Managers
{
    /// <summary>
    /// Менеджер для отображения результатов
    /// </summary>
    public class DrawManager : IDrawManager
    {
        /// <summary>
        /// Отображение динамики
        /// </summary>
        /// <param name="points">Точки</param>
        /// <param name="pathFile">Путь к файлу</param>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        public void DrawDinamics(IList<ResultingPoint> points, string pathFile, int width, int height)
        {
            using (var bitmap = new Bitmap(width, height))
            {
                foreach (var point in points)
                {
                    //var color = point.IsChanged ? Color.Red : GetColorForNdvi(point.Ndvi);
                    //bitmap.SetPixel((int)point.Latitude, (int)point.Longitude, color);
                }

                bitmap.Save(pathFile, ImageFormat.Bmp);
            }
        }

        /// <summary>
        /// Отображение обработанного снимка
        /// </summary>
        /// <param name="points">Точки</param>
        /// <param name="pathFile">Путь к файлу</param>
        /// <param name="width">Ширина картинки</param>
        /// <param name="height">Высота картинки</param>
        public void DrawProcessedSnapshot(IList<ClusterPoint> points, string pathFile, int width, int height)
        {
            using (var bitmap = new Bitmap(width, height))
            {
                foreach (var point in points)
                {
                    var operand1 = point.Values[ChannelEnum.Channel5] - point.Values[ChannelEnum.Channel4];
                    var operand2 = point.Values[ChannelEnum.Channel5] + point.Values[ChannelEnum.Channel4];
                    var ndvi = operand1/operand2;
                    var color = GetColorForNdvi(ndvi);

                    bitmap.SetPixel((int)point.Latitude, (int)point.Longitude, color);
                }

                bitmap.Save(pathFile, ImageFormat.Bmp);
            }
        }

        /// <summary>
        /// Определние цвета точки по значени ndvi
        /// </summary>
        /// <param name="ndvi">Значение NDVI</param>
        /// <returns></returns>
        private Color GetColorForNdvi(double ndvi)
        {
            Color color = new Color();

            if (ndvi >= 0.9)
            {
                color = Color.FromArgb(0x001100);
            }
            else if (ndvi >= 0.8)
            {
                color = Color.FromArgb(0x002000);
            }
            else if (ndvi >= 0.7)
            {
                color = Color.FromArgb(0x003000);
            }
            else if (ndvi >= 0.6)
            {
                color = Color.FromArgb(0x003500);
            }
            else if (ndvi >= 0.5)
            {
                color = Color.FromArgb(0x004000);
            }
            else if (ndvi >= 0.4)
            {
                color = Color.FromArgb(005000);
            }
            else if (ndvi >= 0.3)
            {
                color = Color.FromArgb(0x428c02);
            }
            else if (ndvi >= 0.2)
            {
                color = Color.FromArgb(0x72ba14);
            }
            else if (ndvi >= 0.1)
            {
                color = Color.FromArgb(0x875b28);
            }
            else if (ndvi >= 0.0)
            {
                color = Color.FromArgb(0xe5dcd3);
            }
            else if (ndvi >= -1)
            {
                color = Color.FromArgb(0x030a33);
            }
            return color;
        }
    }
}
