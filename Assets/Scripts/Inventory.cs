using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class Inventory : NetworkBehaviour {
    public int maxItems = 10;
    [SerializeField]
    private List<ulong> itemIds = new List<ulong>();
    private int selectedItemIndex = 0;

    private void Update() {
        if(!IsLocalPlayer && !IsOwner)
            return;

        float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
        if(scrollWheel > 0f) {
            SelectNextItem();
        } else if(scrollWheel < 0f) {
            SelectPreviousItem();
        }
    }

    [ServerRpc]
    public void AddItemServerRpc(ulong itemId) {
        if(!IsServer)
            return;

        if(itemIds.Count < maxItems) {
            itemIds.Add(itemId);
            AddItemClientRpc(itemId);
        } else {
            Debug.Log("Inventory is full!");
        }
    }

    [ClientRpc]
    public void AddItemClientRpc(ulong itemId) {
        if(!IsClient)
            return;

        itemIds.Add(itemId);

        // Do any necessary client-side logic here
        Debug.Log("Item added to inventory: " + itemId.ToString());
    }

    [ServerRpc]
    public void RemoveItemServerRpc(ulong itemId) {
        if(!IsServer)
            return;

        if(itemIds.Contains(itemId)) {
            itemIds.Remove(itemId);
            RemoveItemClientRpc(itemId);
        }
    }

    [ClientRpc]
    public void RemoveItemClientRpc(ulong itemId) {
        if(!IsClient)
            return;

        if(itemIds.Contains(itemId)) {
            itemIds.Remove(itemId);

            // Do any necessary client-side logic here
            Debug.Log("Item removed from inventory: " + itemId.ToString());
        }
    }

    private void SelectNextItem() {
        selectedItemIndex++;
        if(selectedItemIndex >= itemIds.Count) {
            selectedItemIndex = 0;
        }

        Debug.Log("Selected Item: " + itemIds[selectedItemIndex].ToString());
    }

    private void SelectPreviousItem() {
        selectedItemIndex--;
        if(selectedItemIndex < 0) {
            selectedItemIndex = itemIds.Count - 1;
        }

        Debug.Log("Selected Item: " + itemIds[selectedItemIndex].ToString());
    }
}
