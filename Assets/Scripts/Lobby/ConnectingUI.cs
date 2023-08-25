using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{



    private void Start()
    {
        MathDungeonMultiplayer.Instance.OnTryingToJoinGame += MathDungeonMultiplayer_OnTryingToJoinGame;
        MathDungeonMultiplayer.Instance.OnFailedToJoinGame += MathDungeonManager_OnFailedToJoinGame;

        Hide();
    }

    private void MathDungeonManager_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void MathDungeonMultiplayer_OnTryingToJoinGame(object sender, System.EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        MathDungeonMultiplayer.Instance.OnTryingToJoinGame -= MathDungeonMultiplayer_OnTryingToJoinGame;
        MathDungeonMultiplayer.Instance.OnFailedToJoinGame -= MathDungeonManager_OnFailedToJoinGame;
    }

}