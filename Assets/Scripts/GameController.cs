using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private PlayerController[] players;
    public MultiplayerCameraSystem.CameraController cameraController;

    public static GameController instance;

    private int playersReady = 0;

    public GameObject feedbackTextPrefab;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        players = GameObject.FindObjectsOfType<PlayerController>();
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

    public void SetReady()
    {
        playersReady++;

        if (playersReady == players.Length)
            StartGame();
    }

    private void StartGame()
    {
        if (players.Length != 1)
            cameraController.enabled = true;
        Time.timeScale = 1;
    }
    public void FeedbackUI(string _message, Transform _transform)
    {
        GameObject textObj = Instantiate(feedbackTextPrefab, _transform.position, Quaternion.identity);
        textObj.GetComponentInChildren<TextMeshProUGUI>().text = _message;
        Destroy(textObj.gameObject, 1.0f);
    }
}