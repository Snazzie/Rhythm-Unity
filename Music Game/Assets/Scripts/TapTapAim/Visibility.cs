using System;

namespace Assets.TapTapAim
{
    public class Visibility
    {
        public Double VisibleStartOffsetMs { get; set; }
        public Double VisibleEndOffsetMs { get; set; }
        public Double VisibleStartStartTimeInMs { get; set; }
        public Double VisibleEndStartTimeInMs { get; set; }
        public bool fadeInTriggered { get; set; }
        public bool fadeOutTriggered { get; set; }
    }
}
