using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    public float speed = 2.5f;
    public float damage = 1f;
    public Vector3 dir = Vector3.right;

    void Update()
    {
        transform.Translate(speed * Time.deltaTime * dir, Space.World);
        
        if (GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            Destroy(this.gameObject);
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Player") {
            other.gameObject.GetComponent<PlayerController>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }
}
