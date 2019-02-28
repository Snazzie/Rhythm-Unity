using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Player player;
    // Use this for initialization
    void Start()
    {
        player = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {

    }






    public void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.D))
        {
            player.isPerformMove = true;

        }
        else
        {
            player.isPerformMove = false;
        }
    }
}
