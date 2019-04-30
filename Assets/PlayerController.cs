using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    // Normal Movements Variables
    private float walkSpeed;
    private float sprintSpeed;
    private float curSpeed;
    private float maxSpeed;
    private Rigidbody2D rb;
    public CharacterStat status;

    private Animator anim;
    private bool haveDied;
    private bool isAttacking;

    private Dictionary<string, string> registeredAttacks = new Dictionary<string, string>();

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        for (int i = 1; i < 4; i++)
        {
            registeredAttacks.Add("Fire" + i, "attack" + i);
        }

        haveDied = false;
        isAttacking = false;

        walkSpeed = (float)(status.speed + (status.agility / 5));
        sprintSpeed = walkSpeed + (walkSpeed / 2);

    }

    void FixedUpdate()
    {
        HandleDeath();

        if (haveDied)
            return;

        HandleAttack();

        if (isAttacking)
            return;

        HandleAnimation();

        HandleFacingDirection();

        Move();
    }

    public void Move()
    {
        curSpeed = walkSpeed;
        maxSpeed = curSpeed;

        // Move senteces
        rb.velocity = new Vector2(
            Mathf.Lerp(0, Input.GetAxis("Horizontal") * curSpeed, 0.8f),
            Mathf.Lerp(0, Input.GetAxis("Vertical") * curSpeed, 0.8f)
            );
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

    public void HandleAttack()
    {
        foreach (string s in registeredAttacks.Keys)
        {
            if (Input.GetButton(s) && !isAttacking)
            {
                rb.velocity = Vector3.zero;
                anim.Play(registeredAttacks[s]);
                isAttacking = true;

                Attack();
            }
        }

        if (anim.IsInTransition(0))
        {
            isAttacking = false;
        }

        return;
    }
    private void Attack()
    {
        // Bit shift the index of the layer (8) to get a bit mask
        int layerMask = 1 << 8;

        // This would cast rays only against colliders in layer 8.
        // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
        //layerMask = ~layerMask;

        Vector2 dir = (transform.localScale.x > 0 ? Vector2.right : Vector2.left);
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);

        // If it hits something...
        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.white);
            if (hit.distance < status.attackRange)
                hit.collider.gameObject.GetComponent<Enemy>().TakeDamage(status.damage);
        }
    }

    public void HandleDeath()
    {
        if (status.HP <= 0f)
        {
            rb.velocity = Vector3.zero;
            anim.Play("death");
            haveDied = true;

            if (Input.GetButton("Fire1"))
                UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    public void TakeDamage(float _damage)
    {
        status.HP -= _damage;

        LifeManager.instance.LostHalfHeart();
    }
}

[System.Serializable]
public class CharacterStat
{
    public int agility;
    public int speed;
    public float HP;
    public float damage;
    public float attackCooldown;
    public float attackRange;
}