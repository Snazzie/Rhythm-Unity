using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public void Play()
    {
        SceneManager.LoadScene("TapTapAim");
    }
    // Update is called once per frame
}
