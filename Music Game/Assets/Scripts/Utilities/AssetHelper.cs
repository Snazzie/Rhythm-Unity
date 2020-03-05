using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utilities
{
    public class AssetHelper : MonoBehaviour
    {
        private static string AssetDownloadUrl = "https://github.com/Snazzie/Rhythm-Unity/releases/download/Asset%2F0.1/MapsZip.zip";


        public void DownloadAndExtractAsset()
        {
            var gameLaunchParams = GameLaunchSetup.Instance;
            File.Delete(gameLaunchParams.ResourcePath + @"/MapsZip.zip");
            Directory.Delete(gameLaunchParams.ResourcePath + @"/Maps", true);
            DownloadMapAssetFile("");
            ExtractZip(gameLaunchParams.ResourcePath + @"/MapsZip.zip", gameLaunchParams.ResourcePath);
        }

        public static Task DownloadMapAssetFile(string destination)
        {
            var launchSetup = GameLaunchSetup.Instance;
            System.Net.WebClient client = new System.Net.WebClient();

            client.DownloadFileCompleted += Client_DownloadFileCompleted;
            client.DownloadProgressChanged += Client_DownloadProgressChanged;
            client.DownloadFileAsync(new Uri(AssetDownloadUrl), $"{launchSetup.ResourcePath}/MapsZip.zip");

            //File.Delete($"{launchSetup.mapsDir}/MapsZip.zip");
            return Task.CompletedTask;
        }

        private static void Client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            GameObject.Find("ProgressSlider").GetComponent<Slider>().value = e.ProgressPercentage;
        }

        private static void Client_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                GameObject.Find("Important").GetComponent<PopulateMapList>().Populate();
            }
        }

        public static void ExtractZip(string source, string destination)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(source,destination, null);
            Debug.Log("Successfully Extracted Maps");
        }

        public static void DeleteFile(string source)
        {
            File.Delete(source);
        }
    }
}
