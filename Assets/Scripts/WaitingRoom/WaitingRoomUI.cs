using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Unity.Services.Lobbies.Models;

public class WaitingRoomUI : MonoBehaviour
{
    [SerializeField]
    private Button mainMenuButton;
    [SerializeField]
    private TextMeshProUGUI lobbyNameText;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });
    }

    private void Start()
    {
        Lobby lobby = TestLobby.Instance.GetLobby();

        lobbyNameText.text = "Lobby name: " + lobby.Name;
    }
}
