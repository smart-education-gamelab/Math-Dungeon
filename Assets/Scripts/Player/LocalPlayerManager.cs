using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LocalPlayerManager : NetworkBehaviour
{
    [SerializeField]
    Behaviour[] componentToDisable;

    [SerializeField]
    Camera sceneCamera;

    [Header("Player one")]
    [SerializeField]
    private Vector3 spawnPositionPlayerOne;
    [SerializeField]
    private Quaternion spawnRotationPlayerOne;

    [Header("Player two")]
    [SerializeField]
    private Vector3 spawnPositionPlayerTwo;
    [SerializeField] 
    private Quaternion spawnRotationPlayerTwo;
    
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

        if (IsHost)
        {
            this.gameObject.transform.position = spawnPositionPlayerOne;
            this.gameObject.transform.rotation = spawnRotationPlayerOne;
        }
        else
        {
            this.gameObject.transform.position = spawnPositionPlayerTwo;
            this.gameObject.transform.rotation = spawnRotationPlayerTwo;
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
