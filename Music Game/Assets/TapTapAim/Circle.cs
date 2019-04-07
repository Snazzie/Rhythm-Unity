using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.TapTapAim
{
    class Circle: MonoBehaviour, ICircle
    {
        public int QueueID { get; set; }
        public TimeSpan PerfectHitTime { get; set; }
        public ITapTapAimSetup TapTapImAimSetup { get; set; }
        public int ID { get; set; }
        public ITapTapAimSetup TapTapAimSetup { get; set; }
        public Visibility Visibility { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
