using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum Type { Melee, Range};
    public Type type;
    public int damage;
    public float rate;
    public int curAmmo;
    public int maxAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailEffect;

    public Transform bulletPos;
    public GameObject bulletPrefab;
    public Transform casePos;
    public GameObject casePrefab;

    public void Use()
    {
        if(type == Type.Melee)
            StartCoroutine("Swing");
        else if(type == Type.Range && IsAmmo())
        {
            curAmmo -= 1;
            StartCoroutine("Shot");
        }
            
    }

    IEnumerator Swing()
    {
        if (type != Type.Melee)
            yield break;

        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;
        trailEffect.enabled = true;

        yield return new WaitForSeconds(0.3f);

        meleeArea.enabled = false;
        yield return new WaitForSeconds(0.3f);
        trailEffect.enabled = false;
    }

    IEnumerator Shot()
    {
        yield return new WaitForSeconds(0.2f);
        GameObject bulletInstant = Instantiate(bulletPrefab, bulletPos.position, bulletPos.rotation);
        bulletInstant.GetComponent<Bullet>().Shot(bulletPos.forward);
        yield return null;

        GameObject caseInstant = Instantiate(casePrefab, casePos.position, casePos.rotation);
        caseInstant.GetComponent<Bullet>().Case(casePos.forward);
    }

    bool IsAmmo()
    {
        if (curAmmo > 0)
            return true;

        return false;
    }

    public int Reload(int ammo)
    {
        int addAmmo = ammo < maxAmmo ? ammo : maxAmmo;
        curAmmo += addAmmo;
        return addAmmo;
    }

    void OnTriggerEnter(Collider other)
    {
        if(type == Type.Melee && other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Enemy>().OnDamaged(damage, transform.position);
        }
    }
}
