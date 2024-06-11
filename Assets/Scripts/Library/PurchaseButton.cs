using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;

public class PurchaseButton : MonoBehaviour
{
    public async Task OnPurchaseButtonClickedAsync()
    {


        string currencyID = "COINS";
        int incrementAmount = 1000;

        Unity.Services.Economy.Model.PlayerBalance newBalance = await EconomyService.Instance.PlayerBalances.IncrementBalanceAsync(currencyID, incrementAmount);
        Debug.Log("buy");
    }
}