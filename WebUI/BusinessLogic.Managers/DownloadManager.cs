using System;
using System.Collections.Generic;
using BusinessLogic.Abstraction;
using BusinessLogic.Dtos;
using Common.Constants;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
//using ICSharpCode.SharpZipLib;

namespace BusinessLogic.Managers
{
    /// <summary>
    /// Менеджер для скачивания снимков
    /// </summary>
    public class DownloadManager : IDownloadManager
    {
        #region Properties
        private string apiKey;
        private RestClient Client = new RestClient("https://earthexplorer.usgs.gov/inventory/json/");
        private string dataSetName = "LANDSAT_8_C1";
        private string downloadFolder;
        #endregion

        #region Methods
        /// <summary>
        /// Скачивание снимков
        /// </summary>
        /// <param name="point1">Точка 1</param>
        /// <param name="point2">Точка 2</param>
        /// <param name="point3">Точка 3</param>
        /// <param name="point4">Точка 4</param>
        /// <param name="fromDate">От дата</param>
        /// <param name="toDate">До дата</param>" 
        /// <returns>Пути к сохраненным снимкам</returns>
        public IList<Snapshot> DownloadSnapshots(GeographicalPoint point1, GeographicalPoint point2, GeographicalPoint point3,
            GeographicalPoint point4, DateTime fromDate, DateTime toDate)
        {
            var scenes = FindAvailableScenes(point1.Latitude, point1.Longitude, point2.Latitude, point2.Longitude, fromDate, toDate);
            List<Snapshot> snapshots = new List<Snapshot>();
            foreach (var scene in scenes.data.results)
            {
                var downloadList = DownloadSearch(scene.entityId);

                foreach (var optionList in downloadList.data)
                {

                    foreach (var option in optionList.downloadOptions)
                    {

                        if (Convert.ToBoolean(option.available))
                        {
                            ApiDownloadResponse download = Download(scene.entityId);
                            snapshots.Add(new Snapshot { Path = download.data[0] });
                        }

                    }
                }
            }
            
            return snapshots;
        }



        /// <summary>
        /// Метод подключения к earthexplorer
        /// </summary>
        /// <param name="login">Login</param>
        /// <param name="password">Password</param>
        private void Connect(string login = "EvgnEvgn", string password = "ifhbgjd654321")
        {
            //Формируем запрос json
            var request = new RestRequest("login?jsonRequest={" +
                            @"""username"":""" + login + @"""," +
                            @"""password"":""" + password + @"""}",
                            Method.POST);
            //Выполняем запрос, получаем ответ
            IRestResponse apiResponse = Client.Execute(request);
            //Десириализуем ответ из json
            var response = JsonConvert.DeserializeObject<ApiConnectionResponse>(apiResponse.Content);
            //Сохраняем apiKey, он нужен в будущем
            apiKey = response.data;
        }
        /// <summary>
        /// Отключение от сервера
        /// </summary>
        private void Disconnect()
        {
            //Формируем запрос
            var request = new RestRequest("logout?jsonRequest={" +
                           @"""apiKey"":""" + apiKey + @"""}",
                           Method.GET);

            // Выполняем запрос, обнуляем ключ
            IRestResponse apiResponse = Client.Execute(request);

            //var response = JsonConvert.DeserializeObject<ApiConnectionResponse>(apiResponse.Content);
        }

        /// <summary>
        /// Поиск подходящих снимков
        /// </summary>
        /// <param name="lowerLeftLatitude">Широта левого нижнего</param>
        /// <param name="lowerLeftLongitude">Долгота левого нижнего</param>
        /// <param name="upperRightLatitude">Широта правого верхнего</param>
        /// <param name="upperRightLongitude">Долгота правого верхнего</param>
        /// <param name="fromDate">От даты</param>
        /// <param name="toDate">До даты</param>
        /// <returns></returns>
        private ApiSceneSearchResponse FindAvailableScenes(double lowerLeftLatitude, double lowerLeftLongitude,
                                 double upperRightLatitude, double upperRightLongitude,
                                 DateTime fromDate, DateTime toDate)
       {
            lowerLeftLatitude = 48.798188297009958;
            upperRightLatitude = 48.999191302021046;
            lowerLeftLongitude = 2.1670538454397894;
            upperRightLongitude = 2.5900274796983065;
            fromDate = toDate = DateTime.Today;

            var request = new RestRequest("search?jsonRequest={" +
                            @"""datasetName"":""" + dataSetName + @"""," +
                            @"""lowerLeft"":{ ""latitude"":""" + lowerLeftLatitude + @""",""longitude"":""" + lowerLeftLongitude + @"""}," +
                            @"""upperRight"":{ ""latitude"":""" +  upperRightLatitude + @""",""longitude"":""" + upperRightLongitude + @"""}," +
                            @"""startDate"":""" + fromDate.ToString("yyyy-MM-dd") + @"""," +
                            @"""endDate"":""" + toDate.ToString("yyyy-MM-dd") + @"""," +
                            @"""includeUnknownCloudCover"":true," +
                            @"""maxResults"":""9999""," +
                            @"""sortOrder"":""ASC""," +
                            @"""apiKey"":""" + apiKey + @"""," +
                            @"""node"":""" + "EE" + @"""" +
                            "}",
                            Method.GET);

            // Execute the request and get Scenes
            IRestResponse apiResponse = Client.Execute(request);
            return JsonConvert.DeserializeObject<ApiSceneSearchResponse>(apiResponse.Content);
        }
        /// <summary>
        /// Необходимо проверить, возможно ли скачать снимок
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        private ApiDownloadOptionsResponse DownloadSearch(string entityId = "LT81372072017001LGN00")
        {
            var request = new RestRequest("downloadoptions?jsonRequest={" +
                            @"""datasetName"":""" + dataSetName + @"""," +
                            @"""entityIds"":[""" + entityId + @"""]," +
                            @"""machineOnly"":true," +
                            @"""apiKey"":""" + apiKey + @"""," +
                            @"""node"":""" + "EE" + @"""" +
                            "}",
                            Method.GET);

            
            IRestResponse apiResponse = Client.Execute(request);
            return JsonConvert.DeserializeObject<ApiDownloadOptionsResponse>(apiResponse.Content);
        }
        /// <summary>
        /// Метод получения ссылки на скачивание
        /// </summary>
        /// <param name="EntityName">Номер снимка</param>
        /// <returns></returns>
        private ApiDownloadResponse Download(string EntityName)
        {

            var request = new RestRequest("download?jsonRequest={" +
                            @"""datasetName"":""" + dataSetName + @"""," +
                            @"""products"":[""" + "STANDARD" + @"""]," +
                            @"""entityIds"":[""" + EntityName + @"""]," +
                            @"""apiKey"":""" + apiKey + @"""," +
                            @"""node"":""" + "EE" + @"""" +
                            "}",
                            Method.GET);

            IRestResponse apiResponse = Client.Execute(request);
            return JsonConvert.DeserializeObject<ApiDownloadResponse>(apiResponse.Content);
        }

        //private void UnpackDownload(string erosEntityName, string downloadedFile)
        //{
        //    FileInfo tarFileInfo = new FileInfo(downloadedFile);
        //    DirectoryInfo targetDirectory = new DirectoryInfo(downloadFolder + erosEntityName);
        //    if (!targetDirectory.Exists)
        //    { targetDirectory.Create(); }
        //    using (Stream sourceStream = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(tarFileInfo.OpenRead()))
        //    {
        //        using (ICSharpCode.SharpZipLib.Tar.TarArchive tarArchive = ICSharpCode.SharpZipLib.Tar.TarArchive.CreateInputTarArchive(sourceStream,
        //            ICSharpCode.SharpZipLib.Tar.TarBuffer.DefaultBlockFactor))
        //        {
        //            tarArchive.ExtractContents(targetDirectory.FullName);
        //        }
        //    }
        //}
        #endregion
    }

    public class ApiConnectionResponse
    {
        public string errorCode { get; set; }
        public string data { get; set; }
        public string api_version { get; set; }
        public string executionTime { get; set; }
    }
    public class ApiSceneSearchResponse
    {
        public string errorCode { get; set; }
        public string error { get; set; }
        public ApiSearchResponseMetadata data { get; set; }            
        public string api_version { get; set; }
        public string executionTime { get; set; }
    }
    public class ApiSearchResponseMetadata
    {
        public string numberReturned { get; set; }
        public string totalHits { get; set; }
        public string firstRecord { get; set; }
        public string lastRecord { get; set; }
        public string nextRecord { get; set; }
        public List<ApiSearchResponseResults> results { get; set; }     
    }
    public class ApiSearchResponseResults
    {
        public string acquisitionDate { get; set; }
        public string startTime { get; set; }
        public string endTime { get; set; }
        public ApiCoordinates lowerLeftCoordinate { get; set; }
        public ApiCoordinates upperLeftCoordinate { get; set; }
        public ApiCoordinates upperRightCoordinate { get; set; }
        public ApiCoordinates lowerRightCoordinate { get; set; }
        public string sceneBounds { get; set; }
        public string browseUrl { get; set; }
        public string dataAccessUrl { get; set; }
        public string downloadUrl { get; set; }
        public string entityId { get; set; }
        public string displayId { get; set; }
        public string metadataUrl { get; set; }
        public string fgdcMetadataUrl { get; set; }
        public string modifiedDate { get; set; }
        public string orderUrl { get; set; }
        public string bulkOrdered { get; set; }
        public string ordered { get; set; }
        public string summary { get; set; }
    }
    public class ApiCoordinates
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
    }
    public class ApiDownloadOptionsResponse
    {
        public string errorCode { get; set; }
        public string error { get; set; }
        public List<ApiDownloadOptionsResponseMetadata> data { get; set; }
        public string api_version { get; set; }
        public string executionTime { get; set; }
    }
    public class ApiDownloadOptionsResponseMetadata
    {
        public List<ApiDownloadOptionsResponseResults> downloadOptions { get; set; }
        public string entityId { get; set; }
    }
    public class ApiDownloadOptionsResponseResults
    {
        public string available { get; set; }
        public string downloadCode { get; set; }
        public string filesize { get; set; }
        public string productName { get; set; }
        public string url { get; set; }
        public string storageLocation { get; set; }
    }
    public class ApiDownloadResponse
    {
        public string errorCode { get; set; }
        public string error { get; set; }
        public List<string> data { get; set; }
        public string api_version { get; set; }
        public string executionTime { get; set; }
    }
}
