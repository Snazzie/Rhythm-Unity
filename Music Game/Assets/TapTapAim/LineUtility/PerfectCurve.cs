using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.TapTapAim.LineUtility
{
    public class PerfectCurve : MonoBehaviour
    {
        public List<Vector3> pointList;


        public LineRenderer LineRenderer;
        public int vertexCount = 100;

        public void SetUp(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            pointList = new List<Vector3>() { p1, p2, p3 };

            float radius = Vector2.Distance(GetCenter(p1, p3), p2);
            LineRenderer.positionCount = pointList.Count;
            LineRenderer.SetPositions(pointList.ToArray());
        }

        private Vector2 GetCenter(Vector2 start, Vector2 end)
        {
            var difference = start - end;
            return start + difference * 0.5f;
        }
    }
}
