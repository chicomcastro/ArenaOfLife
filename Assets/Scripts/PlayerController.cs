using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public ControllerType controllerType;
    private Controls controls;

    private Animator anim;
    private bool haveDied;
    private bool isAttacking;

    private Dictionary<string, string> registeredAttacks = new Dictionary<string, string>();
    private LifeManager lifeManager;

    public GameObject feedbackTextPrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lifeManager = transform.parent.gameObject.GetComponentInChildren<LifeManager>();

        controls = new Controls(controllerType);

        int i = 0;
        foreach (string s in controls.attackButtons)
        {
            i++;
            registeredAttacks.Add(s, "attack" + i);
        }

        haveDied = false;
        isAttacking = false;
    }

    void FixedUpdate()
    {
        HandleDeath();

        if (haveDied)
            return;

        HandleAttack();

        if (isAttacking)
            return;

        HandleDamage();

        if (isBeenAttacked)
            return;

        HandleAnimation();

        HandleFacingDirection();

        Move();
    }

    private void HandleDamage()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("damage"))
            isBeenAttacked = false;
    }

    private bool isBeenAttacked = false;

    public void Move()
    {
        walkSpeed = (float)(status.speed + (status.agility * 0.5f));
        sprintSpeed = walkSpeed + (walkSpeed / 2);

        curSpeed = walkSpeed;
        maxSpeed = curSpeed;

        // Move senteces
        rb.velocity = new Vector2(
            Mathf.Lerp(0, Input.GetAxis(controls.horizontalAxis) * curSpeed, 0.8f),
            Mathf.Lerp(0, Input.GetAxis(controls.verticalAxis) * curSpeed, 0.8f)
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

        Vector2 facingDir = (transform.localScale.x > 0 ? Vector2.right : Vector2.left);
        Vector3 boundary = gameObject.GetComponent<Collider2D>().bounds.extents;
        // Cast a ray straight down.
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        hits.Add(Physics2D.Raycast(transform.position, facingDir, Mathf.Infinity, layerMask));

        hits.Add(Physics2D.Raycast(transform.position + Vector3.up * boundary.y, facingDir, Mathf.Infinity, layerMask));
        hits.Add(Physics2D.Raycast(transform.position - Vector3.up * boundary.y, facingDir, Mathf.Infinity, layerMask));

        hits.Add(Physics2D.Raycast(transform.position + Vector3.up * boundary.y * 2, facingDir, Mathf.Infinity, layerMask));
        hits.Add(Physics2D.Raycast(transform.position - Vector3.up * boundary.y * 2, facingDir, Mathf.Infinity, layerMask));

        hits.Add(Physics2D.Raycast(transform.position + (Vector3)facingDir * boundary.x, Vector3.up, Mathf.Infinity, layerMask));
        hits.Add(Physics2D.Raycast(transform.position + (Vector3)facingDir * boundary.x, Vector3.down, Mathf.Infinity, layerMask));

        hits.Add(Physics2D.Raycast(transform.position + (Vector3)facingDir * boundary.x * 2, Vector3.up, Mathf.Infinity, layerMask));
        hits.Add(Physics2D.Raycast(transform.position + (Vector3)facingDir * boundary.x * 2, Vector3.down, Mathf.Infinity, layerMask));

        // If it hits something...
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider != null)
            {
                Debug.DrawLine(transform.position, hit.point, Color.white);
                if (hit.distance < status.attackRange)
                {
                    Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
                    if (enemy != null)
                        enemy.TakeDamage(status.damage);

                    if (hit.collider.gameObject.GetComponent<ProjectileMovement>() == null)
                    {
                        Rigidbody2D _rb = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                        Vector2 enemyDir = _rb.transform.position - transform.position;
                        if (Mathf.Abs(enemyDir.y) < boundary.y)
                            enemyDir.y = 0;
                        _rb.AddForce(status.attackKnockback * enemyDir.normalized * _rb.mass * 50f, ForceMode2D.Impulse);//, ForceMode2D.Impulse);
                    }
                }
            }
        }

        GameObject.FindGameObjectWithTag("AudioManager").transform.Find("SwordSwing").GetComponent<AudioSource>().PlayDelayed(0.15f);
    }


    public void HandleDeath()
    {
        if (status.HP <= 0f)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            GetComponent<Collider2D>().isTrigger = true;
            anim.Play("death");
            haveDied = true;

            transform.parent.Find("Canvas").Find("DeathUIAnimation").gameObject.SetActive(true);

            if (GameController.instance != null)
            {
                if (GameController.instance.IsAllPlayersDead())
                {
                    if (transform.parent.Find("Canvas").Find("DeathUIAnimation").gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
                        return;

                    if (Input.GetButton(controls.attackButtons[0]))
                        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
                    else if (Input.GetButton("Cancel"))
                        UnityEngine.SceneManagement.SceneManager.LoadScene("menu");
                }
            }
        }
    }

    public void TakeDamage(float _damage)
    {
        if (status.HP < 0)
            return;

        if (isAttacking && UnityEngine.Random.Range(0f, 1f) < 0.20 * status.agility)
        { // Dodging
            Dodge();
            return;
        }

        anim.Play("damage");
        isBeenAttacked = true;

        status.HP -= _damage;
        FeedbackUI("-" + _damage.ToString());

        if (lifeManager == null)
        {
            Debug.LogWarning("LifeManager is missing on " + gameObject.name);
            return;
        }

        lifeManager.AttHeartQuant(status.HP);
    }

    private void FeedbackUI(string _message)
    {
        GameObject textObj = Instantiate(feedbackTextPrefab, transform.position, Quaternion.identity);
        textObj.GetComponentInChildren<TextMeshProUGUI>().text = _message;
        Destroy(textObj.gameObject, 1.0f);
    }

    [ContextMenu("DodgeTest")]
    public void Dodge()
    {
        FeedbackUI("DODGE!");
    }
}