using Assets.TapTapAim.LineUtility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TapTapAim
{
    public class Slider : MonoBehaviour, ISlider
    {
        public LineRenderer LineRenderer { get; set; }
        public ISliderPositionRing SliderPositionRing { get; set; }
        public float SliderSpeed { get; set; } = 1f;
        public List<Vector3> Points { get; set; }
        public void DrawSlider()
        {
            LineRenderer.positionCount = 0; // clear existing positions from edit demo
            LineRenderer.positionCount = Points.Count;
            var pointsArry = Points.ToArray();
            LineRenderer.SetPositions(pointsArry);
        }

        public SliderType SliderType { get; set; }
        public Vector3 GetPositionAtTime(float tParam)
        {


            switch (SliderType)
            {
                case SliderType.LinearLine:
                    return Points[0] + tParam * (Points[1] - Points[0]);
                case SliderType.QuadraticBezierCurve:
                    {
                        return PerfectCurve.CalculateQuadraticBezierPoint(tParam, Points[0], Points[1], Points[2]);
                    }
                case SliderType.BezierCurve:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }



    }
}
