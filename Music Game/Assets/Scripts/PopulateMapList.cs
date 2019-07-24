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

        // Use this for initialization
        private System.Collections.IEnumerator coroutine;
        void Start()
        {




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

        public async void Populate()
        {
            var gameLaunchParams = GameLaunchSetup.Instance;
            Maps = new List<MapJson>();

            if (!Directory.Exists(gameLaunchParams.mapsDir))
            {
                Directory.CreateDirectory(gameLaunchParams.mapsDir);
                //var path = Application.streamingAssetsPath + "/GameResources/Maps";
                //Copy(path, mapsDir);
            }
            var mapPaths = Directory.GetDirectories(gameLaunchParams.mapsDir);
            if (mapPaths.Length == 0)
            {
                if (!File.Exists(gameLaunchParams.ResourcePath + @"/MapsZip.zip"))
                    Utilities.AssetHelper.DownloadMapAssetFile(gameLaunchParams.ResourcePath);
                else
                {
                    Utilities.AssetHelper.ExtractZip(gameLaunchParams.ResourcePath + @"/MapsZip.zip", gameLaunchParams.ResourcePath);
                }

            }
            mapPaths = Directory.GetDirectories(gameLaunchParams.mapsDir);

            foreach (var mapPath in mapPaths)
            {
                var json = File.ReadAllText(mapPath + gameLaunchParams.mapJsonSubPath);
                var map = JsonUtility.FromJson<MapJson>(json);
                map.filePath = map.filePath.Insert(0, gameLaunchParams.mapsDirIn);
                Maps.Add(map);

            }
            RefreshDisplay();

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
