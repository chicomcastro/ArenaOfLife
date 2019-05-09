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

    private LinkedList<GameObject> lives;

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
        lives = new LinkedList<GameObject>();

        for (int i = 0; i < lifeQuant; i++)
        {
            lives.AddLast(Instantiate(heartPrefab, healthBar.transform));
        }
    }

    [ContextMenu("Lost half heart")]
    public void LostHalfHeart()
    {
        GameObject lastHeart = lives.Last.Value;
        if (lastHeart.GetComponent<Image>().sprite == fullHeart)
        {
            lastHeart.GetComponent<Image>().sprite = halfHeart;
        }
        else
        {
            lastHeart.GetComponent<Image>().sprite = emptyHeart;
            lives.RemoveLast();
        }
    }
}
