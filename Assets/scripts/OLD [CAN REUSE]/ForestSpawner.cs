using UnityEngine;

public class ForestSpawner : MonoBehaviour
{
    [System.Serializable] // Pozwala ustawiæ wartoœci w Inspectorze
    public class TreeType
    {
        public GameObject prefab; // Prefab drzewa
        public float probability; // Jak czêsto wystêpuje
    }

    public TreeType[] treeTypes; // Lista ró¿nych drzew
    public int treeCount = 500;
    public float areaSize = 100f;

    public LayerMask groundLayer; // Warstwa terenu, aby poprawnie ustawiaæ drzewa

    void Start()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-areaSize / 2, areaSize / 2),
                50, // Wysokoœæ pocz¹tkowa (bêdziemy raycastowaæ w dó³)
                Random.Range(-areaSize / 2, areaSize / 2)
            );

            // Raycast w dó³, aby znaleŸæ dok³adn¹ wysokoœæ gruntu
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 100, groundLayer))
            {
                position.y = hit.point.y; // Ustaw wysokoœæ drzewa na poziomie gruntu

                GameObject selectedTree = SelectRandomTree(); // Wybór losowego drzewa

                // Losowa rotacja i skala
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                float scale = Random.Range(0.8f, 1.2f);

                // Tworzenie drzewa
                GameObject treeInstance = Instantiate(selectedTree, position, rotation);
                treeInstance.transform.localScale *= scale;
                treeInstance.transform.parent = transform; // Umieszczenie w hierarchii
            }
        }
    }

    // Wybór losowego drzewa na podstawie prawdopodobieñstwa
    private GameObject SelectRandomTree()
    {
        float totalProbability = 0;
        foreach (TreeType tree in treeTypes)
        {
            totalProbability += tree.probability;
        }

        float randomPoint = Random.Range(0, totalProbability);
        float currentProbability = 0;

        foreach (TreeType tree in treeTypes)
        {
            currentProbability += tree.probability;
            if (randomPoint <= currentProbability)
            {
                return tree.prefab;
            }
        }

        return treeTypes[0].prefab; // Awaryjnie zwracamy pierwsze drzewo
    }
}