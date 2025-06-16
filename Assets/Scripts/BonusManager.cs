using UnityEngine;

public class BonusManager : MonoBehaviour
{
    public GameObject bonusPrefab;
    public Transform player;
    public Transform lanePosition; // одна из полос (например, центр)
    public float spacing = 5f; // расстояние между бонусами
    public float chunkLength = 70f; // длина одной порции
    [SerializeField] private float lastGeneratedZ = 0f;
   

    private void Start()
    {
        float x = lanePosition.position.x;
        float y = lanePosition.position.y;


        for (float z = lastGeneratedZ; z < lastGeneratedZ + chunkLength; z += spacing)
        {
            Vector3 spawnPos = new Vector3(x, y, z);
            GameObject box = Instantiate(bonusPrefab, spawnPos, Quaternion.identity);
        }
    }

    //void Update()
    //{
    //    float distanceToLast = lastGeneratedZ - player.position.z;
    //    if (distanceToLast <= 69f)
    //    {
    //        GenerateBonusChunk(lastGeneratedZ + 1f, lastGeneratedZ + chunkLength);
    //        lastGeneratedZ += chunkLength;
    //    }
    //}

    //void GenerateBonusChunk(float startZ, float endZ)
    //{
    //    float x = lanePosition.position.x;
    //    float y = lanePosition.position.y;

    //    for (float z = startZ; z < endZ; z += spacing)
    //    {
    //        Vector3 spawnPos = new Vector3(x, y, z);
    //        Instantiate(bonusPrefab, spawnPos, Quaternion.identity);
    //    }
    //}
}