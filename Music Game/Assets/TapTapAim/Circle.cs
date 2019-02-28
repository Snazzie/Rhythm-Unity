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
        public int VisibleStartOffsetMs { get; }
        public int VisibleEndOffsetMs { get; }
        public TimeSpan VisibleStartStart { get; }
        public TimeSpan VisibleEndStart { get; }
        public ITapTapAimSetup TapTapImAimSetup { get; set; }
        public int ID { get; set; }
        public ITapTapAimSetup TapTapAimSetup { get; set; }
    }
}
