using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    [Header("Bullet Pool")]
    [Tooltip("Prefab of the bullet to pool.")]
    public GameObject bulletPrefab;

    [Tooltip("Number of bullets pre-instantiated at start.")]
    public int initialSize = 20;

    [Tooltip("Allow pool to expand when empty.")]
    public bool allowExpand = true;

    [Tooltip("Optional parent to keep pooled instances organized.")]
    public Transform poolParent;

    private readonly Queue<GameObject> _pool = new Queue<GameObject>();

    void Awake()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("ObjectPooling: Bullet prefab is not assigned.");
            return;
        }

        for (int i = 0; i < Mathf.Max(0, initialSize); i++)
        {
            var obj = CreateNewBullet();
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }

    private GameObject CreateNewBullet()
    {
        var obj = Instantiate(bulletPrefab, poolParent);

        var pooled = obj.GetComponent<PooledBullet>();
        if (pooled == null)
        {
            pooled = obj.AddComponent<PooledBullet>();
        }
        pooled.SetPool(this);
        return obj;
    }

    /// <summary>
    /// Spawns a bullet from the pool at position/rotation with an optional initial velocity.
    /// </summary>
    public GameObject SpawnBullet(Vector3 position, Quaternion rotation, Vector3 velocity)
    {
        GameObject obj = null;

        if (_pool.Count > 0)
        {
            obj = _pool.Dequeue();
        }

        if (obj == null)
        {
            if (!allowExpand)
            {
                Debug.LogWarning("ObjectPooling: Pool empty and expansion disabled.");
                return null;
            }
            obj = CreateNewBullet();
        }

        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = velocity;
        }

        return obj;
    }

    /// <summary>
    /// Returns a bullet instance back to the pool.
    /// </summary>
    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;

        var rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        obj.SetActive(false);
        if (poolParent != null)
        {
            obj.transform.SetParent(poolParent);
        }
        _pool.Enqueue(obj);
    }
}
