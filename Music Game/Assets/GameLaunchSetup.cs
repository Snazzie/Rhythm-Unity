using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets
{
    public class GameLaunchSetup
    {
        public string mapsDir { get; private set; }
        public string mapsDirIn { get; private set; }
        public string mapJsonSubPath { get; private set; }
        public string ResourcePath { get; private set; }
        public RuntimePlatform platform { get; private set; }
        // Use this for initialization

        private static GameLaunchSetup instance = null;
        public static GameLaunchSetup Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameLaunchSetup();
                }
                return instance;
            }
        }

        private GameLaunchSetup()
        {
            Application.targetFrameRate = 300;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            platform = Application.platform;
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

            Debug.Log(SceneManager.GetActiveScene().name);
        }


    }
}
