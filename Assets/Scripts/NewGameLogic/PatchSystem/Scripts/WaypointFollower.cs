using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class WaypointFollower : MonoBehaviour
{
    [Header("Path Settings")]
    [SerializeField] private PatchSystem pathSystem;
    [SerializeField] private Transform targetToMove;
    [SerializeField] private float totalDuration = 5f;
    [SerializeField] private AnimationCurve speedCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Spline Settings")]
    [SerializeField, Range(0f, 1f)] private float splineTension = 0.5f; // „ем меньше Ч тем м€гче крива€
    [SerializeField] private float lookAheadDistance = 0.05f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSmoothness = 5f;
    [SerializeField] private float returnRotationDuration = 0.5f;

    [Header("Events")]
    public UnityEvent onPathComplete;

    private Coroutine moveRoutine;
    private Quaternion initialRotation;
    private List<Vector3> pathPoints;
    private float gizmoSampleResolution = 50f;

    private void Start()
    {
        StartMoving();
    }

    public void StartMoving()
    {
        if (targetToMove == null || moveRoutine != null) return;

        var points = pathSystem.GetPathPoints();
        if (points.Count < 2) return;

        pathPoints = new List<Vector3>();
        foreach (var p in points)
            pathPoints.Add(p.transform.position);

        initialRotation = targetToMove.rotation;
        moveRoutine = StartCoroutine(FollowPath());
    }

    private IEnumerator FollowPath()
    {
        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            float t = Mathf.Clamp01(elapsed / totalDuration);
            float eval = speedCurve.Evaluate(t);

            Vector3 pos = GetCatmullRomPosition(eval);
            targetToMove.position = pos;

            // ѕоворот в сторону следующей позиции
            float nextT = Mathf.Clamp01(t + lookAheadDistance);
            Vector3 nextPos = GetCatmullRomPosition(speedCurve.Evaluate(nextT));
            Vector3 dir = (nextPos - pos).normalized;

            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                targetToMove.rotation = Quaternion.Slerp(targetToMove.rotation, targetRot, Time.deltaTime * rotationSmoothness);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // ‘инальна€ позици€
        targetToMove.position = pathPoints[pathPoints.Count - 1];

        // ѕлавный возврат к начальному повороту
        targetToMove.DORotateQuaternion(initialRotation, returnRotationDuration)
            .OnComplete(() => onPathComplete?.Invoke());

        moveRoutine = null;
        targetToMove = null;
    }

    private Vector3 GetCatmullRomPosition(float t)
    {
        int count = pathPoints.Count;
        float segmentT = t * (count - 1);
        int i = Mathf.FloorToInt(segmentT);

        int i0 = Mathf.Clamp(i - 1, 0, count - 1);
        int i1 = Mathf.Clamp(i, 0, count - 1);
        int i2 = Mathf.Clamp(i + 1, 0, count - 1);
        int i3 = Mathf.Clamp(i + 2, 0, count - 1);

        float localT = segmentT - i;

        return CatmullRom(pathPoints[i0], pathPoints[i1], pathPoints[i2], pathPoints[i3], localT);
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            (2f * p1) +
            (-p0 + p2) * t * splineTension +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 * splineTension +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3 * splineTension
        );
    }

    public void StopMoving()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (pathPoints == null || pathPoints.Count < 4) return;

        Gizmos.color = Color.green;
        Vector3 prev = GetCatmullRomPosition(0f);

        for (int i = 1; i <= gizmoSampleResolution; i++)
        {
            float t = i / gizmoSampleResolution;
            Vector3 pos = GetCatmullRomPosition(t);
            Gizmos.DrawLine(prev, pos);
            prev = pos;
        }
    }
#endif
}
