using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform spawnPoint;   

    [SerializeField] private Transform weaponHolder;

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject bulletInstance = Instantiate(bullet, spawnPoint.position, Quaternion.LookRotation(spawnPoint.forward));
        bulletInstance.GetComponent<Rigidbody>().linearVelocity = BulletDirection() * bulletSpeed;
        Destroy(bulletInstance, 10f);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }

    public Transform GunPoint() => spawnPoint;

    public Vector3 BulletDirection() {
        Transform aimTarget = player.aim.Aim();
        Vector3 direction =  (aimTarget.position - spawnPoint.position).normalized;

        if ( player.aim.CanAimPrecisely() == false && player.aim.Target() == null )
            direction.y = 0;

        // To do - fix for reload etc
        //weaponHolder.LookAt(aimTarget);
        //spawnPoint.LookAt(aimTarget);
        
        return direction;
    }

}
