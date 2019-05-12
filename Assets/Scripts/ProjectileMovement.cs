using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float speed = 2.5f;
    public float damage = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<Enemy>() != null)
            return;
            
        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
        }

        InvokeRepeating("DestroyRoutine", 0f, 0.1f);
    }

    private void DestroyRoutine()
    {
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Animator>().Play("hitting");

        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            Destroy(this.gameObject);
    }
}
