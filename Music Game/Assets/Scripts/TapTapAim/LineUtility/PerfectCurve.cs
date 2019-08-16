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

            pointList = new List<Vector3> { p1, p2, p3 };


            LineRenderer.positionCount = pointList.Count;
            LineRenderer.SetPositions(GetPoints(pointList).ToArray());
        }
        public static List<Vector3> GetPoints(List<Vector3> vectors)
        {
            var positions = new Dictionary<int,Vector3>();

            for (int i = 1; i < vectors.Count + 1; i++)
            {
                float t = i / (float)vectors.Count;
                positions[i - 1] = CalculateQuadraticBezierPoint(t, vectors[0], vectors[1], vectors[2]);
            }
            return positions.Values.ToList();
        }

        public static Vector2 CalculateQuadraticBezierPoint(float tParam, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - tParam;
            float tt = tParam * tParam;
            float uu = u * u;
            Vector2 p = uu * p1;
            p += 2 * u * tParam * p2;
            p += tt * p3;
            return p;
        }
        private Vector2 GetCenter(Vector2 start, Vector2 end)
        {
            var difference = start - end;
            return start + difference * 0.5f;
        }
    }
}
