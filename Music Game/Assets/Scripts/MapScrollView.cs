using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapScrollView : MonoBehaviour
{
    private AudioSource audioSource;
    private Scrollbar scrollBar;
    private float BPM = 140f;
    // Use this for initialization
    void Start()
    {
        audioSource = Camera.main.transform.Find("AudioSource").GetComponent<AudioSource>();
        scrollBar = transform.Find("ScrollbarHorizontal").GetComponent<Scrollbar>();

        var centerLine = transform.Find("SnapLine").GetComponent<RectTransform>().sizeDelta;
        var field = transform.Find("Viewport").Find("OffsetPlane").GetComponent<RectTransform>();
        field.sizeDelta = new Vector2(675 + (audioSource.clip.length * 110000 / BPM), 200);
        scrollBar.numberOfSteps = 0;
    }

    // Update is called once per frame
    void Update()
    {

        var timeline = GameObject.Find("TimeLineSlider").GetComponent<Slider>();
        scrollBar.value = (timeline.value / timeline.maxValue);
    }
}
