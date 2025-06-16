using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowerSystem : MonoBehaviour
{
    [SerializeField] private PatchSystem patchSystem;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float reachThreshold = 0.1f;

    private class Follower
    {
        public GameObject Object;
        public int TargetIndex;
        public Action OnComplete;
        public bool Completed;
    }

    private List<Vector3> pathPoints = new List<Vector3>();
    private List<Follower> followers = new List<Follower>();

    private void Start()
    {
        if (patchSystem == null)
        {
            Debug.LogError("PatchSystem не установлен!");
            enabled = false;
            return;
        }

        pathPoints = patchSystem.GetPath();

        if (pathPoints.Count < 2)
        {
            Debug.LogError("Недостаточно точек маршрута!");
            enabled = false;
        }
    }

    /// <summary>
    /// Добавляет объект для следования по маршруту. Можно передать onComplete, который вызовется при достижении конца пути.
    /// </summary>


    public void AddObject(GameObject obj, Action onComplete = null)
    {
        if (obj == null) return;

        // Починаємо рух до першої точки маршрута, незалежно від стартової позиції
        followers.Add(new Follower
        {
            Object = obj,
            TargetIndex = 0, // Стартуємо з нуля
            OnComplete = onComplete,
            Completed = false
        });
    }

    private void Update()
    {
        if (pathPoints.Count < 2) return;

        foreach (var follower in followers)
        {
            if (follower.Object == null || follower.Completed || follower.TargetIndex >= pathPoints.Count)
                continue;

            Transform objTransform = follower.Object.transform;
            Vector3 target = pathPoints[follower.TargetIndex];

            objTransform.position = Vector3.MoveTowards(
                objTransform.position,
                target,
                moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(objTransform.position, target) <= reachThreshold)
            {
                follower.TargetIndex++;

                if (follower.TargetIndex >= pathPoints.Count)
                {
                    follower.Completed = true;
                    follower.OnComplete?.Invoke();
                }
            }
        }
    }

}
