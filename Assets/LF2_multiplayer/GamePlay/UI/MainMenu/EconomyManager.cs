using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Economy;
using Unity.Services.Economy.Model;
using UnityEngine;

namespace UnityGamingServicesUseCases
{
    
    public class EconomyManager : MonoBehaviour
    {
        const int k_EconomyPurchaseCostsNotMetStatusCode = 10504;

        public CurrencyHudView currencyHudView;
        public static EconomyManager instance { get; private set; }

        public Dictionary<string , int> cached_CurrentBalance = new Dictionary<string, int>();

        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        public async Task RefreshEconomyConfiguration()
        {
            // Calling GetCurrenciesAsync (or GetInventoryItemsAsync), in addition to returning the appropriate
            // Economy configurations, will update the cached configuration list, including any new Currency,
            // Inventory Item, or Purchases that have been published since the last time the player's configuration
            // was cached.
            //
            // This is important to do before hitting the Economy or Remote Config services for any other calls as
            // both use the cached data list.
            await EconomyService.Instance.Configuration.GetCurrenciesAsync();
        }

        public async Task RefreshCurrencyBalances()
        {
            // Debug.Log("RefreshCurrencyBalances");
            GetBalancesResult balanceResult = null;

            try
            {
                balanceResult = await GetEconomyBalances();
            }
            catch (EconomyRateLimitedException e)
            {
                balanceResult = await Utils.RetryEconomyFunction(GetEconomyBalances, e.RetryAfter);
            }
            catch (Exception e)
            {
                Debug.Log("Problem getting Economy currency balances:");
                Debug.LogException(e);
            }

            // Check that scene has not been unloaded while processing async wait to prevent throw.
            if (this == null) return;
            // Debug.Log("Set Balances");
            // Debug.Log(balanceResult);
            
            currencyHudView.SetBalances(balanceResult);
            SetCachedBalance(balanceResult);
        }

        static Task<GetBalancesResult> GetEconomyBalances()
        {
            var options = new GetBalancesOptions { ItemsPerFetch = 100 };
            return EconomyService.Instance.PlayerBalances.GetBalancesAsync(options);
        }

        void SetCachedBalance(GetBalancesResult getBalancesResult){
            if (getBalancesResult is null) return;            
            foreach (var balance in getBalancesResult.Balances)
            {
                // Debug.Log("Currency Id " + balance.CurrencyId); 
                cached_CurrentBalance[balance.CurrencyId] = (int)balance.Balance; 
            }
        }

        public int GetCurrencyBalance(string currencyId)
        {
            
            return cached_CurrentBalance[currencyId];
        
        }

        public void SetCurrencyBalance(string currencyId, long balance)
        {
            currencyHudView.SetBalance(currencyId, balance);
        }

        public void ClearCurrencyBalances()
        {
            currencyHudView.ClearBalances();
        }

        public async Task<MakeVirtualPurchaseResult> MakeVirtualPurchaseAsync(string virtualPurchaseId)
        {
            try
            {
                return await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(virtualPurchaseId);
            }
            catch (EconomyException e)
            when (e.ErrorCode == k_EconomyPurchaseCostsNotMetStatusCode)
            {
                // Rethrow purchase-cost-not-met exception to be handled by shops manager.
                throw;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
        }

        // public void MakeVirtualPurchase(string virtualPurchaseId)
        // {
        //     try
        //     {
        //         return await EconomyService.Instance.Purchases.MakeVirtualPurchaseAsync(virtualPurchaseId);
        //     }
        //     catch (EconomyException e)
        //     when (e.ErrorCode == k_EconomyPurchaseCostsNotMetStatusCode)
        //     {
        //         // Rethrow purchase-cost-not-met exception to be handled by shops manager.
        //         throw;
        //     }
        //     catch (Exception e)
        //     {
        //         Debug.LogException(e);
        //         return default;
        //     }
        // }

    }
    
}
