using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HandleSelections : MonoBehaviour
    {
        public SongListItem Selected { get; set; } 
        private AudioSource AudioSource { get; set; }
        private AudioClip audioclip;
        // Use this for initialization
        void Start()
        {
            // get list
            // default to first
            AudioSource = GameObject.Find("AudioSource").transform.GetComponent<AudioSource>();

        }

        public void UpdateMapInfoPanel()
        {
            
            StartCoroutine(GetClip(Selected.MapJson.filePath + @"/Audio.wav"));
            StartCoroutine(SetUI());
            
        }
        IEnumerator GetClip(string path)
        {
            
            if(GameLaunchSetup.Instance.platform == RuntimePlatform.Android)
            {
                path = path.Insert(0, "file:///");
            }
            Debug.Log(GameLaunchSetup.Instance.platform.ToString());
            Debug.Log(path);
            using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV))
            {
                yield return www.SendWebRequest();
                if (www.isNetworkError)
                {
                    Debug.LogError(www.error);
                }

                Debug.Log("Clip Load finished");
                audioclip = DownloadHandlerAudioClip.GetContent(www);
            }
            

        }
        IEnumerator SetUI()
        {

            while(audioclip == null)
            {
                yield return null;
            }
            var panel = transform.GetChild(0).transform;
            panel.GetChild(0).GetComponent<Text>().text = Selected.MapJson.artist + " - " + Selected.MapJson.title;
            panel.GetChild(1).GetComponent<Text>().text = "mapped by " + Selected.MapJson.mapCreator;
            var timespan = TimeSpan.FromSeconds(audioclip.length);
            panel.GetChild(2).GetComponent<Text>().text = "Status: " + Selected.MapJson.status + "  Length: " +
                                                          string.Format("{0:00}:{1:00}", timespan.Minutes, timespan.Seconds) + "  BPM: " + Selected.MapJson.bpm;
            panel.GetChild(3).GetComponent<Text>().text = "Complexity: " + Selected.MapJson.complexity + "  Objects: " + Selected.MapJson.map.Count;
            PlaySong(audioclip);
            GameStartParameters.MapJson.audioClip = audioclip;
            GameStartParameters.MapJson.map = Selected.MapJson.map;
        }


        void PlaySong(AudioClip clip)
        {
            AudioSource.clip = clip;
            AudioSource.timeSamples = Selected.MapJson.previewTime;
            AudioSource.Play();

        }

        // Update is called once per frame

    }
}
