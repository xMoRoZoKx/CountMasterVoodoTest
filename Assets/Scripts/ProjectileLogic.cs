using UnityEngine;

public class ProjectileLogic : MonoBehaviour
{
    private Vector3 direction;
    private float speed;
    private int damage;
    private float maxDistance;
    private Vector3 startPosition;

    private bool isBonusSpawned = false;

    public void Initialize(float speed, Vector3 direction, int damage, float maxDistance = 50f, bool isBonusSpawned = false)
    {
        this.speed = speed;
        this.direction = direction.normalized;
        this.damage = damage;
        this.maxDistance = maxDistance;
        this.startPosition = transform.position;
        this.isBonusSpawned = isBonusSpawned;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;

        // Проверка расстояния
        float distanceTraveled = Vector3.Distance(startPosition, transform.position);
        if (distanceTraveled >= maxDistance)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("KICK ENEMY");
            WoodPartView enemy = other.GetComponent<WoodPartView>();
            if (enemy != null)
            {
                Debug.Log("DAMAGE TO ENEMY:" + damage);
                enemy.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (other.CompareTag("Bonus"))
        {
            if (!isBonusSpawned) // только если снаряд оригинальный
            {
                Bonus bonus = other.GetComponent<Bonus>();
                if (bonus != null)
                {
                    bonus.Activate(
                        direction,
                        transform.position,
                        projectilePrefab: gameObject, // можно заменить на префаб, если нужно
                        speed,
                        damage,
                        maxDistance
                    );
                }
            }
        }
        else if (other.CompareTag("Obstacle"))
        {
            Obstacle obstacle = other.GetComponent<Obstacle>();
            if (obstacle != null)
            {
                obstacle.TakeHit();
            }

            Destroy(gameObject);
        }
    }

    public void OverrideDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
    }

}