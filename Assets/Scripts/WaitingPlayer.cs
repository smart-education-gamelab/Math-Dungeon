using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class WaitingPlayer : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    [SerializeField] private Button kickButton;
    [SerializeField] private TextMeshPro playerNameText;


    private void Awake()
    {
        kickButton.onClick.AddListener(() => {
            PlayerData playerData = MathDungeonMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            TestLobby.Instance.KickPlayer(playerData.playerId.ToString());
            MathDungeonMultiplayer.Instance.KickPlayer(playerData.clientId);
        });
    }

    private void Start()
    {
        MathDungeonMultiplayer.Instance.OnPlayerDataNetworkListChanged += MathDungeonMultiplayer_OnPlayerDataNetworkListChanged;
        SetReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

        kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

        UpdatePlayer();
    }

    private void CharacterSelectReady_OnReadyChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void MathDungeonMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (MathDungeonMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();

            PlayerData playerData = MathDungeonMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);

            readyGameObject.SetActive(SetReady.Instance.IsPlayerReady(playerData.clientId));

            playerNameText.text = playerData.playerName.ToString();
        }
        else
        {
            Hide();
        }
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
        MathDungeonMultiplayer.Instance.OnPlayerDataNetworkListChanged -= MathDungeonMultiplayer_OnPlayerDataNetworkListChanged;
    }


}