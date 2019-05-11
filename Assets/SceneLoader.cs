using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string _sceneToLoad)
    {
        SceneManager.LoadScene(_sceneToLoad);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene.Split('-')[0] != "level")
                return;
            int sceneNumber = int.Parse(currentScene.Split('-')[1]);
            string nextScene = "level-" + (sceneNumber + 1).ToString();
            if (HaveKilledAllEnemies())
                LoadScene(nextScene);
        }
    }

    private bool HaveKilledAllEnemies()
    {
        GameObject levelObj = GameObject.FindGameObjectWithTag("Creatures");
        
        Enemy[] enemies = levelObj.GetComponentsInChildren<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.stats.HP > 0)
                return false;
        }

        return true;
    }
}
