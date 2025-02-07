using UnityEngine;
using Photon.Pun;
using static ArtConfigs;


// For Melee Attack
[RequireComponent(typeof(PhotonView))]
public class Blades : Items, IPunObservable
{
    void Awake()
    {
        gameObject.tag = "Proj";
    }

    // Sync
    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);
    }

    // Damage Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Monster"))
        {
            Monsters monster = other.gameObject.GetComponent<Monsters>();
            if (monster != null && monster.gameObject.activeSelf)
            {
                GameObject animObject = Instantiate(AnimConfigs.Instance.GetAnim(hitAnim), Vector3.zero, Quaternion.identity);
                Vector3 pos = new Vector3(monster.transform.position.x, monster.transform.position.y + 4, monster.transform.position.z - 1.5f);
                animObject.transform.position = pos;
                animObject.transform.localRotation = Quaternion.Euler(45, 0, 0);
                animObject.transform.localScale = new Vector3(damageRange, damageRange, damageRange);
                if (PhotonNetwork.IsConnected)
                {
                    photonView.RPC("RPCPlayHitAnim", RpcTarget.Others, hitAnim, pos, damageRange);
                }
                if (!AOE)
                {
                    if (PhotonNetwork.IsConnected)
                    {
                        photonView.RPC("RPCDamageToMonster", RpcTarget.All, monster.photonView.ViewID, damage, isMagic);
                    }
                    else
                    {
                        monster.TakeDamage(damage, isMagic);
                    }
                    GameManager.Instance.monsterManager.despawnCheck(monster);
                }
                else
                {
                    // AOE
                    Explosions explosion = Instantiate(PrefabManager.Instance.ExplosionPrefab, transform.position, Quaternion.identity).GetComponent<Explosions>();
                    explosion.Initialize(damageRange, damage, pen, isMagic);
                }
            }
        }
    }

    [PunRPC]
    private void RPCDamageToMonster(int monsterViewID, float damage, bool isMagic)
    {
        Monsters monster = PhotonView.Find(monsterViewID)?.GetComponent<Monsters>();
        if (monster != null)
        {
            monster.TakeDamage(damage, isMagic);
            GameManager.Instance.monsterManager.despawnCheck(monster);
        }
    }

    [PunRPC]
    public void RPCPlayHitAnim(int id, Vector3 pos, float scale)
    {
        GameObject animObject = Instantiate(AnimConfigs.Instance.GetAnim(id), Vector3.zero, Quaternion.identity);
        animObject.transform.position = pos;
        animObject.transform.localRotation = Quaternion.Euler(45, 0, 0);
        animObject.transform.localScale = new Vector3(scale, scale, scale);
    }
}
