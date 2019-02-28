using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayBackSpeedSlider : MonoBehaviour
{
    private Slider slider;
    private AudioSource audioSource;
    public bool IsSetMultiplier { get; set; }
    public bool IsDragging { get; set; }
    public bool IsHoverOver { get; set; }

    public UnityEvent ue = new UnityEvent();
    // Use this for initialization


    private void Awake()
    {
        Application.targetFrameRate = 300;
    }
    void Start()
    {
        audioSource = Camera.main.transform.Find("AudioSource").GetComponent<AudioSource>();
        slider = transform.GetComponent<Slider>();
        slider.minValue = 0.2f;
        slider.maxValue = 2;
        slider.value = 1;
        IsSetMultiplier = false;
        IsDragging = false;
        ue.AddListener(OnScroll);
    }

    // Update is called once per frame
    void Update()
    {
        GameObject.Find("PlaybackControls").transform.Find("PlaybackSpeedSlider").Find("MultiplierText").GetComponent<Text>().text =
            string.Format(@"x{0} Multiplier", Math.Floor(slider.value * 100) / 100);


        if (IsHoverOver)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
            {
                slider.value += 0.01f;
                IsSetMultiplier = true;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
            {
                slider.value -= 0.01f;
                IsSetMultiplier = true;
            }
        }

        if (IsSetMultiplier)
        {
            audioSource.pitch = (float)Math.Floor(slider.value * 100) / 100;

        }
        else
        {
            slider.value = audioSource.pitch;
        }
    }
    void OnScroll()
    {

    }
}
