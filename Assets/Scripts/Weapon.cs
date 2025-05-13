using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Camera playerCamera;

    //shooting
    public bool isShooting,readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    //burst
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    //spread
    public float spreadIntensity;

    //bullet
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 30f;
    public float bulletPrefabLifeTime = 3f;

    public enum shootingMode
    {
        Single,Burst,Auto
    }

    public shootingMode currentShootingMode;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
    } 

    // Update is called once per frame
    void Update()
    {
        if(currentShootingMode == shootingMode.Auto)
        {
            //holding left click
            isShooting = Input.GetKey(KeyCode.Mouse0);
        }
        else if(currentShootingMode == shootingMode.Single || 
                currentShootingMode == shootingMode.Burst)
        {
            //click left click once
            isShooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if(readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        readyToShoot = false;

        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        //Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab,bulletSpawn.position,Quaternion.identity);
        
        //bullet to shooting direction
        bullet.transform.forward = shootingDirection;
        
        // shoot the bullet
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);
        
        //destroy bullet after some time
        StartCoroutine(DestroyBulletAfterTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        //burst mode
        if (currentShootingMode == shootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }

    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    public Vector3 CalculateDirectionAndSpread()
    {
        //shot from midle of screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f,0.5f,0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
    
        //return shooting directions and spread
        return direction + new Vector3(x,y,0);

    }

    private IEnumerator DestroyBulletAfterTime(GameObject bullet, float bulletPrefabLifeTime)
    {
        yield return new WaitForSeconds(bulletPrefabLifeTime);
        Destroy(bullet);
    }
}
