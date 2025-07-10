using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletSize = 10;


    [SerializeField] private GameObject bulletFxPrefab;
    [SerializeField] private int bulletFxSize = 10;

    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    private void Start()
    {
        InitialPool(bulletPrefab, bulletSize);
        InitialPool(bulletFxPrefab, bulletFxSize);
    }

    public GameObject GetObject(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            InitialPool(prefab, 10);

        if (poolDictionary[prefab].Count == 0)
            CreateNewObject(prefab);

        GameObject objectToGet = poolDictionary[prefab].Dequeue();
        objectToGet.SetActive(true);
        objectToGet.transform.parent = null;
        return objectToGet;
    }

    public void ReturnObject(GameObject objToReturn, float delay = 0f) => StartCoroutine(DelayReturn(objToReturn, delay));

    private IEnumerator DelayReturn(GameObject objToReturn, float delay)
    {
        yield return new WaitForSeconds(delay);

        ReturnToPool(objToReturn);
    }

    private void ReturnToPool(GameObject objToReturn)
    {
        objToReturn.SetActive(false);
        objToReturn.transform.parent = transform;

        poolDictionary[objToReturn.GetComponent<PooledObject>().originalPrefab].Enqueue(objToReturn);
    }

    private void InitialPool(GameObject prefab, int size)
    {
        poolDictionary[prefab] = new Queue<GameObject>();

        for (int i = 0; i < size; i++)
        {
            CreateNewObject(prefab);
        }
    }

    private void CreateNewObject(GameObject prefab)
    {
        GameObject newObject = Instantiate(prefab, transform);
        newObject.AddComponent<PooledObject>().originalPrefab = prefab;

        newObject.SetActive(false);
        poolDictionary[prefab].Enqueue(newObject);
    }
}
