using UnityEngine;

public class ForestSpawner : MonoBehaviour
{
    [System.Serializable] // Pozwala ustawi� warto�ci w Inspectorze
    public class TreeType
    {
        public GameObject prefab; // Prefab drzewa
        public float probability; // Jak cz�sto wyst�puje
    }

    public TreeType[] treeTypes; // Lista r�nych drzew
    public int treeCount = 500;
    public float areaSize = 100f;

    public LayerMask groundLayer; // Warstwa terenu, aby poprawnie ustawia� drzewa

    void Start()
    {
        for (int i = 0; i < treeCount; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(-areaSize / 2, areaSize / 2),
                50, // Wysoko�� pocz�tkowa (b�dziemy raycastowa� w d�)
                Random.Range(-areaSize / 2, areaSize / 2)
            );

            // Raycast w d�, aby znale�� dok�adn� wysoko�� gruntu
            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, 100, groundLayer))
            {
                position.y = hit.point.y; // Ustaw wysoko�� drzewa na poziomie gruntu

                GameObject selectedTree = SelectRandomTree(); // Wyb�r losowego drzewa

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

    // Wyb�r losowego drzewa na podstawie prawdopodobie�stwa
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