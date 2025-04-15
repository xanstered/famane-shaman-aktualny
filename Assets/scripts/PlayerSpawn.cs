using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject playerPrefab;

    void Start()
    {
        if (playerPrefab != null && spawnPoint != null)
        {
            Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        }
    }
}