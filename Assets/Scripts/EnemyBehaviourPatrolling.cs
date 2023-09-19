using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyBehaviourPatrolling : NetworkBehaviour
{
    #region Public Variables
    public float attackDistance;
    public float moveSpeed;
    public float timer; 
    public bool inRange;
    public Vector3 target;

    public NetworkVariable<Vector3> leftLimit = new NetworkVariable<Vector3>(Vector3.zero, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public NetworkVariable<Vector3> rightLimit = new NetworkVariable<Vector3>(Vector3.zero, readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);
    public GameObject hotZone;
    public GameObject triggerArea;
    #endregion

    #region Private Variables
    private Animator anim;
    private float distance; 
    private bool attackMode;
    private bool cooling; 
    private float intTimer;
    #endregion


    void Awake()
    {
        intTimer = timer;
        anim = GetComponent<Animator>();
        inRange = false;
    }

    private void Start()
    {
        SelectTarget();
    }

    void Update()
    {
        if (!attackMode)
        {
            Move();
        }

        if (!InsideOfLimits() && !inRange && !anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            SelectTarget();
        }
              
        if (inRange)
        {
            EnemyLogic();
        }
        else
        {
            StopAttack();
        }
    }

    private void EnemyLogic()
    {
        distance = Vector2.Distance(transform.position, target);

        if (distance > attackDistance)
        {
            StopAttack();
        }
        else if (attackDistance >= distance && cooling == false)
        {
            Attack();
        }

        if (cooling)
        {
            Cooldown();
            anim.SetBool("Attack", false);
        }
    }

    private void Move()
    {
        anim.SetBool("canWalk", true);

        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Enemy_attack"))
        {
            Vector2 targetPosition = new Vector2(target.x, transform.position.y);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void Attack()
    {
        timer = intTimer;
        attackMode = true;

        anim.SetBool("canWalk", false);
        anim.SetBool("Attack", true);
    }

    private void Cooldown()
    {
        timer -= Time.deltaTime;

        if (timer <= 0 && cooling && attackMode)
        {
            cooling = false;
            timer = intTimer;
        }
    }

    private void StopAttack()
    {
        cooling = false;
        attackMode = false;
        anim.SetBool("Attack", false);
    }

    
    public void TriggerCooling()
    {
        cooling = true;
    }

    private bool InsideOfLimits()
    {
        return transform.position.x > leftLimit.Value.x && transform.position.x < rightLimit.Value.x;
    }

    public void SelectTarget()
    {
        float distanceToLeft = Vector3.Distance(transform.position, leftLimit.Value);
        float distanceToRight = Vector3.Distance(transform.position, rightLimit.Value);

        if (distanceToLeft > distanceToRight)
        {
            target = leftLimit.Value;
        }
        else
        {
            target = rightLimit.Value;
        }

        Flip();
    }

    public void Flip()
    {
        Vector3 rotation = transform.eulerAngles;
        if (transform.position.x > target.x)
        {
            rotation.y = 180;
        }
        else
        {
            rotation.y = 0;
        }

        transform.eulerAngles = rotation;
    }
}
