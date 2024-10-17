using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_WeaponController : MonoBehaviour
{
    [SerializeField] private LayerMask whatIsAlly;
    [Space]
    private Player player;
    private const float REFERENCE_BULLET_SPEED = 20;
    //This is the default speed from whcih our mass formula is derived.

    [SerializeField] private List<Weapon_Data> defaultWeaponData;
    [SerializeField] private Weapon currentWeapon;
    int currentWeaponIndex;
    private bool weaponReady;
    private bool isShooting;

    [Header("Bullet details")]
    [SerializeField] private float bulletImpactForce = 100;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed;


    [SerializeField] private Transform weaponHolder;

    [Header("Inventory")]

    [SerializeField] private int maxSlots = 4;
    [SerializeField] private List<Weapon> weaponSlots;

    [SerializeField] private GameObject weaponPickupPrefab;

    private void Start()
    {
        player = GetComponent<Player>();
        AssignInputEvents();
    }

    private void Update()
    {
        if (isShooting)
            Shoot();
    }

    #region Slots managment - Pickup\Equip\Drop\Ready Weapon

    public void SetDefaultWeapon(List<Weapon_Data> newWeaponData)
    {
        defaultWeaponData = new List<Weapon_Data>(newWeaponData);
        weaponSlots.Clear();

        foreach(Weapon_Data weaponData in defaultWeaponData)
        {
            PickupWeapon(new Weapon(weaponData));
        }

        EquipWeapon(0);
    }
    private void EquipWeapon(int i)
    {
        if (i >= weaponSlots.Count)
            return;

        SetWeaponReady(false);

        currentWeapon = weaponSlots[i];
        player.weaponVisuals.PlayWeaponEquipAnimation();

        //CameraManager.instance.ChangeCameraDistance(currentWeapon.cameraDistance);

        UpdateWeaponUI();
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (WeaponInSlots(newWeapon.weaponType) != null)
        {
            WeaponInSlots(newWeapon.weaponType).totalReserveAmmo += newWeapon.bulletsInMagazine;
            return;
        }

        if (weaponSlots.Count >= maxSlots && newWeapon.weaponType != currentWeapon.weaponType)
        {
            int weaponIndex = weaponSlots.IndexOf(currentWeapon);

            player.weaponVisuals.SwitchOffWeaponModels();
            weaponSlots[weaponIndex] = newWeapon;

            CreateWeaponOnTheGround();
            EquipWeapon(weaponIndex);
            return;
        }

        weaponSlots.Add(newWeapon);
        player.weaponVisuals.SwitchOnBackupWeaponModel();

        UpdateWeaponUI();
    }
    private void DropWeapon()
    {
        if (HasOnlyOneWeapon())
            return;


        CreateWeaponOnTheGround();

        weaponSlots.Remove(currentWeapon);
        EquipWeapon(0);
    }

    private void CreateWeaponOnTheGround()
    {
        GameObject droppedWeapon = ObjectPool.instance.GetObject(weaponPickupPrefab, transform);
        droppedWeapon.GetComponent<Pickup_Weapon>()?.SetupPickupWeapon(currentWeapon, transform);
    }

    public void SetWeaponReady(bool ready)
    {
        weaponReady = ready;

        if(ready)
            player.sound.weaponReady.Play();
    }
    public bool WeaponReady() => weaponReady;

    #endregion


    public void UpdateWeaponUI()
    {
        UI.instance.inGameUI.UpdateWeaponUI(weaponSlots, currentWeapon);
    }

    private IEnumerator BurstFire()
    {
        SetWeaponReady(false);

        for (int i = 1; i <= currentWeapon.bulletsPerShot; i++)
        {
            FireSingleBullet();

            yield return new WaitForSeconds(currentWeapon.burstFireDelay);

            if (i >= currentWeapon.bulletsPerShot)
                SetWeaponReady(true);
        }
    }

    private void Shoot()
    {
        if (WeaponReady() == false)
            return;

        if (currentWeapon.CanShoot() == false)
            return;

        player.weaponVisuals.PlayFireAnimation();

        if (currentWeapon.shootType == ShootType.Single)
            isShooting = false;

        if (currentWeapon.BurstActivated() == true)
        {
            StartCoroutine(BurstFire());
            return;
        }


        FireSingleBullet();
        TriggerEnemyDodge();
    }

    private void FireSingleBullet()
    {
        currentWeapon.bulletsInMagazine--;
        UpdateWeaponUI();

        player.weaponVisuals.CurrentWeaponModel().fireSFX.Play();


        GameObject newBullet = ObjectPool.instance.GetObject(bulletPrefab,GunPoint());

        newBullet.transform.rotation = Quaternion.LookRotation(GunPoint().forward);

        Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        bulletScript.BulletSetup(whatIsAlly,currentWeapon.bulletDamage, currentWeapon.gunDistance,bulletImpactForce);


        Vector3 bulletsDirection = currentWeapon.ApplySpread(BulletDirection());

        rbNewBullet.mass = REFERENCE_BULLET_SPEED / bulletSpeed;
        rbNewBullet.linearVelocity = bulletsDirection * bulletSpeed;

        if ( currentWeapon.bulletsInMagazine <= 0 && currentWeapon.totalReserveAmmo > 0)
            Reload(); 
    }

    private void Reload()
    {
        SetWeaponReady(false);
        player.weaponVisuals.PlayReloadAnimation();

        player.weaponVisuals.CurrentWeaponModel().realodSfx.Play();
    }


    public Vector3 BulletDirection()
    {
        Transform aim = player.aim.Aim();

        Vector3 direction = (aim.position - GunPoint().position).normalized;

        if (player.aim.CanAimPrecisly() == false && player.aim.Target() == null)
            direction.y = 0;

        return direction;
    }

    public bool HasOnlyOneWeapon() => weaponSlots.Count <= 1;
    public Weapon WeaponInSlots(WeaponType weaponType)
    {
        foreach (Weapon weapon in weaponSlots)
        {
            if (weapon.weaponType == weaponType)
                return weapon;
        }

        return null;
    }
    public Weapon CurrentWeapon() => currentWeapon;
    public Transform GunPoint() => player.weaponVisuals.CurrentWeaponModel().gunPoint;

    private void TriggerEnemyDodge()
    {
        Vector3 rayOrigin = GunPoint().position;
        Vector3 rayDirection = BulletDirection();

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, Mathf.Infinity))
        {
            Enemy_Melee enemy_Melee = hit.collider.gameObject.GetComponentInParent<Enemy_Melee>();

            if (enemy_Melee != null)
                enemy_Melee.ActivateDodgeRoll();
        }
    }

    private void SwitchToNextWeapon() {
        if (weaponSlots.Count <= 1)
            return;

        currentWeaponIndex = (currentWeaponIndex + 1) % weaponSlots.Count;
        EquipWeapon(currentWeaponIndex);

    }

    private void SwitchToPreviousWeapon() {
        if (weaponSlots.Count <= 1)
            return;

        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weaponSlots.Count - 1;

        EquipWeapon(currentWeaponIndex);
    }

    public bool HasEmptySlots() => weaponSlots.Count < maxSlots;


    #region Input Events

    private void AssignInputEvents()
    {
        InputSystem_Actions.CharacterActions controls = player.controls;

        controls.Fire.performed += context => isShooting = true;
        controls.Fire.canceled += context => isShooting = false;

        controls.Next.performed += context => SwitchToNextWeapon();
        controls.Previous.performed += context => SwitchToPreviousWeapon();

        // controls.Character.DropCurrentWeapon.performed += context => DropWeapon();

        controls.InteractReload.performed += context =>
        {
            if (currentWeapon.CanReload() && WeaponReady())
            {
                Reload();
            }
        };

        //controls.Character.ToogleWeaponMode.performed += context => currentWeapon.ToggleBurst();

    }



    #endregion
}
