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
            var positions = new Dictionary<int, Vector3>();

            for (int i = 1; i < vectors.Count + 1; i++)
            {
                float t = i / (float)vectors.Count;
                positions[i - 1] = CalculatePerfectArcPoint(t, vectors[0], vectors[1], vectors[2]);
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

        public static Vector2 CalculatePerfectArcPoint(float tParam, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            var p2Actual = GetPTwoActual(p1, p2, p3);
            float u = 1 - tParam;
            float tt = tParam * tParam;
            float uu = u * u;
            Vector2 p = uu * p1;
            p += 2 * u * tParam * p2Actual;
            p += tt * p3;
            return p;
        }

        private static Vector2 GetCenter(Vector2 start, Vector2 end)
        {
            return start + 0.5f * (end - start);
        }

        /// <summary>
        /// Normalizes p2 to create a isosceles. its a good enough thing to help mimick osu!'s Arc bullshit
        /// credit https://gist.github.com/Mikeywalsh/4ac256e0a6c30df9b37ccc6a6ff50a05
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        private static Vector2 GetPTwoActual(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float gradient1;
            float gradient2;
            float yIntersect2;

            gradient1 = (p3.y - p1.y) / (p3.x - p1.x);
            gradient2 = gradient1;
            yIntersect2 = p2.y - gradient1 * (p2.x);

            var midPos = (p3 + p1) / 2;

            float yIntersect3;
            float gradient3;
            float resultX;
            float resultY;
            if (Mathf.Abs(gradient1) > float.Epsilon)
            {
                gradient3 = (-1 / gradient1);
                yIntersect3 = midPos.y - (gradient3 * midPos.x);
                resultX = (yIntersect3 - yIntersect2) / (gradient2 - gradient3);
                resultY = (gradient3 * resultX) + yIntersect3;
            }
            else
            {
                resultX = midPos.x;
                resultY = p2.y;
            }

            return new Vector2(resultX, resultY);
        }
    }
}
