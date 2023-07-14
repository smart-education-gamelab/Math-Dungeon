using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField]
    private Button closeButton;
    [SerializeField]
    private Button createButton;
    [SerializeField]
    private TMP_InputField lobbyName;

    private void Awake()
    {
        closeButton.onClick.AddListener(() =>
        {
            Hide();
        });

        createButton.onClick.AddListener(() =>
        {
            TestLobby.Instance.CreateLobby(lobbyName.text);
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
