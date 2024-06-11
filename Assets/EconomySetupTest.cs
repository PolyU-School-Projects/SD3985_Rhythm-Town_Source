using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using System.Collections.Generic;

public class EconomySetupTest : MonoBehaviour
{
    public async void Start()
    {

        await UnityServices.InitializeAsync();


        await EconomyService.Instance.Configuration.SyncConfigurationAsync();


        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        List<CurrencyDefinition> definitions = EconomyService.Instance.Configuration.GetCurrencies();

    }

}