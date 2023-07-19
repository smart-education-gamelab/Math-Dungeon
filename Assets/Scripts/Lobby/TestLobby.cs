using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using Unity.Netcode;

public class TestLobby : MonoBehaviour
{
    [SerializeField]
    public static TestLobby Instance { get; private set; }

    private Lobby joinedLobby;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(this);

        InitializeUnityAuthentication();
    }

    private async void InitializeUnityAuthentication()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializeOptions = new InitializationOptions();
            initializeOptions.SetProfile(UnityEngine.Random.Range(0, 1000).ToString());

            await UnityServices.InitializeAsync(initializeOptions);

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }

    public async void CreateLobby(String lobbyName)
    {
        try
        {
            joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 2);

            NetworkManager.Singleton.StartHost();
            Loader.LoadNetwork(Loader.Scene.PuzzleOneDoors);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
    }

    public Lobby GetLobby()
    {
        return joinedLobby;
    }

    //public async void JoinLobby()
    //{
    //    await;
    //}

}
