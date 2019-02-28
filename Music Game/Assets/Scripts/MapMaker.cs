using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapMaker : MonoBehaviour
{
    public Map map;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject.Find("PlaybackControls").transform.Find("PlaybackToggle").GetComponent<ToggleButton>().Toggle();
        }
    }


    public class Map
    {
        private DateTime createDate;
        public string title;
        public string author;
        private byte[] musicFile;
        

        public float playBackSpeedCreated;
        public List<string> notePlacements;
        public string creator;
        private DateTime uploadDate;

        private Map(string musicFilePath)
        {
            createDate = DateTime.Now;
            AddMusic(musicFilePath);
        }

        private bool AddMusic(string path)
        {
            if(musicFile == null)
            {
                // get file

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
