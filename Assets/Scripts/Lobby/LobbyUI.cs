using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private Button mainMenuButton;
    [SerializeField]
    private Button createLobbyButton;
    [SerializeField]
    private Button joinLobbyButton;
    [SerializeField]
    private LobbyCreateUI lobbyCreateUI;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenu);
        });
        createLobbyButton.onClick.AddListener(() => {
            lobbyCreateUI.Show();
        });
        joinLobbyButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.MainMenu);
        });
    }
}
