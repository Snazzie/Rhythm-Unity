using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.TapTapAim.LineUtility
{
    public class LinearLine : MonoBehaviour
    {
        public List<Vector3> pointList;
        public LineRenderer LineRenderer;

        public void SetUp(Vector2 p1, Vector2 p2)
        {
            pointList = new List<Vector3>() { p1, p2 };

            LineRenderer.positionCount = pointList.Count;
            LineRenderer.SetPositions(pointList.ToArray());
        }

        public static List<Vector3> GetPoints(Vector2 p1, Vector2 p2)
        {
            return new List<Vector3>() { p1, p2 };
        }
        public List<Vector3> GetPoints()
        {
            return pointList;
        }

    }
}
