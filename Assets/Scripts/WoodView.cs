using System.Collections.Generic;
using UnityEngine;

public class WoodView : MonoBehaviour
{
    public float moveSpeed = 3f;
    public List<WoodPartView> woodParts = new();

    private void Awake()
    {
        woodParts.ForEach(w =>
        {
            w.connections += w.OnDead.Subscribe(() => woodParts.Remove(w));
        });
    }

    private void FixedUpdate()
    {
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
