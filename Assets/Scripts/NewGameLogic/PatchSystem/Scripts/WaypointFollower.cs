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

    [Header("Rotation Settings")]
    [SerializeField] private float returnRotationDuration = 0.5f;

    [Header("Events")]
    public UnityEvent onPathComplete;

    private List<Vector3> pathPoints;
    private Quaternion initialRotation;
    private Tween moveTween;

    private void Start()
    {
        StartMoving();
    }

    public void StartMoving()
    {
        if (targetToMove == null || moveTween != null) return;

        var points = pathSystem.GetPathPoints();
        if (points.Count < 2) return;

        pathPoints = new List<Vector3>();
        foreach (var p in points)
            pathPoints.Add(p.transform.position);

        initialRotation = targetToMove.rotation;

        moveTween = targetToMove
            .DOPath(pathPoints.ToArray(), totalDuration, PathType.CatmullRom, PathMode.Full3D, 10, Color.green)
            .SetEase(Ease.Linear)
            .SetLookAt(0.05f) // јвтоматическое направление взгл€да по пути
            .OnComplete(() =>
            {
                targetToMove.DORotateQuaternion(initialRotation, returnRotationDuration)
                    .OnComplete(() => onPathComplete?.Invoke());

                moveTween = null;
                targetToMove = null;
            });
    }

    public void StopMoving()
    {
        if (moveTween != null && moveTween.IsActive())
        {
            moveTween.Kill();
            moveTween = null;
        }
    }
}
