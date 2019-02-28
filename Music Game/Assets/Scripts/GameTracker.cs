using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTracker : MonoBehaviour {

    public float Score { get; private set; }
    private Text ScoreText;
    public bool IsGameOver { get; private set; }
    public AudioSource AudioSource { get; set; }
	// Use this for initialization
	void Start () {
        Score = 0;
        ScoreText = Camera.main.transform.Find("Canvas").transform.Find("Score").GetComponent<Text>();
        IsGameOver = false;
	    AudioSource = GameObject.Find("AudioSource").GetComponent<AudioSource>();
	}
	
    void SetGameOver()
    {
        IsGameOver = true;
    }
    public  IEnumerator TriggerOuchText()
    {

        Camera.main.transform.Find("Canvas").transform.Find("Ouch").GetComponent<Text>().enabled = true;
        new WaitForSecondsRealtime(500);
        Camera.main.transform.Find("Canvas").transform.Find("Ouch").GetComponent<Text>().enabled = false;
        yield return 0;
    }
	// Update is called once per frame
	void Update () {
        Score += 12 * Time.deltaTime;

        ScoreText.text = "Score:" + (int)Score;
	}
}
