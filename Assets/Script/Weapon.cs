using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
public class Weapon : MonoBehaviour
{
    public Camera playerCamera;
    public GameObject bulletPrefab;
    public ObjectPooling bulletPool;
    public Transform firePoint;
    //  viên đạn
    public int weaponDamage = 25;
    public float bulletSpeed = 20f;
    public float bulletLifetime = 2f;
    public float fireRate = 0.5f;
    private Vector3 targetPoint;
    public float shootingDelay = 2f;
    public bool isShooting, readyToShoot;
    bool allowedToShoot = true;
    //Số viên đạn trong mỗi loạt bắn
    public int burstCount = 3;
    public int burstBulletsLeft;
    //Phân tán đạn
    public float spreadAngle = 5f;
    // Chế độ bắn
    public enum FireMode { Single, Burst, Automatic }
    public FireMode fireMode = FireMode.Single;
    // Hiệu ứng muzzle flash
    public GameObject muzzleFlashEffect;
    public Animator animator;
    //Thay đạn
    public float reloadTime = 1.5f;
    public int magazineSize, bulletsLeft;
    public bool isReloading = false;
    //UI
    public Text ammoDisplay;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        burstBulletsLeft = burstCount;
        readyToShoot = true;
        animator = GetComponent<Animator>();
        bulletsLeft = magazineSize;
    }

    // Update is called once per frame
    void Update()
    {
        bool isMobile = InputModeManager.Instance != null && InputModeManager.Instance.IsMobile;

        // PC mode: bắn bằng chuột như code cũ
        if (!isMobile)
        {
            if (fireMode == FireMode.Automatic)
            {
                isShooting = Input.GetKey(KeyCode.Mouse0);
            }
            else if (fireMode == FireMode.Burst || fireMode == FireMode.Single)
            {
                isShooting = Input.GetKeyDown(KeyCode.Mouse0);
            }

            if (isShooting)
            {
                TryShoot();
            }
        }
        else
        {
            // Mobile mode: chỉ bắn khi nút bắn Mobile báo đang giữ (isShooting)
            if (isShooting)
            {
                TryShoot();
            }
        }
        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < magazineSize && !isReloading)
        {
            Reload();
        }
        if (bulletsLeft <= 0)
        {
            Reload();
        }

        if (ammoDisplay != null)
        {
            ammoDisplay.text = bulletsLeft + " / " + magazineSize;
        }
    }

    // Gọi từ nút bắn Mobile (UI Button OnClick)
    public void OnMobileFireButtonPressed()
    {
        // Chỉ xử lý khi đang ở chế độ Mobile
        if (InputModeManager.Instance != null && !InputModeManager.Instance.IsMobile)
            return;

        TryShoot();
    }

    // Gọi khi giữ nút bắn Mobile (Pointer Down)
    public void OnMobileFireButtonDown()
    {
        if (InputModeManager.Instance != null && !InputModeManager.Instance.IsMobile)
            return;

        isShooting = true;
    }

    // Gọi khi nhả nút bắn Mobile (Pointer Up)
    public void OnMobileFireButtonUp()
    {
        if (InputModeManager.Instance != null && !InputModeManager.Instance.IsMobile)
            return;

        isShooting = false;
    }

    // Gọi từ nút Reload Mobile (UI Button OnClick)
    public void OnMobileReloadButtonPressed()
    {
        // Chỉ xử lý khi đang ở chế độ Mobile
        if (InputModeManager.Instance != null && !InputModeManager.Instance.IsMobile)
            return;

        if (bulletsLeft < magazineSize && !isReloading)
        {
            Reload();
        }
    }

    private void TryShoot()
    {
        if (!readyToShoot || !allowedToShoot || isReloading)
            return;

        if (fireMode == FireMode.Single)
        {
            Fire();
            readyToShoot = false;
            Invoke("ResetShot", fireRate);
        }
        else if (fireMode == FireMode.Burst)
        {
            burstBulletsLeft = burstCount;
            StartCoroutine(FireBurst());
            readyToShoot = false;
            Invoke("ResetShot", fireRate * burstCount);
        }
        else if (fireMode == FireMode.Automatic)
        {
            Fire();
            readyToShoot = false;
            Invoke("ResetShot", fireRate);
        }
    }

    void Fire()
    {
        bulletsLeft--;
        // Hiệu ứng muzzle flash
        muzzleFlashEffect.GetComponent<ParticleSystem>().Play();
        
        // Force animation restart by crossfading to shoot state immediately
        animator.CrossFadeInFixedTime("handgun_combat_shoot", 0f);
        SoundManager.Instance.PlayGunShot();

        Vector3 shootDirection = GetSpreadDirection().normalized;
        Quaternion shootRotation = Quaternion.LookRotation(shootDirection);

        GameObject bullet = null;
        if (bulletPool != null)
        {
            bullet = bulletPool.SpawnBullet(firePoint.position, shootRotation, shootDirection * bulletSpeed);
        }
        else if (bulletPrefab != null)
        {
            bullet = Instantiate(bulletPrefab, firePoint.position, shootRotation);
        }

        if (bullet == null)
        {
            return;
        }

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.bulletDamage = weaponDamage;
        }

        var pooled = bullet.GetComponent<PooledBullet>();
        if (pooled != null)
        {
            pooled.lifeTime = bulletLifetime;
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null && bulletPool == null)
        {
            rb.AddForce(shootDirection * bulletSpeed, ForceMode.Impulse);
        }

        // Chỉ Destroy nếu KHÔNG có PooledBullet (pool không được dùng)
        if (pooled == null && bulletPool == null)
        {
            Destroy(bullet, bulletLifetime);
        }
    }

    private void Reload()
    {
        if (isReloading || bulletsLeft == magazineSize) return;

        isReloading = true;
        animator.SetTrigger("Reload");
        SoundManager.Instance.PlayReload();
    }

    private void FinishReload()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
    }

    System.Collections.IEnumerator FireBurst()
    {
        for (int i = 0; i < burstCount; i++)
        {
            Fire();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowedToShoot = true;
    }

    private void ResetBurst()
    {
        allowedToShoot = true;
        burstBulletsLeft = burstCount;
    }

    Vector3 GetSpreadDirection()
    {
        // Xác định điểm trên màn hình để bắn
        // PC: dùng vị trí chuột
        // Mobile: dùng tâm màn hình (trùng hồng tâm UI ở giữa)
        Vector3 screenPoint;
        bool isMobile = InputModeManager.Instance != null && InputModeManager.Instance.IsMobile;

        if (isMobile)
        {
            screenPoint = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        }
        else
        {
            screenPoint = Input.mousePosition;
        }

        Ray ray = playerCamera.ScreenPointToRay(screenPoint);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }
        Vector3 direction = targetPoint - firePoint.position;
        float spreadX = Random.Range(-spreadAngle, spreadAngle);
        float spreadY = Random.Range(-spreadAngle, spreadAngle);
        return direction + new Vector3(spreadX, spreadY, 0);
    }

}
