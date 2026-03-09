using UnityEngine;

public class PooledBullet : MonoBehaviour
{
    [Tooltip("Seconds before the bullet auto-returns to pool.")]
    public float lifeTime = 3f;

    private float _expireAt;
    private ObjectPooling _pool;
    private Rigidbody _rb;

    public void SetPool(ObjectPooling pool)
    {
        _pool = pool;
    }

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        _expireAt = Time.time + lifeTime;
    }

    void Update()
    {
        if (Time.time >= _expireAt)
        {
            ReturnToPool();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_rb != null)
        {
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
        }

        if (_pool != null)
        {
            _pool.ReturnToPool(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
