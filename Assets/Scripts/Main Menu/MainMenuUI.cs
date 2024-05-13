using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{


    [SerializeField] private Button playMultiplayerButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;


    private void Awake()
    {
        playMultiplayerButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.Lobby);
        });
        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
        creditsButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.Credits);
        });
        //Time.timeScale = 1f;
    }

}