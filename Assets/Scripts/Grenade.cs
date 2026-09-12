using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public GameObject mesh;
    public GameObject effect;
    public Rigidbody rigid;

    void Start()
    {
        StartCoroutine(ExplosionRoutine());
    }

    IEnumerator ExplosionRoutine()
    {
        yield return new WaitForSeconds(3f);

        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        mesh.SetActive(false);
        effect.SetActive(true);


        RaycastHit[] rayhit = Physics.SphereCastAll(transform.position, 15, Vector3.up, 0f, LayerMask.GetMask("Enemy"));
        foreach (RaycastHit hitObj in rayhit)
            hitObj.transform.GetComponent<Enemy>().OnDamaged(100, transform.position);

        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }
}
