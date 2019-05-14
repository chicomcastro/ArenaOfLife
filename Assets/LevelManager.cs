using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private Enemy[] enemies;

    void Start()
    {
        enemies = gameObject.GetComponentsInChildren<Enemy>();
        InvokeRepeating("CheckForLevelCompletion", 0f, 0.5f);
    }

    private void CheckForLevelCompletion()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy.stats.HP > 0)
                return;
        }

        Destroy(transform.Find("Portal").gameObject);
        this.enabled = false;
        CancelInvoke();
    }
}
