using System;
using UnityEngine;

namespace Assets.TapTapAim
{
    public class SliderPositionRing : MonoBehaviour, ISliderPositionRing
    {
        public event EventHandler OnInteract;

        void Update()
        {

        }

        public void TryInteract()
        {
            throw new NotImplementedException();
        }

        public bool IsInInteractionBound(TimeSpan time)
        {
            throw new NotImplementedException();
        }

        public Visibility Visibility { get; set; }
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public int InteractionID { get; set; }
        public TimeSpan PerfectInteractionTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int AccuracyLaybackMs { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public TimeSpan InteractionBoundStart => throw new NotImplementedException();

        public TimeSpan InteractionBoundEnd => throw new NotImplementedException();
    }
}