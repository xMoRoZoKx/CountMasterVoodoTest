using UnityEngine;

public class ListokVisibilityZone : MonoBehaviour
{
    [Tooltip("Объект Player, у которого есть child 'Listok'")]
    public GameObject player;

    public ListokView listok;

    private void Start()
    {
        if (player != null)
        {
           // Transform found = player.transform.Find("Listok");
            //if (found != null)
            //    listok = found;
            //else
            //    Debug.LogWarning("Не найден child 'Listok' у объекта Player!");
        }
        else
        {
            Debug.LogError("Не назначен объект Player в ListokVisibilityZone.");
        }

        SetListokVisible(false); // по умолчанию скрыт
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            SetListokVisible(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            SetListokVisible(false);
        }
    }

    private void SetListokVisible(bool visible)
    {
        if (listok != null)
        {
            listok.SetPaused(!visible);
           // listok.gameObject.SetActive(visible);
        }
    }
}
