using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour {

    // Use this for initialization
    public float speed = 10;
	void Start () {
		
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * -1 * Time.deltaTime * speed);
    }
}
