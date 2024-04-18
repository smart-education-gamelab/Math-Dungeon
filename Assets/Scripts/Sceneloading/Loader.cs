using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class Loader
{

    public enum Scene
    {
        MainMenu,
        Lobby,
        Loading,
        WaitingRoom,
        PuzzleOneDoors,
        PuzzleTwoGears,
        PuzzleFourPotions,
        Necromancer
        
    }

    private static Scene targetScene;

    public static void Load(Scene targetScene)
    {
        Loader.targetScene = targetScene;

        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoadNetwork(Scene targetScene)
    {
        if (NetworkManager.Singleton && NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
        }
    }

public static void LoaderCallback()
    {
        SceneManager.LoadScene(targetScene.ToString());
    }
}