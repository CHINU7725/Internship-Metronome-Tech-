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
    public AudioSource audio;

    private List<GameObject> instantiatedObjects = new List<GameObject>();
    private List<GameObject> coloredObjects = new List<GameObject>();

    private GameObject largestColoredObject = null;

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
        UpdateLargestColoredObject(parent, child);
        CompareAndDestroyAdjacent(child);
    }

    void UpdateLargestColoredObject(GameObject parent, GameObject child)
    {
        if (largestColoredObject == null || child.transform.localScale.y > largestColoredObject.transform.GetChild(0).localScale.y)
        {
            largestColoredObject = parent;
        }
    }

    void CompareAndDestroyAdjacent(GameObject currentChild)
    {
        if (largestColoredObject == null) return;

        Vector2 largestPosition = largestColoredObject.transform.position;
        GameObject largestChild = largestColoredObject.transform.GetChild(0).gameObject;

        foreach (GameObject obj in coloredObjects)
        {
            if (obj != null && obj.transform.childCount > 0)
            {
                GameObject objChild = obj.transform.GetChild(0).gameObject;
                Vector2 objPosition = obj.transform.position;

                
                if (IsAdjacent(largestPosition, objPosition) && objChild.transform.localScale.y < currentChild.transform.localScale.y)
                {
                    Destroy(objChild);
                    RotateChildToCover(largestChild, largestPosition, objPosition);
                }
            }
        }
    }

    bool IsAdjacent(Vector2 pos1, Vector2 pos2)
    {
        return (Mathf.Abs(pos1.x - pos2.x) == 1 && pos1.y == pos2.y) || 
               (Mathf.Abs(pos1.y - pos2.y) == 1 && pos1.x == pos2.x);   
    }

    void RotateChildToCover(GameObject child, Vector2 largestPosition, Vector2 destroyedPosition)
    {
        Vector3 rotationAxis = Vector3.forward;
        Vector2 direction = destroyedPosition - largestPosition;

        if (direction.x == 1) 
        {
            child.transform.RotateAround(largestPosition, rotationAxis, -90f);
        }
        else if (direction.x == -1) 
        {
            child.transform.RotateAround(largestPosition, rotationAxis, 90f);
        }
        else if (direction.y == 1) 
        {
            child.transform.RotateAround(largestPosition, rotationAxis, 180f);
        }
        else if (direction.y == -1) 
        {
            
            child.transform.RotateAround(largestPosition, rotationAxis, 0f);
        }

        child.transform.position = (largestPosition + destroyedPosition) / 2;
    }

    public void ClearGrid()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj);
        }
        instantiatedObjects.Clear();
        coloredObjects.Clear();
        largestColoredObject = null;
    }

    public void RegenerateGrid()
    {
        audio.Play();
        ClearGrid();
        CreateGrid();
    }
}
