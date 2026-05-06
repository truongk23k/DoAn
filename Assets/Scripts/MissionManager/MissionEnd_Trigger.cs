using UnityEngine;

public class MissionEnd_Trigger : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        player = Player.instance.gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != player)
            return;

        if (MissionManager.instance.MissionCompleted())
        {
            GameManager.instance.GameCompleted();
            Debug.Log("Level completed!");
        }
    }
}
