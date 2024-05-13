using UnityEngine;
using UnityEngine.UI;

public class CreditsUI : MonoBehaviour
{

    [SerializeField] private Button backButton;

    private void Awake()
    {
        backButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenu);
        });
    }
}
