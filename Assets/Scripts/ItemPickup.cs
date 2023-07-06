using Unity.Netcode;
using UnityEngine;

public class ItemPickup : NetworkBehaviour {
    private void Update() {
        if(!IsServer && !IsClient)
            return;

        if((IsServer && IsClient) || IsOwner) {
            if(Input.GetKeyDown(KeyCode.F)) {
                Collider[] colliders = Physics.OverlapSphere(transform.position, 2f);
                foreach(Collider collider in colliders) {
                    if(collider.CompareTag("Player")) {
                        Inventory playerInventory = collider.GetComponent<Inventory>();
                        if(playerInventory != null) {
                            NetworkObject networkObject = GetComponent<NetworkObject>();
                            if(networkObject != null) {
                                if(IsServer) {
                                    playerInventory.AddItemServerRpc(networkObject.NetworkObjectId);
                                } else if(IsClient) {
                                    playerInventory.AddItemClientRpc(networkObject.NetworkObjectId);
                                }
                            } else {
                                Debug.LogWarning("Item is missing NetworkObject component!");
                            }
                            if(IsServer) {
                                playerInventory.RemoveItemServerRpc(networkObject.NetworkObjectId);
                            } else if(IsClient) {
                                playerInventory.RemoveItemClientRpc(networkObject.NetworkObjectId);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }
}