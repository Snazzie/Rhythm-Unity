using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleButton : MonoBehaviour {
    private AudioSource audioSource;
    public bool toggle = true;
    // Use this for initialization

    private void Start()
    {
        audioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
        audioSource.Play();
        audioSource.Pause();
        
    }
    public void Toggle()
    {
        if(toggle == false)
        {
            if(audioSource.isPlaying == false)
            {
                audioSource.Play();
            }
            audioSource.UnPause();
            transform.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            toggle = true;
        }
        else
        {
            audioSource.Pause();
            transform.GetComponent<Image>().color = new Color(1, 1, 1, 0.3f);
            toggle = false;
        }

    }
}
