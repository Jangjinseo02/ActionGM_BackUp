using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    Rigidbody rigid;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }

    public void Shot(Vector3 bulletPos)
    {
        rigid.velocity = bulletPos * 50;
    }
    public void Case(Vector3 casePos)
    {
        rigid.AddForce(casePos * Random.Range(1, 3) * (-1) + Vector3.up * 5, ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
            Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<Enemy>())
            other.gameObject.GetComponent<Enemy>().OnDamaged(damage, transform.position);
        else if(other.gameObject.GetComponent<Player>())
            other.gameObject.GetComponent<Player>().OnDamaged(damage, transform.position);

        if(!isMelee)
            Destroy(gameObject);            
    }
}
