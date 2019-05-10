﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BTAI;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    public GameObject[] players;
    public GameObject enemyAttack;

    private float lastAttackTime;
    private float walkSpeed;
    private float sprintSpeed;
    private float curSpeed;
    private float maxSpeed;
    private Vector3 dir;
    private Vector2 facingDir = Vector2.right;
    private GameObject target;
    private Vector3 initialPos;


    private Animator anim;
    private Rigidbody2D rb;

    private Root aiRoot = BT.Root();

    [Header("Enemy Stats")]
    public EnemyStats stats;

    void OnEnable()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        gameObject.layer = 8;
        gameObject.tag = "Enemy";

        aiRoot.OpenBranch(
            BT.If(IsAlive).OpenBranch(
                BT.Selector().OpenBranch(
                    BT.If(PlayerInRange).OpenBranch(
                        BT.Call(PrepareForAttack),
                        BT.If(CanAttack).OpenBranch(
                            BT.Call(Attack)
                        )
                    ),
                    BT.If(PlayerInTerritory).OpenBranch(
                        BT.Call(Follow)
                    ),
                    BT.If(AwayFromHome).OpenBranch(
                        BT.Call(ReturnHome)
                    ),
                    BT.Sequence().OpenBranch(
                        BT.Call(Rest),
                        BT.Wait(2.0f),
                        BT.Call(Turn)
                    )
                )
            )
        );
    }

    private void PrepareForAttack()
    {
        if (enemyAttack != null)
        {
            Rest();
        }
    }

    private bool TestVisibleTarget()
    {
        if (SameDirection(dir, facingDir))
        {
            LayerMask layerMask = 1 << 8;
            layerMask = ~layerMask;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, Mathf.Infinity, layerMask);
            if (hit.collider.gameObject.tag == "Player")
                return true;

            return false;
        }

        return false;
    }

    private bool AwayFromHome()
    {
        return ((initialPos - transform.position).magnitude > 0.1f);
    }

    private void ReturnHome()
    {
        MoveOn((initialPos - transform.position).normalized);
    }

    private void Aim(Vector3 dir)
    {
        if (!SameDirection(facingDir, dir))
            Turn();
    }

    private void Patrol()
    {
        LayerMask layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, facingDir, Mathf.Infinity, layerMask);

        if (hit.collider != null)
        {
            Debug.DrawLine(transform.position, hit.point, Color.white);
            if (hit.distance <= gameObject.GetComponent<Collider2D>().bounds.size.x)
            {
                Turn();
            }
        }

        MoveOn(facingDir);
    }

    private void Turn()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = -localScale.x;
        transform.localScale = localScale;

        facingDir = (transform.localScale.x > 0 ? Vector2.right : Vector2.left);
    }

    private void Follow()
    {
        if (IsBusy())
            return;

        MoveOn(dir);
    }

    private bool PlayerInTerritory()  // Preceeds Follow
    {
        if (dir.magnitude < stats.searchingRange && TestVisibleTarget())
        {
            return true;
        }

        return false;
    }

    private bool PlayerInRange()  // Preceeds Attack
    {
        if (dir.magnitude < stats.attackRange && TestVisibleTarget())
        {
            return true;
        }

        return false;
    }

    private bool IsBusy()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
            return true;

        return false;
    }

    private bool SameDirection(Vector3 dir, Vector2 facingDir)
    {
        if (Vector3.Dot(facingDir, dir) < 0)
            return false;
        return true;
    }

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        lastAttackTime = Time.time;
        initialPos = transform.position;

        walkSpeed = (float)(stats.speed + (stats.agility / 5));
        sprintSpeed = walkSpeed + (walkSpeed / 2);

        Vector3 localScale = transform.localScale;
        localScale.x *= (UnityEngine.Random.Range(0f, 1f) > 0.5 ? -1 : 1);
        transform.localScale = localScale;

        facingDir = (transform.localScale.x > 0 ? Vector2.right : Vector2.left);
    }

    void Update()
    {
        curSpeed = walkSpeed;
        maxSpeed = curSpeed;

        GetMinimalPlayerDist();

        aiRoot.Tick();
    }

    private void GetMinimalPlayerDist()
    {
        dir = Mathf.Infinity * Vector3.one;
        foreach (GameObject player in players)
        {
            if ((player.transform.position - transform.position).magnitude < dir.magnitude)
            {
                dir = player.transform.position - transform.position;
                target = player;
            }
        }
    }

    public bool CanAttack()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            return false;
        }

        if (target.GetComponent<PlayerController>().status.HP <= 0)
        {
            List<GameObject> newPlayersList = new List<GameObject>(players);
            newPlayersList.Remove(target);
            players = newPlayersList.ToArray();
            return false;
        }

        if (Time.time - lastAttackTime > stats.attackCooldown)
        {
            lastAttackTime = Time.time;
            return true;
        }

        return false;
    }

    private void MoveOn(Vector3 dir)
    {
        Aim(dir);
        anim.Play("moving");
        rb.velocity = dir.normalized * curSpeed;
    }

    private void Attack()
    {
        Aim(dir);
        anim.Play("attack");

        if (enemyAttack != null)
        {
            RangedAttack();
        }
        else
        {
            MeleeAttack();
        }
    }

    private void MeleeAttack()
    {
        PlayerController pc = target.gameObject.GetComponent<PlayerController>();
        pc.TakeDamage(stats.damage);

        Rigidbody2D _rb = target.gameObject.GetComponent<Rigidbody2D>();
        Vector2 enemyDir = _rb.transform.position - transform.position;
        if (Mathf.Abs(enemyDir.y) < gameObject.GetComponent<Collider2D>().bounds.extents.y)
            enemyDir.y = 0;
        _rb.AddForce(stats.attackKnockback * enemyDir.normalized * _rb.mass * 0.5f, ForceMode2D.Impulse);//, ForceMode2D.Impulse);
    }

    private void RangedAttack()
    {
        GameObject gamo = Instantiate(enemyAttack, transform.position, Quaternion.LookRotation(Vector3.forward, Vector3.Cross(dir, Vector3.forward)));
        gamo.GetComponent<ProjectileMovement>().dir = dir;
        gamo.GetComponent<ProjectileMovement>().damage = stats.damage;
    }

    public void Rest()
    {
        anim.Play("idle");
        rb.velocity = Vector3.zero;
    }

    public bool IsAlive()
    {
        if (stats.HP <= 0f)
        {
            anim.Play("death");
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            gameObject.GetComponent<Collider2D>().isTrigger = true;
            return false;
        }

        return true;
    }

    public void TakeDamage(float _damage)
    {
        stats.HP -= _damage;

        lastAttackTime = Time.time - stats.attackCooldown / 2;
    }
}