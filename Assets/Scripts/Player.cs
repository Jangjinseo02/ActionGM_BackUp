using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamaged
{
    PlayerInput playerInput;
    Animator anim;
    Rigidbody rigid;
    MeshRenderer[] meshes;
    TrailRenderer dodgeEffect;
    ParticleSystem dodgeParticle;

    Vector3 moveVec;
    Vector3 dodgeVec;
    Vector3 bfVec;
    Vector3 nextVec;

    public float speed;
    public int jumpPower;

    [Header("------------------Weapon")]
    public bool[] hasWeapon;
    public GameObject[] weapons;
    GameObject nearWeapon;
    Weapon bfWeapon;
    int weaponIndex = -1;
    bool isSwap;
    float fireDelay;

    [Header("------------------Grenade")]
    public int hasGrenade = -1;
    public GameObject[] grenades;
    public GameObject grenadesGameObj;

    [Header("------------------Item")]
    public int health;
    public int ammo;
    public int coin;

    public int maxHealth;
    public int maxAmmo;
    public int maxCoin;
    public int maxGrenade;

    bool isDamage;
    bool isReload;
    bool isDodge;
    bool isJump;
    bool isWall;
    bool isFireReady;

    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        meshes = GetComponentsInChildren<MeshRenderer>();
        dodgeEffect = GetComponentInChildren<TrailRenderer>();
        dodgeParticle = GetComponentInChildren<ParticleSystem>();
    }

    void FixedUpdate()
    {
        Wall();
    }

    // Update is called once per frame
    void Update()
    {
        MouseClik();
        Move();
        Jump();
        Reload();
        Throw();
        Attack();
        Dodge();
        Swap();
    }

    void MouseClik()
    {
        if (playerInput.Fire || playerInput.Fire2)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayhit;
            if (Physics.Raycast(ray, out rayhit, 100))
            {
                nextVec = rayhit.point - transform.position;
                nextVec.y = 0;
                nextVec = nextVec.normalized;
            }
        }
    }

    void Wall()
    {
        Debug.DrawRay(transform.position, (moveVec) * (!isWall ? 5 : 10), Color.green);
        isWall = Physics.Raycast(transform.position, moveVec, !isWall ? 5 : 10, LayerMask.GetMask("Wall"));        
    }

    void Move()
    {           
        //������
        moveVec = new Vector3(playerInput.h, 0, playerInput.v).normalized;

        if (isDodge)
            moveVec = dodgeVec;
        if (isSwap)
            moveVec = Vector3.zero;

        if(!isWall)
            transform.position += moveVec * (playerInput.IsWalk ? speed * 0.3f : speed)* Time.deltaTime;

        //�ִϸ��̼�
        anim.SetBool("IsRun", playerInput.IsRun);
        anim.SetBool("IsWalk", playerInput.IsWalk);

        //ȸ��
        Rotate();

        //���� ���� �� ����
        bfVec = playerInput.Fire ? nextVec : moveVec;

        
        //transform.LookAt(transform.position + moveVec);
    }

    void Attack()
    {
        if (bfWeapon == null)
            return;

        fireDelay += Time.deltaTime;
        isFireReady = fireDelay > bfWeapon.rate;

        if (playerInput.Fire && isFireReady && !isSwap && !isReload)
        {
            bfWeapon.Use();
            anim.SetTrigger(bfWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
           
        }
            
    }
    void Reload()
    {
        if (!playerInput.Reload)
            return;
        if (bfWeapon != null && bfWeapon.type == Weapon.Type.Melee)
            return;

        isReload = true;

        anim.SetTrigger("doReload");
        StartCoroutine("ReloadRoutine");
    }

    IEnumerator ReloadRoutine()
    {
        yield return new WaitForSeconds(2f);
        ammo -= bfWeapon.Reload(ammo);
        isReload = false;
    }

    void Throw()
    {
        if (hasGrenade <= 0)
            return;
        if (!playerInput.Fire2 || isSwap || isReload)
            return;

        GameObject instantGrenade = Instantiate(grenadesGameObj, transform.position, transform.rotation);
        Rigidbody grenadeRigid = instantGrenade.GetComponent<Rigidbody>();
        nextVec.y = 2;
        grenadeRigid.AddForce(nextVec * 10, ForceMode.Impulse);
        grenadeRigid.AddTorque(Vector3.back * 10, ForceMode.Impulse);

        hasGrenade--;
        grenades[hasGrenade].SetActive(false);
    }

    void Rotate()
    {
        if (moveVec == Vector3.zero)
            moveVec = bfVec;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerInput.Fire ? nextVec : moveVec), Time.deltaTime * speed);

    }

    void Jump()
    {
        if (playerInput.Jump && !playerInput.IsRun && !isJump  && !isDodge && !isSwap)
        {
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            anim.SetBool("IsJump", true);
            anim.SetTrigger("doJump");
            isJump = true;
        }   
    }

    void Dodge()
    {
        if (playerInput.IsRun && playerInput.Jump && !isJump && !isDodge && !isSwap)
        {
            dodgeVec = playerInput.Fire ? nextVec : moveVec;
            
            speed *= 2;
            anim.SetTrigger("doDodge");
            isDodge = true;
            dodgeEffect.enabled = true;
            dodgeParticle.Play();
            
            Invoke("DodgeOut", 0.7f);
        }
    }
   

    void DodgeOut()
    {
        speed *= 0.5f;
        //dodgeEffect.enabled = false;
        dodgeParticle.Stop();
        isDodge = false;
    }

    void Swap()
    {
        if (playerInput.Swap1)
            weaponIndex = 0;
        if (playerInput.Swap2)
            weaponIndex = 1;
        if (playerInput.Swap3)
            weaponIndex = 2;

        if ((playerInput.Swap1 || playerInput.Swap2 || playerInput.Swap3) && hasWeapon[weaponIndex] && !isDodge && !isJump && !isReload)
        {
            if (bfWeapon == weapons[weaponIndex].GetComponent<Weapon>())
                return;

            if (bfWeapon != null)
                bfWeapon.gameObject.SetActive(false);

            isSwap = true;
            bfWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            bfWeapon.gameObject.SetActive(true);

            anim.SetTrigger("doSwap");
            Invoke("SwapOut", 0.4f);
        }
    }

    void SwapOut()
    {
        isSwap = false;
    }

    public void OnDamaged(int damage, Vector3 ent)
    {
        if (isDamage)
            return;

        health -= damage;
        StartCoroutine("DamageRoutine");
    }

    IEnumerator DamageRoutine()
    {
        isDamage = true;

        foreach (MeshRenderer meshs in meshes)
            meshs.material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

        yield return new WaitForSeconds(1f);

        isDamage = false;

        foreach (MeshRenderer meshs in meshes)
            meshs.material.color = new Color(1, 1, 1, 1);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            anim.SetBool("IsJump", false);
            isJump = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    ammo += item.value;
                    if(ammo >= maxAmmo)
                        ammo = maxAmmo;
                    break;
                case Item.Type.Coin:
                    coin += item.value;
                    if (coin >= maxCoin)
                        coin = maxCoin;
                    break;
                case Item.Type.Heart:
                    health += item.value;
                    if (health >= maxHealth)
                        health = maxHealth;
                    break;
                case Item.Type.Grenade:
                    if (hasGrenade == maxGrenade)
                        return;
                    grenades[hasGrenade].SetActive(true);
                    hasGrenade += item.value;
                    break;
            }
            Destroy(other.gameObject);
        }
    }


    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearWeapon = other.gameObject;
            if (playerInput.IsInteraction)
            {
                Item item = nearWeapon.GetComponent<Item>();
                hasWeapon[item.value] = true;
                Destroy(nearWeapon);
            }
        }
        
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag == "Weapon")
        {
            nearWeapon = null;
        }
    }
}
