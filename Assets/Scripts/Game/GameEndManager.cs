using System;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;
using UnityEngine.UI;

public class GameEndManager : MonoBehaviour
{
    [SerializeField] string gameName;
    [SerializeField] TMP_Text accuracyText, bestRecordText, coinText, expText;
    [SerializeField] List<GameObject> difficultyIcons;
    [SerializeField] List<Image> starIcons;
    int coinAwarded, expAwarded, energyConsumed;
    
    private async void Awake()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        energyConsumed = 10;
        coinAwarded = 100;

        GetBalancesOptions options = new GetBalancesOptions { ItemsPerFetch = 4, };
        GetBalancesResult getBalancesResult =
            await EconomyService.Instance.PlayerBalances.GetBalancesAsync(options);

        if (getBalancesResult.Balances.Count > 0)
        {
            PlayerBalance energy = getBalancesResult.Balances[1]; 
            coinAwarded = Math.Clamp(int.Parse(energy.Balance.ToString()) * 10, 0, 100);
            energyConsumed = Math.Clamp(10, 0, (int)energy.Balance);
        }
    }

    async void Start()
    {
        accuracyText.text = $"{ GameController.accuracy }" + (GameStartManager.isInfinite? "" : "%");
        for (int i = 0; i < starIcons.Count; i ++)
            starIcons[i].sprite = Resources.Load<Sprite>("Stars/Star " + (GameStartManager.bestRecordList[i] < (i < 4? 60 : 1000)? "Gray" :
                GameStartManager.bestRecordList[i] < (i < 4? 90 : 1500)? "Yellow" : "Pink"));

        for (int i = 0; i < difficultyIcons.Count; i++)
        {
            difficultyIcons[i].SetActive(i == GameStartManager.lastDifficulty);
        }
        if (GameController.accuracy > GameStartManager.bestRecord)
        {
            GameStartManager.bestRecordList[GameStartManager.lastDifficulty] = GameController.accuracy;
            starIcons[GameStartManager.lastDifficulty].sprite = Resources.Load<Sprite>("Stars/Star " + 
                (GameStartManager.bestRecordList[GameStartManager.lastDifficulty] < (GameStartManager.lastDifficulty < 4? 60 : 1000)? "Gray" :
                GameStartManager.bestRecordList[GameStartManager.lastDifficulty] < (GameStartManager.lastDifficulty < 4? 90 : 1500)? "Yellow" : "Pink"));
            bestRecordText.text = $"{ GameController.accuracy }" + (GameStartManager.isInfinite? "" : "%");
            string[] recordLabels = {"Easy", "Normal", "Hard", "Expert", "Infinite"};
            var recordData = new Dictionary<string, object> {
                { $"Record_{gameName}_{recordLabels[GameStartManager.lastDifficulty]}", GameController.accuracy }
            };
            await CloudSaveService.Instance.Data.Player.SaveAsync(recordData);
            expAwarded = 10;
        }
        else{
            bestRecordText.text = $"{ GameStartManager.bestRecord }" + (GameStartManager.isInfinite? "" : "%");
            expAwarded = 1;
        }
        
        switch (GameStartManager.skillType)
        {
            case "Coin":
                coinAwarded += GameStartManager.skillLevel * 5;
                break;

            case "Energy":
                energyConsumed -= GameStartManager.skillLevel;
                break;
            
            default:
                break;
        }
        
        coinText.text = $"<size=80%>Coin\n<size=100%>+ {coinAwarded}";
        expText.text = $"<size=80%>Sense of Rhythm\n<size=100%>+ {expAwarded}";
        Award("COINS", coinAwarded);
        AddEXP(expAwarded);
        ConsumeEnergy(energyConsumed);
    }

    public async void Award(string currencyID, int amount)
    {
        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(
            currencyID,
            amount
        ); 
    }

    public async void AddEXP(int amount)
    {
        string currencyID = "EXP";
        string currencyLevel = "LEVEL";

        int Levelone;
        int currentEXP;

        GetBalancesOptions options = new GetBalancesOptions { ItemsPerFetch = 4, };

        GetBalancesResult getBalancesResult =
            await EconomyService.Instance.PlayerBalances.GetBalancesAsync(options);

        if (getBalancesResult.Balances.Count > 0)
        {
            PlayerBalance exp = getBalancesResult.Balances[2];
            string expText = exp.Balance.ToString();
            currentEXP = int.Parse(expText);

            PlayerBalance level = getBalancesResult.Balances[3];
            string levelText = level.Balance.ToString();
            Levelone = int.Parse(levelText);

        if (currentEXP + amount >= 10 * Levelone)
        {
            PlayerBalance newBalance =
                        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(
                            currencyLevel,
                            1
                        );
            PlayerBalance EXPnewBalance = await EconomyService.Instance.PlayerBalances.SetBalanceAsync(currencyID, currentEXP + amount - 10 * Levelone);
            Debug.Log("Level up");
        }
        else
        {
            PlayerBalance newBalance =
                        await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(
                            currencyID,
                            amount
                        );
        }
        }
    }


    public async void addCoin(int amount)
    {

        string currencyID = "COINS";

        PlayerBalance newBalance =
            await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(
                currencyID,
                amount
            );
    }

    public async void ConsumeEnergy(int amount)
    {
        await EconomyService.Instance.PlayerBalances.DecrementBalanceAsync(
            "ENERGY",
            amount
        ); 
    }
}