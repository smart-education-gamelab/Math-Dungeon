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

    [SerializeField]
    private Vector3 spawnPositionPlayerOne;

    [SerializeField]
    private Vector3 spawnPositionPlayerTwo;
    
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

        if (IsOwnedByServer)
        {
            this.gameObject.transform.position = spawnPositionPlayerOne;
            this.gameObject.transform.rotation = Quaternion.identity;
        }
        else
        {
            this.gameObject.transform.position = spawnPositionPlayerTwo;
            this.gameObject.transform.rotation = Quaternion.identity;
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
