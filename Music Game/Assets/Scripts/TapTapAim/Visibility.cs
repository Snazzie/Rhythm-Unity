using System;

namespace Assets.TapTapAim
{
    public class Visibility
    {
        public int VisibleStartOffsetMs { get; set; }
        public int VisibleEndOffsetMs { get; set; }
        public TimeSpan VisibleStartStart { get; set; }
        public TimeSpan VisibleEndStart { get; set; }
        public bool fadeInTriggered { get; set; }
        public bool fadeOutTriggered { get; set; }
    }
}
