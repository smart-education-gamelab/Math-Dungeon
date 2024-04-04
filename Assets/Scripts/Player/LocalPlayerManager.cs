using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LocalPlayerManager : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentToDisable;

    [SerializeField]
    Camera sceneCamera;

    [Header("Player one puzzle 1")]
    [SerializeField]
    private Vector3 spawnPositionPlayerOne;
    [SerializeField]
    private Quaternion spawnRotationPlayerOne;

    [Header("Player two puzzle 1")]
    [SerializeField]
    private Vector3 spawnPositionPlayerTwo;
    [SerializeField] 
    private Quaternion spawnRotationPlayerTwo;

    [Header("Player one puzzle 2")]
    [SerializeField]
    private Vector3 spawnPositionPlayerOne2;
    [SerializeField]
    private Quaternion spawnRotationPlayerOne2;

    [Header("Player two puzzle 2")]
    [SerializeField]
    private Vector3 spawnPositionPlayerTwo2;
    [SerializeField]
    private Quaternion spawnRotationPlayerTwo2;

    [Header("Player one puzzle 4")]
    [SerializeField]
    private Vector3 spawnPositionPlayerOne4;
    [SerializeField]
    private Quaternion spawnRotationPlayerOne4;

    [Header("Player two puzzle 4")]
    [SerializeField]
    private Vector3 spawnPositionPlayerTwo4;
    [SerializeField]
    private Quaternion spawnRotationPlayerTwo4;

    // Start is called before the first frame update
    void Start()
    {
        if (!IsLocalPlayer)
        {
            for (int i = 0; i < componentToDisable.Length; i++)
            {
                componentToDisable[i].enabled = false;
            }
        }
        else 
        {
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }
            
        }

        if(SceneManager.GetActiveScene().name == Loader.Scene.PuzzleOneDoors.ToString()) {
            if(IsHost) {
                this.gameObject.transform.position = spawnPositionPlayerOne;
                this.gameObject.transform.rotation = spawnRotationPlayerOne;
            } else {
                this.gameObject.transform.position = spawnPositionPlayerTwo;
                this.gameObject.transform.rotation = spawnRotationPlayerTwo;
            }
        } else if(SceneManager.GetActiveScene().name.Equals(Loader.Scene.PuzzleTwoGears.ToString())) {
            if(IsHost) {
                this.gameObject.transform.position = spawnPositionPlayerOne2;
                this.gameObject.transform.rotation = spawnRotationPlayerOne2;
            } else {
                this.gameObject.transform.position = spawnPositionPlayerTwo2;
                this.gameObject.transform.rotation = spawnRotationPlayerTwo2;
            }
        }


    }
    void OnDisable()
    {
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }
    }
}
