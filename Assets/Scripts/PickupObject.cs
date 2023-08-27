using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class PickupObject : NetworkBehaviour
{
    public Transform objectHolder; // The Transform that represents the position of the object holder
    public LayerMask pickupLayer; // The layer of the objects that can be picked up
    public Camera cam;

    [SerializeField]
    private NetworkObject currentObject; // The current picked-up object

    [SerializeField]
    private Image crosshairSprite; // Referentie naar het sprite renderer component van het puntje

    private void Update()
    {
        if (!IsLocalPlayer)
            return;

        // Check if the player presses the pickup button
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentObject == null)
            {
                // Raycast to select and pick up an object
                Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.red, 100f);
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, pickupLayer))
                {
                    crosshairSprite.color = Color.blue;
                    // Send an RPC to the server to pick up the object
                    PickUpObjectServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
                }
            }
            else
            {
                // Send an RPC to the server to drop the object
                DropObjectServerRpc();
            }
        }

        if (currentObject != null)
        {
            // Update the position and rotation of the held object to match the object holder
            currentObject.transform.position = objectHolder.position;
            currentObject.transform.rotation = objectHolder.rotation;
        }
    }

    [ServerRpc]
    private void PickUpObjectServerRpc(ulong objectId)
    {
        if (currentObject != null)
            return;

        // Check if the dictionary contains the key
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject pickedObject))
        {
            // Mark the object as picked up
            currentObject = pickedObject;
            currentObject.GetComponent<Rigidbody>().isKinematic = true;

            // Send an RPC to all clients to synchronize the changes in the picked-up object
            PickUpObjectClientRpc(currentObject.NetworkObjectId);
        }
        else
        {
            Debug.LogError($"Failed to find object with NetworkObjectId: {objectId}");
        }
    }

    [ClientRpc]
    private void PickUpObjectClientRpc(ulong objectId)
    {
        // Mark the object as picked up
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj))
        {
            currentObject = obj;
            currentObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    [ServerRpc]
    private void DropObjectServerRpc()
    {
        if (currentObject == null)
            return;

        // Unmark the object as picked up and let it drop
        currentObject.GetComponent<Rigidbody>().isKinematic = false;

        // Send an RPC to all clients to synchronize the changes in the picked-up object
        DropObjectClientRpc(currentObject.NetworkObjectId);

        currentObject = null;
    }

    [ClientRpc]
    private void DropObjectClientRpc(ulong objectId)
    {
        // Unmark the object as picked up and let it drop
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj))
        {
            obj.GetComponent<Rigidbody>().isKinematic = false;
        }
    }
}