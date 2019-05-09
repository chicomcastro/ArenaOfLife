using UnityEngine;

public class GameController : MonoBehaviour
{
    public PlayerController[] players;

    public static GameController instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    public bool IsAllPlayersDead()
    {
        foreach (PlayerController p in players)
        {
            if (p.status.HP > 0)
                return false;
        }

        return true;
    }
}