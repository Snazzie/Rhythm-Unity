using UnityEngine;
using UnityEngine.UI;

namespace Assets
{
    public class TapTicker : MonoBehaviour
    {
        private Transform B1 { get; set; }
        private int B1Count { get; set; }
        private Transform B2 { get; set; }

        private int B2Count { get; set; }
        // Start is called before the first frame update
        void Start()
        {
            B1 = transform.GetChild(0).GetChild(0);
            B2 = transform.GetChild(1).GetChild(0);
            
        }

        public void IncrementButton(int button)
        {
            switch (button)
            {
                case 1:
                    B1Count++;
                    B1.GetComponent<Text>().text = B1Count.ToString();
                    return;
                case 2:
                    B2Count++;
                    B2.GetComponent<Text>().text = B1Count.ToString();
                    return;
            }
        }
    }
}
