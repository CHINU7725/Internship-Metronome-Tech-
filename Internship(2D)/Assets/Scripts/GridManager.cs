using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager2D : MonoBehaviour
{
    public GameObject prefab;
    public GameObject[] childPrefabs;
    public int rows = 3;
    public int columns = 3;
    public Color targetColor = Color.red;
    public float colorTransitionSpeed = 1.0f;

    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private List<GameObject> coloredObjects = new List<GameObject>();

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        ClearGrid();

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector2 position = new Vector2(i, j);
                GameObject newPrefab = Instantiate(prefab, position, Quaternion.identity);
                instantiatedObjects.Add(newPrefab);
                AddChildObject(newPrefab);
            }
        }
    }

    void AddChildObject(GameObject parent)
    {
        int randomIndex = Random.Range(0, childPrefabs.Length);
        GameObject child = Instantiate(childPrefabs[randomIndex], parent.transform);
        float randomScale = Random.Range(0.5f, 2f);
        child.transform.localScale = Vector3.one * randomScale;
        if (child.transform.localScale.y > parent.transform.localScale.y)
        {
            StartCoroutine(LerpColor(parent.GetComponent<SpriteRenderer>(), targetColor, parent, child));
        }
    }
    IEnumerator LerpColor(SpriteRenderer spriteRenderer, Color targetColor, GameObject parent, GameObject child)
    {
        Color startColor = spriteRenderer.color;
        float elapsedTime = 0f;

        while (elapsedTime < 1.0f)
        {
            elapsedTime += Time.deltaTime * colorTransitionSpeed;
            spriteRenderer.color = Color.Lerp(startColor, targetColor, elapsedTime);
            yield return null;
        }
        coloredObjects.Add(parent);
        CompareAndDestroy(child);
    }
    void CompareAndDestroy(GameObject currentChild)
    {
        for (int i = coloredObjects.Count - 1; i >= 0; i--)
        {
            GameObject obj = coloredObjects[i];
            if (obj.transform.childCount > 0)
            {
                GameObject objChild = obj.transform.GetChild(0).gameObject;
                if (objChild.transform.localScale.y < currentChild.transform.localScale.y)
                {
                    Destroy(objChild);
                    coloredObjects.RemoveAt(i);
                }
            }
        }
    }

    public void ClearGrid()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();
        coloredObjects.Clear();
    }

    public void RegenerateGrid()
    {
        ClearGrid();
        CreateGrid();
    }
}
