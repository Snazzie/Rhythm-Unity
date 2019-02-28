using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    public class ImportOsuMaps : MonoBehaviour
    {
        private string osuMapFolder;
        private string gameMapFolder;
        private string[] osuMapFolderPaths;
        // Use this for initialization

        // Currently assumes its osu file only
        public bool Import()
        {
            osuMapFolder = @"E:\osu!\Songs";
            var gameExeFolder = Directory.GetCurrentDirectory();
            var suggestGameMapFolder = gameExeFolder + @"\Maps";
            if (!Directory.Exists(suggestGameMapFolder))
                Directory.CreateDirectory(suggestGameMapFolder);

            gameMapFolder = suggestGameMapFolder;
            osuMapFolderPaths = Directory.GetDirectories(osuMapFolder);

            foreach (var folderPath in osuMapFolderPaths)
            {
                var osuMapBundle = new OsuFormat()
                {
                    Title = Path.GetDirectoryName(folderPath),
                    Path = folderPath
                };
                foreach (var filePath in Directory.GetFiles(folderPath))
                {
                    var extension = Path.GetExtension(filePath);
                    if (extension == "mp3")
                    {
                        osuMapBundle.AudioPath = Path.GetFileName(filePath);
                    }
                    else if (extension == "jpg")
                        osuMapBundle.ImagePath = filePath;
                    else if (extension == "osu")
                    {
                        var jsonFile = ConvertOsuToJSON(filePath);
                    }


                }
            }

            return true;
        }

        private string ConvertOsuToJSON(string path)
        {
            var lines = File.ReadAllLines(path);
            string newJson;
            string currentClass = null;
            bool classEnd = true;
            var fields = new OSUFakeExtensionValuesWeCareAbout();

            foreach (var line in lines)
            {

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentClass = line.Trim('[', ']');

                }
                else
                {
                    if (currentClass == "General")
                    {
                        fields.General.Add(line);
                    }
                }
            }
            var json = "";
            return json;
        }

        // Update is called once per frame

        public class OSUFakeExtensionValuesWeCareAbout
        {
            public List<string> General;
            public List<string> MetaData;
            public List<string> Difficulty;
            public List<string> TimingPoints;
            public List<string> HitObjects;

        }

        public class Format
        {
            public string Title { get; set; }
            public string Artist { get; set; }
            public string MapCreator { get; set; }
            public string Directory { get; set; }
            public string Path { get; set; }
            public string AudioPath { get; set; }
            public string UploadDate { get; set; }
            public string ImagePath { get; set; }

        }
        class OsuFormat : Format
        {
            public string[] OsuData { get; set; }
            public string[] OsuTaikoData { get; set; }
        }


    }
}
