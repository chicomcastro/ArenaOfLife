using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class HeartCurrency : MonoBehaviour
{
    public PlayerController player;
    public int totalHearts;

    public int lifeQuant;

    public int powerQuant;

    public int speedQuant;

    public TextMeshProUGUI totalText;
    public GameObject currencyPanel;
    public Button readyButton;

    public GameObject heartPrefab;
    public GameObject HPCorePanel;
    public GameObject PowerCorePanel;
    public GameObject AgilityCorePanel;

    private string totalTextString = "Cores Lefting: ";
    private LifeManager lifeManager;

    void Awake()
    {
        lifeManager = GetComponent<LifeManager>();
        Time.timeScale = 0;

        AttTotalString();
    }

    private void Update()
    {
        readyButton.interactable = (totalHearts == 0);
    }

    public void SetUpStats()
    {
        if (totalHearts != 0)
            return;

        lifeManager.SetUpStats(lifeQuant);
        player.status.HP = 10f * lifeQuant;
        player.status.damage = 5f * (powerQuant + 1);
        player.status.agility = speedQuant;

        currencyPanel.SetActive(false);
        GameController.instance.SetReady();
    }

    public void IncrementHP()
    {
        lifeQuant = Increment(HPCorePanel, lifeQuant);
    }

    public void IncrementPower()
    {
        powerQuant = Increment(PowerCorePanel, powerQuant);
    }

    public void IncrementAglity()
    {
        speedQuant = Increment(AgilityCorePanel, speedQuant);
    }

    private int Increment(GameObject _g, int _quant)
    {
        if (totalHearts == 0)
            return _quant;

        Instantiate(heartPrefab, _g.transform).name = "Heart";
        totalHearts--;

        AttTotalString();

        return _quant + 1;
    }

    public void DecrementHP()
    {
        lifeQuant = Decrement(HPCorePanel, lifeQuant);
    }

    public void DecrementPower()
    {
        powerQuant = Decrement(PowerCorePanel, powerQuant);
    }

    public void DecrementAgility()
    {
        speedQuant = Decrement(AgilityCorePanel, speedQuant);
    }

    private int Decrement(GameObject _g, int _quant)
    {
        if (_quant == 0)
            return 0;

        Destroy(_g.transform.Find("Heart").gameObject);
        totalHearts++;
        AttTotalString();
        return _quant - 1;
    }

    private void AttTotalString()
    {
        totalText.SetText(totalTextString + totalHearts.ToString());
    }
}
