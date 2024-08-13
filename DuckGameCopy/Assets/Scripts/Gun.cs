using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Animator gunAnimator;

    [Header("Weapon")]
    public Transform firePoint;
    public Transform rotatePoint;
    public GameObject bulletPrefab;

    [Header("Firing")]
    public float gunCooldown = 0.3f;
    public float cooldownCounter = 0;

    public int ammo = 9;


    // Update is called once per frame
    void Update()
    {
        //Shooting
        if (Input.GetKeyDown(KeyCode.H) && cooldownCounter <= 0 && ammo > 0)
        {
            Shoot();
            cooldownCounter = gunCooldown;
        }

        if(cooldownCounter > 0)
        {
            cooldownCounter -= Time.deltaTime;
        }
    }
    void Shoot()
    {
        gunAnimator.SetBool("SHOOT", false);
        Instantiate(bulletPrefab, firePoint.position, rotatePoint.rotation);
        gunAnimator.SetBool("SHOOT", true);
        ammo--;
    }

    void NoShoot()
    {
        gunAnimator.SetBool("SHOOT", false);
    }
}
