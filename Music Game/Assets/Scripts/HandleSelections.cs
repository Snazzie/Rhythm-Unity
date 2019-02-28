using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class HandleSelections : MonoBehaviour
    {
        public SongListItem Selected { get; set; } 
        private AudioSource AudioSource { get; set; }
        // Use this for initialization
        void Start()
        {
            // get list
            // default to first
            AudioSource = GameObject.Find("AudioSource").transform.GetComponent<AudioSource>();

        }

        public void UpdateMapInfoPanel()
        {
            var panel = transform.GetChild(0).transform;
            var clip = GetClip(Selected.MapJson.filePath + @"\Audio.wav");

            panel.GetChild(0).GetComponent<Text>().text = Selected.MapJson.artist + " - " + Selected.MapJson.title;
            panel.GetChild(1).GetComponent<Text>().text = "mapped by " + Selected.MapJson.mapCreator;
            var timespan = TimeSpan.FromSeconds(clip.length);
            panel.GetChild(2).GetComponent<Text>().text = "Status: " + Selected.MapJson.status + "  Length: " + 
                                                          string.Format("{0:00}:{1:00}",timespan.Minutes,timespan.Seconds) + "  BPM: " + Selected.MapJson.bpm;
            panel.GetChild(3).GetComponent<Text>().text = "Complexity: " + Selected.MapJson.complexity + "  Objects: " + Selected.MapJson.map.Count;
            PlaySong(clip);
            GameStartParameters.MapJson.audioClip = clip;
            GameStartParameters.MapJson.map = Selected.MapJson.map;
        }
        public static AudioClip GetClip(string path)
        {

            var www = new WWW(path);
            AudioClip audioclip = www.GetAudioClip(true, false, AudioType.WAV);

            while (audioclip.loadState != AudioDataLoadState.Loaded)
            {
                // wait
            }
            Debug.Log("Clip Load finished");
            return audioclip;

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
