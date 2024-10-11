using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    private Player player;

    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private Transform spawnPoint;   

    private void Start()
    {
        player = GetComponent<Player>();
        player.controls.Character.Fire.performed += context => Shoot();
    }

    private void Shoot()
    {
        GameObject bulletInstance = Instantiate(bullet, spawnPoint.position, Quaternion.LookRotation(spawnPoint.forward));
        bulletInstance.GetComponent<Rigidbody>().linearVelocity = (spawnPoint.forward * bulletSpeed);
        Destroy(bulletInstance, 10f);

        GetComponentInChildren<Animator>().SetTrigger("Fire");
    }
}
