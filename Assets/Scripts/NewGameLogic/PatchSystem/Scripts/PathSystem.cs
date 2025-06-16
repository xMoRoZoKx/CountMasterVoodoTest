using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PatchSystem : MonoBehaviour
{
    [SerializeField] private Color gizmoColor = Color.green;
    [SerializeField] private bool drawArrows = true;
    [SerializeField] private float arrowHeadLength = 0.3f;
    [SerializeField] private float arrowHeadAngle = 20.0f;

    private List<Transform> waypoints = new List<Transform>();

    /// <summary>
    /// Повертає список позицій активного маршруту.
    /// </summary>
    public List<Vector3> GetPath()
    {
        UpdateWaypointList();
        List<Vector3> path = new List<Vector3>();
        foreach (var point in waypoints)
        {
            if (point.gameObject.activeInHierarchy)
                path.Add(point.position);
        }
        return path;
    }

    /// <summary>
    /// Повертає точки з PathPoint-ами.
    /// </summary>
    public List<PathPoint> GetPathPoints()
    {
        UpdateWaypointList();
        List<PathPoint> result = new List<PathPoint>();
        foreach (var t in waypoints)
        {
            var pp = t.GetComponent<PathPoint>();
            if (pp != null && t.gameObject.activeInHierarchy)
                result.Add(pp);
        }
        return result;
    }

    private void UpdateWaypointList()
    {
        waypoints.Clear();
        foreach (Transform child in transform)
        {
            waypoints.Add(child);
        }
        waypoints.Reverse();
    }

    private void OnDrawGizmos()
    {
        var path = GetPath();
        Gizmos.color = gizmoColor;

        for (int i = 0; i < path.Count - 1; i++)
        {
            var from = path[i];
            var to = path[i + 1];

            Gizmos.DrawLine(from, to);
            if (drawArrows)
                DrawArrowHead(from, to);
        }
    }

    private void DrawArrowHead(Vector3 from, Vector3 to)
    {
        Vector3 direction = (to - from).normalized;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        Vector3 arrowTip = Vector3.Lerp(from, to, 0.9f);

        Gizmos.DrawLine(arrowTip, arrowTip + right * arrowHeadLength);
        Gizmos.DrawLine(arrowTip, arrowTip + left * arrowHeadLength);
    }
}
