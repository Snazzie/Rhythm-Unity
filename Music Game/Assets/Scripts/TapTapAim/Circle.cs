using System;
using Assets.TapTapAim;
using UnityEngine;

namespace Assets.Scripts.TapTapAim
{
    class Circle: MonoBehaviour, ICircle
    {
        public int QueueID { get; set; }
        public TimeSpan PerfectHitTime { get; set; }
        public int ID { get; set; }
        public TapTapAimSetup TapTapAimSetup { get; set; }
        public Visibility Visibility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
