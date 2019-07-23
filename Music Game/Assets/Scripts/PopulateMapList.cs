using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;

namespace Assets.Scripts
{
    public class PopulateMapList : MonoBehaviour
    {
        private List<MapJson> Maps;

        public SongListItem SongListItem;
        private string mapsDir;
        private string mapsDirIn;
        private string mapJsonSubPath;
        private string ResourcePath { get; set; }
        private string AssetDownloadUrl = "https://github.com/acoop133/Rhythm-Unity/releases/download/Asset%2F0.1/MapsZip.zip";
        // Use this for initialization
        private System.Collections.IEnumerator coroutine;
        void Start()
        {
            var platform = Application.platform;
            switch (platform)
            {
                case RuntimePlatform.LinuxEditor:
                case RuntimePlatform.OSXEditor:
                case RuntimePlatform.WindowsEditor:
                    {

                        ResourcePath = Application.streamingAssetsPath + "/GameResources";
                        //if (Directory.Exists(ResourcePath))
                        //{
                        //    Directory.Delete(ResourcePath, true);
                        //}
                        mapsDir = ResourcePath + @"/Maps";
                        mapsDirIn = mapsDir + "/";
                        mapJsonSubPath = @"/Info.json";
                        break;
                    }
                case RuntimePlatform.WindowsPlayer:
                    {
                        ResourcePath = Application.streamingAssetsPath + "/GameResources";
                        mapsDir = ResourcePath + @"\Maps";
                        mapsDirIn = mapsDir + @"\";
                        mapJsonSubPath = @"\Info.json";
                        break;
                    }
                case RuntimePlatform.Android:
                    {
                        ResourcePath = Application.persistentDataPath + "!/assets/GameResources";
                        mapsDir = ResourcePath + "/Maps";
                        mapsDirIn = mapsDir + "/";
                        mapJsonSubPath = "/Info.json";
                        break;
                    }
            }





            if (!Directory.Exists(ResourcePath))
            {
                Directory.CreateDirectory(ResourcePath);

            }



            Populate();

            //
        }
        void Copy(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                if (Path.GetExtension(file) != ".meta")
                    File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));

            foreach (var directory in Directory.GetDirectories(sourceDir))
                Copy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }

        public void Populate()
        {
            Maps = new List<MapJson>();

            if (!Directory.Exists(mapsDir))
            {
                Directory.CreateDirectory(mapsDir);
                //var path = Application.streamingAssetsPath + "/GameResources/Maps";
                //Copy(path, mapsDir);
            }
            var mapPaths = Directory.GetDirectories(mapsDir);
            if (mapPaths.Length == 0)
            {
                System.Net.WebClient client = new System.Net.WebClient();
                DownloadMapAssetFile();


            }


            foreach (var mapPath in mapPaths)
            {
                var json = File.ReadAllText(mapPath + mapJsonSubPath);
                var map = JsonUtility.FromJson<MapJson>(json);
                map.filePath = map.filePath.Insert(0, mapsDirIn);
                Maps.Add(map);

            }
            RefreshDisplay();

        }
        private bool isDownloading = false;
        private string downloadErrorMsg;
        private string progress;
        void DownloadMapAssetFile()
        {
            System.Net.WebClient client = new System.Net.WebClient();
            //client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileCompleted);
            //client.DownloadFileAsync(new Uri(AssetDownloadUrl), $"{ResourcePath}/MapsZip.zip");
            //client.DownloadProgressChanged += Client_DownloadProgressChanged;
            //isDownloading = true;

            //while (isDownloading)
            //{
            //    var progressNow = progress;

            //    if (!isDownloading)
            //    {
            //        break;
            //    }
            //}
            //if (downloadErrorMsg != null)
            //{
            //    Debug.LogError(downloadErrorMsg);
            //}
            //else
            //{
            //    Decompress(new FileInfo($"{mapsDir}/MapsZip.zip"));
            //    File.Delete($"{mapsDir}/MapsZip.zip");
            //}
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(ResourcePath + @"/maps.zip", ResourcePath+@"/Maps", null);
            Debug.Log("Successfully Extracted Maps");
            //client.DownloadFile(new Uri(AssetDownloadUrl), $"{mapsDir}/MapsZip.zip");
            //File.Delete($"{mapsDir}/MapsZip.zip");
        }

        private void Client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            progress = e.ProgressPercentage.ToString();
        }

        void DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            isDownloading = false;
            if (e.Error != null)
                downloadErrorMsg = e.Error.Message;

        }
        public void RefreshDisplay()
        {
            var list = GameObject.Find("SongList").transform;
            foreach (var item in list)
            {
                Destroy(((Transform)item).gameObject);
            }
            foreach (var info in Maps)
            {
                var i = Instantiate(SongListItem, list);
                i.MapJson = info;
                i.UpdateText();
            }
        }
       

    }
    public class MapJson
    {
        public int id;
        public string title;
        public string artist;
        public string mapCreator;
        public int complexity;
        public string breakPeriod;
        public Difficulty difficulty;
        public string status;
        public int bpm;
        public string filePath;
        public List<string> map;
        public int previewTime;

        public class Difficulty
        {
            public int hpDrainRate;
            public int circleSize;
            public int overalDifficulty;
            public int approachRate;
            public int sliderMultiplier;
            public int sliderTickRate;
        }
    }


}
