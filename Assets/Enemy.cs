using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public GameObject player;
    public GameObject enemyAttack;

    [Header("Enemy Stats")]
    public float attackRange = 1f;
    public float searchingRange = 5f;
    private float lastAttackTime;
    private int patrolSentido;

    private float walkSpeed;
    private float sprintSpeed;
    private float curSpeed;
    private float maxSpeed;


    private Animator anim;
    private bool canAttack;
    private bool isAttacking;
    private bool haveDied;
    private Rigidbody2D rb;

    private float time;

    public CharacterStat enemyStats;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        canAttack = true;
        haveDied = false;
        isAttacking = false;

        time = Time.time;
        lastAttackTime = Time.time;
        patrolSentido = -1;

        walkSpeed = (float)(enemyStats.speed + (enemyStats.agility / 5));
        sprintSpeed = walkSpeed + (walkSpeed / 2);

        Rest();
    }

    void Update()
    {
        HandleAnimation();

        HandleFacingDirection();

        HandleDeath();

        if (haveDied)
        {
            Rest();
            return;
        }

        HandleAttack();

        if (isAttacking)
            return;

        HandleMovement();
    }

    public void HandleAttack()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("attacking"))
        {
            isAttacking = false;
        }

        if (!IsLoockingToPlayer())
        {
            canAttack = false;
        }

        if (Time.time - lastAttackTime > enemyStats.attackCooldown)
        {
            canAttack = true;
            lastAttackTime = Time.time;
        }
    }

    private void HandleMovement()
    {
        curSpeed = walkSpeed;
        maxSpeed = curSpeed;

        Vector3 dir = player.transform.position - transform.position;

        if (dir.magnitude < searchingRange && IsLoockingToPlayer())
        {
            if (dir.magnitude < attackRange && canAttack)
            {
                Attack();
            }

            if (!isAttacking && dir.magnitude > attackRange * 0.9f)
            {
                MoveOn(dir);
            }
        }
        else
        {
            if (Time.time - time > 5f)
            {
                Rest();

                if (Time.time - time > 4f)
                {
                    patrolSentido *= (-1);
                    Patrol(patrolSentido);
                }
            }
        }
    }

    public void MoveOn(Vector3 dir)
    {
        anim.Play("moving");
        rb.velocity = dir.normalized * curSpeed;
    }

    private bool IsLoockingToPlayer()
    {
        return (
            (transform.localScale.x > 0) && (player.transform.position.x - transform.position.x > 0) ||
            (transform.localScale.x < 0) && (player.transform.position.x - transform.position.x < 0)
        );
    }

    public void Attack()
    {
        isAttacking = true;
        canAttack = false;
        anim.Play("attacking");
        GameObject gamo = Instantiate(enemyAttack, transform.position, Quaternion.identity);
        gamo.GetComponent<ProjectileMovement>().dir = (player.transform.position - transform.position).normalized;
        gamo.GetComponent<ProjectileMovement>().damage = enemyStats.damage;
        gamo.GetComponent<ProjectileMovement>().player = player;

        rb.velocity = Vector3.zero;
    }

    public void Patrol(int sentido)
    {
        time = Time.time;
        rb.velocity = sentido * transform.right * curSpeed;
    }

    public void HandleAnimation()
    {
        if (rb.velocity.magnitude < 0.1f)
            anim.Play("idle");
        else
            anim.Play("moving");
    }

    public void HandleFacingDirection()
    {
        if (Mathf.Abs(rb.velocity.x) < 0.1f)
            return;

        if (rb.velocity.x < 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (-1), transform.localScale.y, transform.localScale.z);
        else if (rb.velocity.x > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * (1), transform.localScale.y, transform.localScale.z);
    }

    public void Rest()
    {
        rb.velocity = Vector3.zero;
    }

    public void HandleDeath()
    {
        if (enemyStats.HP <= 0f)
        {
            anim.Play("death");
            haveDied = true;
        }

        if (player.GetComponent<PlayerController>().plStat.HP <= 0)
            haveDied = true;
    }
}