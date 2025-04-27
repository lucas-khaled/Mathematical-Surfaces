using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject prefab;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();

    public List<GameObject> GetObjects(int count)
    {
        while (activeObjects.Count > count)
        {
            GameObject objToReturn = activeObjects[activeObjects.Count - 1];
            activeObjects.RemoveAt(activeObjects.Count - 1);
            ReturnObject(objToReturn);
        }

        while (activeObjects.Count < count)
        {
            if (pool.Count == 0)
            {
                GameObject newObj = Instantiate(prefab, transform);
                newObj.transform.SetParent(transform, false);
                newObj.SetActive(false);
                pool.Enqueue(newObj);
            }

            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            activeObjects.Add(obj);
        }

        return new List<GameObject>(activeObjects);
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public void ReturnAll()
    {
        foreach (var obj in activeObjects)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
        activeObjects.Clear();
    }
}
