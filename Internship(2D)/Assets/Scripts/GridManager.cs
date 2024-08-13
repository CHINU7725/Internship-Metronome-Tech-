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

    // List to store objects whose child has changed color
    private List<GameObject> coloredObjects = new List<GameObject>();

    void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector2 position = new Vector2(i, j);
                GameObject newPrefab = Instantiate(prefab, position, Quaternion.identity);
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

        // If child is larger than the parent, start the color change
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

        // After color transition, add the object to the list of colored objects
        coloredObjects.Add(parent);

        // Destroy children of objects whose child size is less than the child of this object
        CompareAndDestroy(child);
    }

    void CompareAndDestroy(GameObject currentChild)
    {
        foreach (GameObject obj in coloredObjects)
        {
            if (obj != currentChild.transform.parent)
            {
                GameObject objChild = obj.transform.GetChild(0).gameObject;
                if (objChild.transform.localScale.y < currentChild.transform.localScale.y)
                {
                    Destroy(objChild);
                }
            }
        }
    }
}
