using System;
using System.Collections.Generic;
using System.Linq;
using Assets.TapTapAim.LineUtility;
using UnityEngine;

namespace Assets.TapTapAim
{
    public class Slider : MonoBehaviour, ISlider
    {
        public LineRenderer LineRenderer { get; set; }
        public ISliderPositionRing SliderPositionRing { get; set; }
        public float SliderSpeed { get; set; } = 1f;
        public void DrawSlider()
        {
            LineRenderer.positionCount = Points.Count;
            var pointsArry = Points.ToArray();
            LineRenderer.SetPositions(pointsArry);
        }

        public SliderType SliderType { get; set; }
        public List<Vector3> Points { get; set; }

    }
}
