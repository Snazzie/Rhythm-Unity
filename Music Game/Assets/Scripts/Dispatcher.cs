using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Dispatcher : MonoBehaviour {

        protected Queue<Action> _pending = new Queue<Action>();

        void Update()
        {
            lock (_pending)
            {
                while (_pending.Count != 0) _pending.Dequeue().Invoke();
            }
        }

        public void Invoke(Action a)
        {
            lock (_pending) { _pending.Enqueue(a); }
        }
	
    }
}
