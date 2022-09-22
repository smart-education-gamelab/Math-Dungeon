using Unity.Netcode;
using UnityEngine;

public class InteractableMechanism : NetworkBehaviour {
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public virtual void Activate() {
        Debug.Log("Now activating mechanism");
    }
}