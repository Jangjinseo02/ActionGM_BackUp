using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orbit : MonoBehaviour
{
    public Transform target;
    public float speed;
    Vector3 offset;

    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
        transform.RotateAround(target.position, Vector3.up, speed * Time.deltaTime);
        
    }
}
