using System;
using UnityEngine;

namespace Assets.TapTapAim
{
    public class SliderPositionRing : MonoBehaviour, ISliderPositionRing
    {
        public ITapTapAimSetup TapTapAimSetup { get; set; }
        public int VisibleStartOffsetMs { get; }
        public int VisibleEndOffsetMs { get; }
        public TimeSpan VisibleStartStart { get; }
        public TimeSpan VisibleEndStart { get; }

        void Update()
        {

        }

        public int HitID { get; set; }
        public TimeSpan PerfectHitTime { get; set; }
        public int AccuracyLaybackMs { get; set; }
        public Visibility Visibility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}