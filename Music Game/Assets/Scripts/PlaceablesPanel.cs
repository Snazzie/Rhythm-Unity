using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class PlaceablesPanel : MonoBehaviour {

        public Transform button;
        private List<Transform> buttons;
        // Use this for initialization
        void Start () {

            buttons = new List<Transform>();
            var rectangle = button;

            rectangle.GetComponent<ObstacleButton>().IsToggled = true;

            buttons.Add(rectangle);

            var second = button;
            buttons.Add(second);
            foreach (var b in buttons)
            {

                Instantiate(b, Camera.main.transform.Find("Canvas").transform.Find("PlaceablesPanel"));
            }

        }
	

        public  void Toggle(Transform transform)
        {
            transform.GetComponent<ObstacleButton>().IsToggled = true;
            transform.GetComponent<Image>().color = new Color(0.8f,0.8f,0.8f);

            var obstacles =  GameObject.FindGameObjectsWithTag("MakerObstacle");

            foreach(var b in buttons)
            {
                if(b != transform)
                {
                    b.GetComponent<ObstacleButton>().IsToggled = false;
                    b.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f);
                }
            }
        }
        // Update is called once per frame
        void Update () {
		
        }
    }
}
