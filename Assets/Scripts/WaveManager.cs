using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GameObject[] levels;
    public PlayerController player;

    private int currentLevel;

    void Awake()
    {
        foreach (GameObject g in levels)
        {
            g.SetActive(false);
        }

        currentLevel = 0;
        levels[currentLevel].SetActive(true);
    }

    void Start()
    {
        InvokeRepeating("CheckForNextLevel", 5.0f, 1.0f);
    }

    private void CheckForNextLevel()
    {
        if (player.status.HP <= 0) {
            CancelInvoke();
            return;
        }

        Enemy[] enemies = levels[currentLevel].GetComponentsInChildren<Enemy>();
        int enemiesAlive = enemies.Length;
        foreach (Enemy e in enemies)
        {
            if (e.stats.HP <= 0)
            {
                enemiesAlive--;
            }
        }
        if (enemiesAlive == 0)
        {
            TriggerNextLevel();
        }
    }

    private void TriggerNextLevel()
    {
        // End animation

        levels[currentLevel].SetActive(false);

        currentLevel++;
        if (currentLevel > levels.Length)
        {
            CancelInvoke();
            // End game animation
            return;
        }

        levels[currentLevel].SetActive(true);

        // Next level animation
    }
}
