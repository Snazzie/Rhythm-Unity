using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.TapTapAim.LineUtility
{
    [ExecuteInEditMode]
    public class BezierCurve : MonoBehaviour
    {

        public Vector2[] points;


        public LineRenderer LineRenderer;
        public static int vertexCount = 100;
        private List<Vector3> pointList;

        public static List<Vector3> GetPoints(List<Vector3> vectors)
        {
            var pointList = new List<Vector3>();
            for (int i = 0; i < vectors.Count; i += 3)
            {
                if (!(i + 2 >= vectors.Count))
                {
                    for (float ratio = 0f; ratio <= 1; ratio += 1.0f / vertexCount)
                    {

                        var tangentLineVertex1 = Vector2.Lerp(vectors[i], vectors[i + 1], ratio);
                        var tangentLineVector2 = Vector2.Lerp(vectors[i + 1], vectors[i + 2], ratio);
                        var bezierPoint = Vector2.Lerp(tangentLineVertex1, tangentLineVector2, ratio);
                        pointList.Add(bezierPoint);

                    }
                }
            }


            return pointList;
        }
        void Update()
        {
            pointList = new List<Vector3>();

            for (int i = 0; i < points.Length; i += 3)
            {
                for (float ratio = 0f; ratio <= 1; ratio += 1.0f / vertexCount)
                {
                    var tangentLineVertex1 = Vector2.Lerp(points[i], points[i + 1], ratio);
                    var tangentLineVector2 = Vector2.Lerp(points[i + 1], points[i + 2], ratio);
                    var bezierPoint = Vector2.Lerp(tangentLineVertex1, tangentLineVector2, ratio);
                    pointList.Add(bezierPoint);
                }
            }

            LineRenderer.positionCount = pointList.Count;
            LineRenderer.SetPositions(pointList.ToArray());
        }
        //public void SetPoints(List<Vector2> points, List<BezierControlPointMode> bezierControlPoints)
        //{
        //    this.points = points.ToArray();
        //    modes = bezierControlPoints.ToArray();
        //}

        void OnDrawGizmos()
        {

            //Gizmos.DrawLine(point1.position, point2.position);


            //Gizmos.DrawLine(point2.position, point3.position);

            //Gizmos.color = Color.red;
            for (float ratio = 0f / vertexCount; ratio < 1; ratio += 1.0f / vertexCount)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(Vector2.Lerp(points[0], points[1], ratio), Vector2.Lerp(points[1], points[2], ratio));
                Gizmos.color = Color.green;
                Gizmos.DrawLine(Vector2.Lerp(points[3], points[4], ratio), Vector2.Lerp(points[4], points[5], ratio));
            }

        }

    }
}
