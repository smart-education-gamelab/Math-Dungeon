using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Analytics;

public class UGS_Analytics : MonoBehaviour
{
    [System.Obsolete]
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            GiveConsent(); // Get user consent according to various legislations
            LevelCompletedCustomEvent();
        }
        catch (ConsentCheckException e)
        {
            Debug.Log(e.ToString());
        }
    }

    [System.Obsolete]
    private void LevelCompletedCustomEvent()
    {
        int currentLevel = 1;
        // Define Custom Parameters
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "levelName", "level" + currentLevel.ToString()}
        };

        // The ‘levelCompleted’ event will get cached locally
        //and sent during the next scheduled upload, within 1 minute
        AnalyticsService.Instance.CustomData("levelCompleted", parameters);

        // You can call Events.Flush() to send the event immediately
        AnalyticsService.Instance.Flush();
    }

    public void GiveConsent()
    {
	// Call if consent has been given by the user
        AnalyticsService.Instance.StartDataCollection();
        Debug.Log($"Consent has been provided. The SDK is now collecting data!");
    }
}
