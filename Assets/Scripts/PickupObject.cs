using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class PickupObject : NetworkBehaviour
{
    public Transform objectHolder; // The Transform that represents the position of the object holder
    public LayerMask pickupLayer; // The layer of the objects that can be picked up
    public LayerMask snapLayer; // The layer of snapping points
    public Camera cam;

    [SerializeField]
    private NetworkObject player;

    [SerializeField]
    private NetworkObject currentObject; // The current picked-up object

    private GameObject currentSnapPoint; // The current snap-point in use

    private Image crosshairImage; // Referentie naar de image component van de crosshair

    private GearPuzzleController gearPuzzleController;

    [SerializeField]
    private float rayLength;

    private void Start() {
        crosshairImage = GameObject.FindWithTag("Crosshair").GetComponent<Image>();
        // Verkrijg een referentie naar de GearPuzzleController
        gearPuzzleController = FindObjectOfType<GearPuzzleController>();
    }

	private void Update()
    {
        if (!IsLocalPlayer)
            return;

        // Raycast to select and pick up an object
        Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.red, 100f);
        RaycastHit hit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, pickupLayer)) {
            crosshairImage.color = Color.green;
            Debug.Log(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        } else if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, snapLayer)) {
            crosshairImage.color = Color.magenta;
        } else {
            crosshairImage.color = Color.white;
        }

        // Check if the player presses the pickup button
        if (Input.GetKeyDown(KeyCode.E))
        {

            Debug.Log("Am I client? " + IsClient + " " + "Am I host? " + IsHost + " " + "Am I server? " + IsServer);
            Debug.Log(IsLocalPlayer);
            Debug.Log("I CLICKED E");


            if (currentObject == null)
            {
                Debug.Log("CURRENT OBJECT IS 0");
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, pickupLayer))
                {
                    // Send an RPC to the server to pick up the object
                    PickUpObjectServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
                    Debug.Log("PICKED UP");
                }
            }
            else
            {
                Debug.Log("Whoops");
                if(Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, snapLayer)) {
                    currentSnapPoint = hit.transform.gameObject;
                    SnapObjectServerRpc(hit.transform.position);
                } else {
                    // Send an RPC to the server to drop the object
                    DropObjectServerRpc(currentObject.NetworkObjectId);
                }
            }
        }

        if (currentObject != null)
        {
            // Update the position and rotation of the held object to match the object holder
            currentObject.transform.position = objectHolder.position;
            currentObject.transform.rotation = objectHolder.rotation;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpObjectServerRpc(ulong objectId, ServerRpcParams serverRpcParams = default)
    {
        if (currentObject != null)
            return;
        var clientId = serverRpcParams.Receive.SenderClientId;


        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject pickedObject))
            {
                Debug.Log(objectId);

                // Mark the object as picked up
                pickedObject.ChangeOwnership(clientId);
                currentObject = pickedObject;
                Debug.Log(pickedObject.OwnerClientId);
                currentObject.GetComponent<Rigidbody>().isKinematic = true;

                // Send an RPC to all clients to synchronize the changes in the picked-up object
                PickUpObjectClientRpc(currentObject.NetworkObjectId);
            }
            else
            {
                Debug.LogError($"Failed to find object with NetworkObjectId: {objectId}");
            }
        }
        
    }

    [ClientRpc]
    private void PickUpObjectClientRpc(ulong objectId)
    {
        // Mark the object as picked up
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject pickedObject))
        {
            currentObject = pickedObject;
            //currentObject.GetComponent<Rigidbody>().isKinematic = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropObjectServerRpc(ulong objectId, ServerRpcParams serverRpcParams = default) {

        var clientId = serverRpcParams.Receive.SenderClientId;
        if (currentObject == null)
            return;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            var client = NetworkManager.ConnectedClients[clientId];
            // Unmark the object as picked up and let it drop
            currentObject.GetComponent<Rigidbody>().isKinematic = false;
            currentObject.RemoveOwnership();
            // Send an RPC to all clients to synchronize the changes in the picked-up object
            DropObjectClientRpc(currentObject.NetworkObjectId);
            
            currentObject = null;
        }
    }

    [ClientRpc]
    private void DropObjectClientRpc(ulong objectId)
    {
        // Unmark the object as picked up and let it drop
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj))
        {
            obj.GetComponent<Rigidbody>().isKinematic = false;
            currentObject = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SnapObjectServerRpc(Vector3 snapPointTransform) {
        if(currentObject == null)
            return;

        // Unmark the object as picked up and let it drop
        currentObject.transform.position = snapPointTransform;
        currentObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

        bool didItWork;

        if(gearPuzzleController.CheckSnapPointFormula(currentSnapPoint, currentObject.GetComponentInChildren<TextMeshProUGUI>().text)) {
            // Voer hier acties uit voor correct geplaatste tandwielen
            Debug.Log("GOEDZO 1");
            didItWork = true;
            currentObject.gameObject.layer = 0;
            currentSnapPoint.gameObject.layer = 0;
        } else {
            currentObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            Debug.Log("FOUTZO 1");
            didItWork = false;
        }

        // Send an RPC to all clients to synchronize the changes in the picked-up object
        SnapObjectClientRpc(snapPointTransform, currentObject.NetworkObjectId, didItWork);

        currentObject = null;
    }

    [ClientRpc]
    private void SnapObjectClientRpc(Vector3 snapPointTransform, ulong objectId, bool didItWork) {
        // Unmark the object as picked up and let it drop
        if(NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj)) {
            obj.transform.position = snapPointTransform;
            currentObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

            if(didItWork) {
                // Voer hier acties uit voor correct geplaatste tandwielen
                currentObject.gameObject.layer = 0;
                currentSnapPoint.gameObject.layer = 0;
                Debug.Log("GOEDZO 2");
            } else {
                currentObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                Debug.Log("FOUTZO 1");
            }
        }
    }
}