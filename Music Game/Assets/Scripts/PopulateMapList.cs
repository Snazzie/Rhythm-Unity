using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class PopulateMapList : MonoBehaviour
    {
        private List<MapJson> Maps;

        public SongListItem SongListItem;

        private string ResourcePath { get; set; }
        // Use this for initialization
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

                        break;
                    }
                case RuntimePlatform.WindowsPlayer:
                    {
                        ResourcePath = Application.streamingAssetsPath + "/GameResources";
                        break;
                    }
                case RuntimePlatform.Android:
                    {
                        ResourcePath = "jar:file://" + Application.dataPath + "!/assets/GameResources/";

                        break;
                    }
            }






            //Directory.CreateDirectory(ResourcePath);
            //var path = Application.ass + "/GameResources";
            //Copy(path, ResourcePath);


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

            var targetDirectory = ResourcePath + @"\Maps";

            var mapPaths = Directory.GetDirectories(targetDirectory);
            foreach (var mapPath in mapPaths)
            {
                var json = File.ReadAllText(mapPath + @"\Info.json");
                var map = JsonUtility.FromJson<MapJson>(json);
                map.filePath = map.filePath.Insert(0, targetDirectory + @"\");
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
