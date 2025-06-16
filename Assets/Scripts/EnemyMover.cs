using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMover : ConnectableMonoBevaviour
{
    public float moveSpeed = 3f;
    public List<EntityView> woodParts = new();

    private void Awake()
    {
        woodParts.ForEach(w =>
        {
            w.connections += w.OnDead.Subscribe(() => woodParts.Remove(w));
        });

        StartCoroutine(CheckDead());
    }
    bool check = true;
    private IEnumerator CheckDead()
    {
        while (check)
        {
            if (woodParts.Count <= 0)
            {
                check = false;
                Destroy(gameObject, 2);
            }

            yield return null;
        }
    }
    private void FixedUpdate()
    {
        if (!check) return;
        Vector3 moveDirection = Vector3.back;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
        }

        foreach (var part in woodParts)
        {
            part.Tick(Time.fixedDeltaTime); // Вызов логики каждой части
        }
    }
}
