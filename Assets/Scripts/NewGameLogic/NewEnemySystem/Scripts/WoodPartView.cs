using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniTools;

public class WoodPartView : ConnectableMonoBevaviour
{
    public int maxHealth = 30;
    public GameObject hitEffect;

    private int currentHealth;
    private bool isDead = false;
    private bool sinking = false;
    private Vector3 sinkTarget;
    private Animator animator;
    private Transform player;
    private CharacterController3Lane playerController;
    public Transform parsContainer;
    public Transform riderView;

    public UnityEvent OnDead;

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            playerController = player.GetComponent<CharacterController3Lane>();
        }
        Physics.gravity = new Vector3(0, -150, 0);
    }

    public void Tick(float dt)
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < 1f)
        {
            if (playerController != null && playerController.isLandingNow)
                return;

            Debug.Log("ИГРОК ПРОИГРАЛ!");
            Time.timeScale = 0f;
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        // if (hitEffect != null)
        //     Instantiate(hitEffect, transform.position + Vector3.up * 1f, Quaternion.identity);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        isDead = true;

        if (animator != null)
            animator.SetTrigger("Death");

        sinkTarget = transform.position + Vector3.down * 3f;
        sinking = true;

        StartCoroutine(AddPhysicsToPartsCoroutine());

        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 1.5f);

        OnDead.Invoke();
    }
    private IEnumerator AddPhysicsToPartsCoroutine()
    {
        float radius = 0.0001f;
        float height = radius * 2f;

        if (riderView != null)
        {
            AddPhysicsComponent(riderView, radius, height, 0);
        }
        if (parsContainer != null)
        {
            // Собираем все дочерние трансформы в список
            List<Transform> parts = new List<Transform>();
            foreach (Transform part in parsContainer)
            {
                parts.Add(part);
            }

            // Сортируем по позиции Y (по возрастанию — от самого нижнего)
            parts.Sort((a, b) =>
            {
                int zCompare = a.position.z.CompareTo(b.position.z); // от дальнего к ближнему
                if (zCompare != 0)
                    return zCompare;

                // если Z равны, сравниваем по Y
                return a.position.y.CompareTo(b.position.y); // сверху вниз
            });

            // Добавляем физику по очереди
            foreach (Transform part in parts)
            {
                AddPhysicsComponent(part, radius, height, 13);
                if (RandomTools.GetChance(5)) yield return null;
            }
        }
    }
    private void AddPhysicsComponent(Transform obj, float radius, float height, float velocityMult = 1)
    {
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.gameObject.AddComponent<Rigidbody>();
            rb.mass = 10f;

            // Определяем направление по X относительно родителя
            float localX = obj.localPosition.x;
            float localY = obj.localPosition.y;
            float directionX = 0f;

            const float centerDeadZone = 0.1f; // Центр (нейтральная зона)
            if (localX > centerDeadZone)
                directionX = -1f;
            else if (localX < -centerDeadZone)
                directionX = 1f;

            // Добавляем инерцию

            var vel = new Vector3((-localX * 2) * Random.Range(0.1f, 0.3f), 0.0f + localY, -1.2f) * (velocityMult + Random.Range(-0.1f, 0.1f));
            rb.velocity = vel;
           // DOVirtual.DelayedCall(0.6f, () =>
           // rb.velocity = vel.WithY(-3));
        }

        if (obj.GetComponent<CapsuleCollider>() == null)
        {
            var col = obj.gameObject.AddComponent<CapsuleCollider>();
            col.radius = radius;
            col.height = height;
            col.direction = 1; // Y-axis
            col.isTrigger = true;
        }
    }



}
