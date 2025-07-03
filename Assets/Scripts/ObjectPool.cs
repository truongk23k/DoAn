using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int bulletSize = 10;

    private Queue<GameObject> bulletPool;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(instance.gameObject);
    }

    private void Start()
    {
        bulletPool = new Queue<GameObject>();

        CreateInitialPool();
    }

    public GameObject GetBullet()
    {
        if (bulletPool.Count == 0)
            CreateBullet();

        GameObject bulletToGet = bulletPool.Dequeue();
        bulletToGet.SetActive(true);
        bulletToGet.transform.parent = null;
        return bulletToGet;
    }

    public void ReturnBullet(GameObject bullet)
    {
        bullet.SetActive(false);
        bullet.transform.parent = transform;
        bulletPool.Enqueue(bullet);
    }

    private void CreateInitialPool()
    {
        for (int i = 0; i < bulletSize; i++)
        {
            CreateBullet();
        }
    }

    private void CreateBullet()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform);
        newBullet.SetActive(false);
        bulletPool.Enqueue(newBullet);
    }
}
