using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Placement
{

    public static List<Vector3> GetPointsOnSphere(int nPoints, float radius) {
        List<Vector3> points = new List<Vector3>();
        float fPoints = (float)nPoints;
        float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
        float off = 2 / fPoints;

        for (int k = 0; k < nPoints; k++) {
            float y = k * off - 1 + (off / 2);
            float r = Mathf.Sqrt(1 - y * y);
            float phi = k * inc;
            points.Add(new Vector3(Mathf.Cos(phi) * r, y, Mathf.Sin(phi) * r));
        }

        var scaler = new Vector3(radius, radius, radius);
        for (int i = 0; i < points.Count; i++) {
            points[i] = Vector3.Scale(points[i], scaler);
        }

        return points;
    }

    public static List<Vector3> SortPointsBasedOnDistance(List<Vector3> points, Vector3 point)
    {
        return points.OrderBy(
           x => Vector3.Distance(point, x)
          ).ToList();
    }

    public static List<Vector3> RemoveFromBottom(List<Vector3> points, int numberToRemove) {
        var newList = points.OrderByDescending(p => p.y).ToList();
        while (numberToRemove > 0) {
            if (newList.Count > 0) {
                newList.RemoveAt(newList.Count - 1);
            }
            numberToRemove--;
        }
        return newList;
    }

    public static List<Vector3> RotatePoints(List<Vector3> points, float range = 45f) {
        var rotation = Quaternion.Euler(UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range), UnityEngine.Random.Range(-range, range));
        for (int i = 0; i < points.Count; i++) {
            points[i] = rotation * points[i];
        }
        return points;
    }

}
