using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.TapTapAim;
using Assets.TapTapAim;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameLaunchSetup : MonoBehaviour
{
    private bool userControl;

    private Tracker tracker;
    // Use this for initialization
    private GameObject _Cursor { get; set; }
    void Start()
    {
        _Cursor = GameObject.FindWithTag("Cursor");

        try
        {
            tracker = GameObject.Find("Tracker").GetComponent<Tracker>();
        }
        catch
        {

        }


        Debug.Log(SceneManager.GetActiveScene().name);

    }

    // Update is called once per frame
    void Update()
    {

        




    }


}
