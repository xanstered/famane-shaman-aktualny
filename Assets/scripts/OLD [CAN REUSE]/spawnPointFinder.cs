using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class spawnPointFinder : MonoBehaviour
{
    [SerializeField] private Transform player;
    private GameObject spawnPoint;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }
    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "chata")
        {
            spawnPoint = GameObject.Find("SpawnPoint");
            player.SetPositionAndRotation(spawnPoint.transform.position, player.transform.rotation);
            Physics.SyncTransforms();
        }

        if (scene.name == "Prolog")
        {
            spawnPoint = GameObject.Find("SpawnPoint");
            player.SetPositionAndRotation(spawnPoint.transform.position, player.transform.rotation);
            Physics.SyncTransforms();
            Debug.Log("Spawn Found");
        }

        if (scene.name == "hub")
        {
            spawnPoint = GameObject.Find("SpawnPoint");
            player.SetPositionAndRotation(spawnPoint.transform.position, player.transform.rotation);
            Physics.SyncTransforms();
        }
    }
}
