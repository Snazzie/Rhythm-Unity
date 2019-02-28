using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    // Use this for initialization
    private Rigidbody rigidbody;
    public bool isPerformMove = true;
    public int speed = 20;
    public float speedMultiplier = 1.5f;

    public Transform gameTracker;
    void Start()
    {
        gameTracker =  GameObject.Find("GameTracker").transform;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            StartCoroutine(gameTracker.GetComponent<GameTracker>().TriggerOuchText());
        }
    }
    private void FixedUpdate()
    {
        if (isPerformMove)
        {
            PerformMovement();
        }
        else
        {
            rigidbody.velocity = new Vector3(0, 0, 0);
        }

    }

    public void PerformMovement()
    {
        if (rigidbody == null)
        {
            rigidbody = GetComponent<Rigidbody>();

        }
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontal, 0, vertical);


        rigidbody.AddForce(movement * speed * speedMultiplier);





    }
}
