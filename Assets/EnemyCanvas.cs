using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCanvas : MonoBehaviour
{
    public Slider healthBar;
    public Enemy enemy;

    public Vector3 offSet;

    void FixedUpdate()
    {
        transform.position = enemy.transform.position + offSet;
        healthBar.value = enemy.stats.HP;

        if (healthBar.value <= 0)
        {
            healthBar.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }
    }
}
