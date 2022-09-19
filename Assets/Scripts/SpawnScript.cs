using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnScript : MonoBehaviour
{
    [SerializeField]
    GameObject myPrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject Player = Instantiate(myPrefab, Vector3.zero, Quaternion.identity);
        Player.GetComponent<NetworkObject>().Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
