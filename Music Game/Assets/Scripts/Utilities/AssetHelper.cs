using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class AssetHelper
    {

        public static void DownloadMapAssetFile(string destination)
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
            
            //client.DownloadFile(new Uri(AssetDownloadUrl), $"{mapsDir}/MapsZip.zip");
            //File.Delete($"{mapsDir}/MapsZip.zip");
        }

        public static void ExtractZip(string source, string destination)
        {
            FastZip fastZip = new FastZip();
            fastZip.ExtractZip(source,destination, null);
            Debug.Log("Successfully Extracted Maps");
        }
    }
}
