using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.TapTapAim;
using Assets.TapTapAim;
using UnityEngine;
using UnityEngine.UI;
using Slider = UnityEngine.UI.Slider;

public class TapTapAimUI : MonoBehaviour {

    private Text Score { get; set; }
    private Text Combo { get; set; }
    private Slider HealthBar { get; set; }
    private Tracker _Tracker { get; set; }
    private Text _Accuracy { get; set; }
    // Use this for initialization
    void Start ()
    {
        Score = GameObject.FindWithTag("Score").transform.GetComponent<Text>();
        Combo = GameObject.FindWithTag("Combo").transform.GetComponent<Text>();
        HealthBar = GameObject.Find("HealthBar").transform.GetComponent<Slider>();
        _Tracker = GameObject.Find("Tracker").transform.GetComponent<Tracker>();
        _Accuracy = GameObject.Find("Accuracy").transform.GetComponent<Text>();
        HealthBar.maxValue = 100;
    }
	
	// Update is called once per frame
	void Update ()
	{
	    Combo.text = "x" + _Tracker.Combo;
	    Score.text = FormatNumber(_Tracker.Score);
	    HealthBar.value = _Tracker.Health;
	    _Accuracy.text = _Tracker.HitAccuracy.ToString("###.#") + "%";
	}
    static string FormatNumber(long num)
    {
        if (num >= 100000000)
        {
            return (num / 100000000D).ToString("0.000B");
        }
        if (num >= 1000000)
        {
            return (num / 1000000D).ToString("0.000M");
        }
        if (num >= 100000)
        {
            return (num / 1000D).ToString("00.00k");
        }
        if (num >= 10000)
        {
            return (num / 1000D).ToString("0.00k");
        }

        return num.ToString("#,0");
    }
}
