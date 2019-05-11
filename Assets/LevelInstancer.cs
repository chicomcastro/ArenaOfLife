using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInstancer : MonoBehaviour
{
    GameObject[] players;
    public GameObject[] levels;

    void Awake()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    void Update()
    {
        foreach (GameObject level in levels)
        {
            foreach (GameObject player in players)
            {
                if ((transform.localPosition - player.transform.position).magnitude > 20f)
                    level.SetActive(false);
                else
                    level.SetActive(true);
            }
        }
    }
}
