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
    public CharacterStat plStat;
    public Animator anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        walkSpeed = (float)(plStat.speed + (plStat.agility / 5));
        sprintSpeed = walkSpeed + (walkSpeed / 2);

    }

    void FixedUpdate()
    {
        curSpeed = walkSpeed;
        maxSpeed = curSpeed;

        // Move senteces
        rb.velocity = new Vector2(
            Mathf.Lerp(0, Input.GetAxis("Horizontal") * curSpeed, 0.8f),
            Mathf.Lerp(0, Input.GetAxis("Vertical") * curSpeed, 0.8f)
            );

        HandleAnimation();

        HandleFacingDirection();
    }

    public void HandleAnimation()
    {
        if (rb.velocity.magnitude < 0.1f)
            anim.Play("idle");
        else
            anim.Play("moving");
    }

    public void HandleFacingDirection() {
        
        if (rb.velocity.x < 0f)
            transform.localScale = new Vector3 (Mathf.Abs(transform.localScale.x)*(-1), transform.localScale.y,transform.localScale.z);
        else
            transform.localScale = new Vector3 (Mathf.Abs(transform.localScale.x)*(1), transform.localScale.y,transform.localScale.z);
    }
}

[System.Serializable]
public class CharacterStat
{
    public int agility;
    public int speed;
}