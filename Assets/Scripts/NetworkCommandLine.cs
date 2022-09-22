using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Netcode.Samples;

public class NetworkCommandLine : MonoBehaviour
{
    private NetworkManager netManager;

    [SerializeField]
    private TMP_InputField passwordInputField;

    [SerializeField]
    private GameObject canvas;
    

    void Start()
    {
        netManager = GetComponentInParent<NetworkManager>();

        if (Application.isEditor) return;

        
    }

    public void Host()
    {
        canvas.SetActive(false);
        NetworkManager.Singleton.ConnectionApprovalCallback = ApprovalCheck;
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(passwordInputField.text);
        NetworkManager.Singleton.StartHost();

    }

    public void Client()
    {
        canvas.SetActive(false);
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes(passwordInputField.text);
        NetworkManager.Singleton.StartClient();
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        response.Pending = false;
        // The client identifier to be authenticated
        var clientId = request.ClientNetworkId;

        // Additional connection data defined by user code
        var connectionData = request.Payload;
        

        // Your approval logic determines the following values

        string password = System.Text.Encoding.ASCII.GetString(connectionData);

        response.Approved = password == passwordInputField.text;
        response.CreatePlayerObject = true;

        response.PlayerPrefabHash = null;

        Debug.Log("password = " + password);
    }
}