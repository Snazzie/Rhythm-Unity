using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets
{
    public class InputManager : MonoBehaviour
    {
        private List<KeyCode> ClickKeys { get; set; }

        public event EventHandler OnClick;
        public InputManager()
        {
            // load from config file


            // temporary hardcode keys
            ClickKeys = new List<KeyCode>()
            {
                KeyCode.Q,
                KeyCode.W
            };


        }

        void Update()
        {
            if (OnClick != null)
                if (Input.GetKeyDown(ClickKeys[0]) || Input.GetKeyDown(ClickKeys[1]))
                {
                    OnClick.Invoke(this, EventArgs.Empty);
                }


        }
    }
}
