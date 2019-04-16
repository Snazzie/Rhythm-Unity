using System;
using UnityEngine;

namespace Assets.TapTapAim
{
    public class SliderPositionRing : MonoBehaviour, ISliderPositionRing
    {


        void Update()
        {

        }
        public Visibility Visibility { get; set; }
        public TapTapAimSetup TapTapAimSetup { get; set; }
    }
}