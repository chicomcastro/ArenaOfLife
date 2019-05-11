using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeManager : MonoBehaviour
{
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public GameObject heartPrefab;

    public GameObject healthBar;

    private int lifeQuant;

    private List<GameObject> lives;

    public static LifeManager instance;

    void Awake()
    {
        instance = this;

        foreach (Image i in healthBar.GetComponentsInChildren<Image>())
        {
            Destroy(i.gameObject);
        }
    }

    [ContextMenu("Set up stats")]
    public void SetUpStats(int lifeQuant)
    {
        lives = new List<GameObject>();

        for (int i = 0; i < lifeQuant; i++)
        {
            lives.Add(Instantiate(heartPrefab, healthBar.transform));
        }
    }

    public void AttHeartQuant(float _hp)
    {
        int currentFullHearts = (int)_hp / 10;
        currentFullHearts = currentFullHearts >= 0 ? currentFullHearts : 0;
        int currentHalfHearts = (int)(_hp - currentFullHearts * 10) / 5;
        currentHalfHearts = currentHalfHearts >= 0 ? currentHalfHearts : 0;

        ManageHalfHearts(currentFullHearts, currentHalfHearts);
    }

    private void ManageHalfHearts(int _fullHearts, int _halfHearts)
    {
        int i;
        for (i = 0; i < _fullHearts; i++)
        {
            lives[i].GetComponent<Image>().sprite = fullHeart;
        }
        for (i = 0; i < _halfHearts; i++)
        {
            lives[i + _fullHearts].GetComponent<Image>().sprite = halfHeart;
        }
        for (i = 0; i < lives.Count - (_fullHearts + _halfHearts); i++)
        {
            lives[i + _fullHearts + _halfHearts].GetComponent<Image>().sprite = emptyHeart;
        }
    }
}
