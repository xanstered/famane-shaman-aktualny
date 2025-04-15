using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeColliderGenerator : MonoBehaviour
{
    public Terrain terrain;
    public bool convex = false; // Czy collider ma by� wypuk�y

    void Start()
    {
        AddCollidersToTrees();
    }

    void AddCollidersToTrees()
    {
        // Pobierz wszystkie instancje drzew z terenu
        TreeInstance[] trees = terrain.terrainData.treeInstances;

        for (int i = 0; i < trees.Length; i++)
        {
            // Konwersja pozycji lokalnej drzewa na pozycj� �wiatow�
            Vector3 treeWorldPos = Vector3.Scale(trees[i].position, terrain.terrainData.size) + terrain.transform.position;

            // Pobierz prefab drzewa
            GameObject treePrefab = terrain.terrainData.treePrototypes[trees[i].prototypeIndex].prefab;

            if (treePrefab != null)
            {
                // Utw�rz instancj� drzewa
                GameObject treeObject = Instantiate(treePrefab, treeWorldPos, Quaternion.identity);

                // Usu� istniej�ce kolidery
                Collider[] existingColliders = treeObject.GetComponents<Collider>();
                foreach (Collider col in existingColliders)
                {
                    Destroy(col);
                }

                // Dodaj Mesh Collider
                MeshCollider meshCollider = treeObject.AddComponent<MeshCollider>();

                // Znajd� Mesh Renderer
                MeshRenderer renderer = treeObject.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    // Pobierz mesh z pierwszego renderera
                    MeshFilter meshFilter = treeObject.GetComponent<MeshFilter>();
                    if (meshFilter != null)
                    {
                        meshCollider.sharedMesh = meshFilter.sharedMesh;
                        meshCollider.convex = convex;
                    }
                }

                // Opcjonalnie: ustaw jako statyczny
                treeObject.isStatic = true;
            }
        }
    }
}
