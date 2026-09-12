using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        rigid.velocity = transform.forward * 10;
    }

    void Update()
    {
        transform.Rotate(Vector3.forward * 5);
    }

    private void OnTriggerEnter(Collider other)
    {

        //Destroy(gameObject);
    }

}
