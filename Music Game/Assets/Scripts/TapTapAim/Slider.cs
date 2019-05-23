using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TapTapAim
{
    public class Slider : MonoBehaviour, ISlider
    {
        public LineRenderer LineRenderer { get; set; }
        public ISliderPositionRing SliderPositionRing { get; set; }
        public float SliderSpeed { get; set; } = 1f;
        public void DrawSlider()
        {
            LineRenderer.positionCount = 0; // clear existing positions from edit demo
            LineRenderer.positionCount = Points.Count;
            var pointsArry = Points.ToArray();
            LineRenderer.SetPositions(pointsArry);
        }

        public SliderType SliderType { get; set; }
        public List<Vector3> Points { get; set; }

    }
}
