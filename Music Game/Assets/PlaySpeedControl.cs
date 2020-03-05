using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModSliderControl))]
public class PlaySpeedControl : MonoBehaviour
{
    private ModSliderControl ModSliderControl;
    private AudioSource musicSource;
    // Start is called before the first frame update
    void Start()
    {
        musicSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        ModSliderControl = transform.GetComponent<ModSliderControl>();
        ModSliderControl.OnSliderValueChanged += OnChange;
    }

    private void OnChange()
    {
        musicSource.pitch = ModSliderControl.Value;

        GameStartParameters.PlayBackSpeed = ModSliderControl.Value;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
