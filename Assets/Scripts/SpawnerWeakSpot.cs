using UnityEngine;

public class SpawnerWeakSpot : MonoBehaviour
{
    private EnemySpawner spawner;

    private void Start()
    {
        spawner = GetComponentInParent<EnemySpawner>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile")) // убедись, что снаряды имеют этот тег
        {
            if (spawner != null)
            {
                spawner.TakeHit();
            }

            Destroy(other.gameObject); // уничтожить снаряд
        }
    }
}