using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{


    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;


    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        MathDungeonMultiplayer.Instance.OnFailedToJoinGame += MathDungeonMultiplayer_OnFailedToJoinGame;
        TestLobby.Instance.OnCreateLobbyStarted += TestLobby_OnCreateLobbyStarted;
        TestLobby.Instance.OnCreateLobbyFailed += TestLobby_OnCreateLobbyFailed;
        TestLobby.Instance.OnJoinStarted += TestLobby_OnJoinStarted;
        TestLobby.Instance.OnJoinFailed += TestLobby_OnJoinFailed;
        TestLobby.Instance.OnQuickJoinFailed += TestLobby_OnQuickJoinFailed;

        Hide();
    }

    private void TestLobby_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a Lobby to Quick Join!");
    }

    private void TestLobby_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join Lobby!");
    }

    private void TestLobby_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining Lobby...");
    }

    private void TestLobby_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create Lobby!");
    }

    private void TestLobby_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating Lobby...");
    }

    private void MathDungeonMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to connect");
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
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
        MathDungeonMultiplayer.Instance.OnFailedToJoinGame -= MathDungeonMultiplayer_OnFailedToJoinGame;
        TestLobby.Instance.OnCreateLobbyStarted -= TestLobby_OnCreateLobbyStarted;
        TestLobby.Instance.OnCreateLobbyFailed -= TestLobby_OnCreateLobbyFailed;
        TestLobby.Instance.OnJoinStarted -= TestLobby_OnJoinStarted;
        TestLobby.Instance.OnJoinFailed -= TestLobby_OnJoinFailed;
        TestLobby.Instance.OnQuickJoinFailed -= TestLobby_OnQuickJoinFailed;
    }

}