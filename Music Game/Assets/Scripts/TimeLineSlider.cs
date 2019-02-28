using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class TimeLineSlider : MonoBehaviour
    {

        private AudioSource audioSource;
        private Slider slider;
        private Text timeStamp;
        public bool IsSetTime { get; set; }
        public bool IsDragging { get; set; }
        // Use this for initialization
        void Start()
        {
            audioSource = Camera.main.transform.Find("AudioSource").GetComponent<AudioSource>();
            timeStamp = GameObject.Find("PlaybackControls").transform.Find("TimeStamp").GetComponent<Text>();
            slider = transform.GetComponent<Slider>();
            slider.maxValue = audioSource.clip.length;
            IsSetTime = false;
            IsDragging = false;

        }



        // Update is called once per frame
        void Update()
        {
            var timespan = TimeSpan.FromSeconds(audioSource.time);
            timeStamp.text = "Current Song Position: " +
                             string.Format("{0:00}:{1:00}:{2:000}",
                                 (int)timespan.TotalMinutes, timespan.Seconds, timespan.Milliseconds);

            if (IsSetTime)
            {
                audioSource.time = slider.value;

            }
            else
            {
                slider.value = audioSource.time;
            }

            //if (!IsDragging)
            //{
            //    if (IsSetTime)
            //    {
            //        audioSource.time = slider.value;
            //        IsSetTime = false;
            //    }
            //    else
            //    {
            //        slider.value = audioSource.time;
            //    }
            //}
        }
    }
}
