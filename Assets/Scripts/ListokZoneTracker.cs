using UnityEngine;

public class ListokZoneTracker : MonoBehaviour
{
    public bool isInListokZone { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInListokZone = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInListokZone = false;
        }
    }
}
