using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamaged
{
    public enum Type { A, B, C };
    public Type type;

    public int maxHealth;
    public int curHealth;

    public float targetRadius;
    public float targetRange;

    public Transform target;
    public GameObject meleeSize;
    public GameObject missile;
    
    bool isChase;
    bool isAttack;
    bool isDead;
    
    Animator anim;
    Rigidbody rigid;
    MeshRenderer mat;
    NavMeshAgent nav;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        mat = GetComponentInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        Invoke("StartSetting", 3f);
    }

    void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
           
        
    }

    void FixedUpdate()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
        

        Search();
    }

    void StartSetting()
    {
        curHealth = maxHealth;
        isDead = false;
        if (target != null)
            isChase = true;
        nav.enabled = true;
        nav.speed = 10f;
        anim.SetBool("IsWalk", true);
    }

    void Search()
    {
        RaycastHit[] rayhit = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));

        if (rayhit.Length > 0 && !isAttack)
        {
            rigid.velocity = Vector3.zero;
            StartCoroutine("AttackRoutine");
        }
    }
    IEnumerator AttackRoutine()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("IsAttack", true);

        switch (type)
        {
            case Type.A:
                meleeSize.SetActive(true);
                yield return new WaitForSeconds(1f);

                meleeSize.SetActive(false);
                yield return new WaitForSeconds(1f);
                break;
            case Type.B:
                yield return new WaitForSeconds(0.1f);
                rigid.AddForce(transform.forward * 30, ForceMode.Impulse);
                meleeSize.SetActive(true);
                yield return new WaitForSeconds(0.5f);

                rigid.velocity = Vector3.zero;

                yield return new WaitForSeconds(0.5f);
                meleeSize.SetActive(false);
                yield return new WaitForSeconds(1.5f);
                break;  
            case Type.C:
                yield return new WaitForSeconds(0.1f);
                Instantiate(missile, new Vector3(transform.position.x, 2, transform.position.z) , transform.rotation);
                break;
        }
        yield return new WaitForSeconds(2f);
        isChase = true;
        isAttack = false;
        anim.SetBool("IsAttack", false);
    }

    public void OnDamaged(int damage, Vector3 other)
    {
        if (isDead)
            return;

        StopCoroutine("PeakRoutine");
        curHealth -= damage;

        StartCoroutine(PeakRoutine(other));

    }

    IEnumerator PeakRoutine(Vector3 other)
    {
        if (curHealth <= 0)
        {
            isChase = false;
            isDead = true;
            nav.enabled = false;
            gameObject.layer = 11;
            mat.material.color = new Color(0.5f,0.5f,0.5f, 0.5f);
            rigid.AddForce((transform.position - other).normalized * 10 + Vector3.up * 10, ForceMode.Impulse);
            anim.SetTrigger("doDie");


            yield return new WaitForSeconds(2f);

            Destroy(gameObject);

            yield break;
        }

        mat.material.color = Color.green;

        yield return new WaitForSeconds(0.4f);
        
        mat.material.color = Color.white;        
    }
}
