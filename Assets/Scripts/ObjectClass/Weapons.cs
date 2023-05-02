using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using static ConfigManager;

// Define Weapons
public class Weapons : DefaultObjects
{
    // Weapon Stats
    [SerializeField] protected string wpName = "DeaultWeapon";
    [SerializeField] protected int id = 0;

    [SerializeField] protected bool isBullet = true;
    [SerializeField] protected float attack = 10;
    [SerializeField] protected float pen = 0.1f;
    [SerializeField] protected float life = 6.0f;
    [SerializeField] protected float cd = 0.5f;
    [SerializeField] protected bool selfDet = false;
    [SerializeField] protected float projectileSpeed = 10f;
    [SerializeField] private float damageRange = 0.1f;
    [SerializeField] private bool aoe = false;

    public const string UPDATE_PROJ = "UpdatePosition";
    private float timer = 0;

    // Morph the weapon
    public void SetWeapons(WeaponConfig weaponConfigs)
    {
        id = weaponConfigs.id;
        wpName = weaponConfigs.name;
        isBullet = weaponConfigs.isBullet;
        attack = weaponConfigs.attack;
        pen = weaponConfigs.pen;
        life = weaponConfigs.life;
        cd = weaponConfigs.cd;
        selfDet = weaponConfigs.selfDet;
        projectileSpeed = weaponConfigs.projectileSpeed;
        aoe = weaponConfigs.aoe;
    }

    // Fire based on type
    public void Fire(int playerIdx, Vector3 direction)
    {
        if (timer < cd)
        {
            return;
        }
        if (isBullet)
        {
            FireBullet(playerIdx, direction);
        }
        timer = 0;
    }

    private void FireBullet(int playerIdx, Vector3 direction)
    {
        // Get projectile from pool
        Projectiles proj = GameManager.Instance.dataManager.TakeProjPool();
        if (proj != null)
        {
            int projectileID = proj.photonView.ViewID;
            Vector3 firePos = new Vector3(transform.position.x, 0.1f, transform.position.z);
            // Config the Projectile
            proj.transform.position = transform.position;
            proj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            proj.Damage = attack;
            proj.Owner = playerIdx;
            proj.Life = life;
            proj.SelfDet = true;
            proj.Player = true;
            proj.AOE = aoe;
            proj.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
            proj.Activate();

            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("SimulateProjectile", RpcTarget.Others, projectileID, photonView.ViewID, firePos, direction, projectileSpeed);
            }
        }
    }

    // Simulating a projectile
    [PunRPC]
    private void SimulateProjectile(int projectileID, int weaponViewID, Vector3 firePos, Vector3 direction, float speed)
    {
        // Get projectile from pool
        Projectiles proj = PhotonView.Find(projectileID).GetComponent<Projectiles>();
        Weapons weapon = PhotonView.Find(weaponViewID).GetComponent<Weapons>();

        if (proj != null)
        {
            Vector3 localFirePos = weapon.transform.position;
            // Config the Projectile
            proj.transform.position = localFirePos;
            proj.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
            proj.Damage = attack;
            proj.Owner = photonView.ViewID;
            proj.Life = life;
            proj.SelfDet = true;
            proj.Player = true;
            proj.AOE = aoe;
            proj.GetComponent<Rigidbody>().velocity = direction * speed;
        }
    }

    public void SetRotation(Vector3 targetPosition)
    {
        transform.LookAt(targetPosition);
    }

    public void SetRotation(Quaternion rotation)
    {
        transform.localRotation = rotation;
    }

    private void Update()
    {
        timer += Time.deltaTime;
    }

    // Class properties
    public string WpName
    {
        get { return wpName; }
        set { wpName = value; }
    }

    public int ID
    {
        get { return id; }
        set { id = value; }
    }

    public bool IsBullet
    {
        get { return isBullet; }
        set { isBullet = value; }
    }

    public float Attack
    {
        get { return attack; }
        set { attack = value; }
    }

    public float Pen
    {
        get { return pen; }
        set { pen = value; }
    }

    public float Life
    {
        get { return life; }
        set { life = value; }
    }

    public float ProjectileSpeed
    {
        get { return projectileSpeed; }
        set { projectileSpeed = value; }
    }

    public float CD
    {
        get { return cd; }
        set { cd = value; }
    }

    public bool SelfDet
    {
        get { return selfDet; }
        set { selfDet = value; }
    }

    public float DamageRange
    {
        get { return damageRange; }
        set { damageRange = value; }
    }
} 