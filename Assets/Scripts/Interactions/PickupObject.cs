using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

public class PickupObject : NetworkBehaviour
{
    public Transform objectHolder; // The Transform that represents the position of the object holder
    public LayerMask pickupLayer; // The layer of the objects that can be picked up
    public LayerMask snapLayer; // The layer of snapping points
    public LayerMask activationLayer;

    public Camera cam;

    public AudioClip grabbedClip;
    public AudioSource grabbedSource;

    [SerializeField]
    private NetworkObject player;

    [SerializeField]
    private NetworkObject currentObject; // The current picked-up object

    private GameObject currentSnapPoint; // The current snap-point in use

    private Image crosshairImage; // Reference to the image component of the crosshair

    private GearPuzzleController gearPuzzleController;

    [SerializeField]
    private float rayLength;

    private Vector3 lastServerPosition;
    private Quaternion lastServerRotation;

    private float serverPositionUpdateTime = 0.1f;
    private float timeSinceLastUpdate = 0f;

    private void Start()
    {
        crosshairImage = GameObject.FindWithTag("Crosshair").GetComponent<Image>();
        // Get a reference to the GearPuzzleController
        if (SceneManager.GetActiveScene().name == Loader.Scene.PuzzleTwoGears.ToString())
        {
            gearPuzzleController = FindObjectOfType<GearPuzzleController>();
        }

        grabbedSource.clip = grabbedClip;
    }

    private void Update()
    {
        if (!IsLocalPlayer)
            return;

        // Raycast to select and pick up an object
        Debug.DrawRay(cam.transform.position, cam.transform.forward, Color.red, 100f);
        RaycastHit hit;

        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, pickupLayer))
        {
            crosshairImage.color = Color.green;
            Debug.Log(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
        else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, snapLayer))
        {
            crosshairImage.color = Color.magenta;
        }
        else if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, activationLayer))
        {
            crosshairImage.color = Color.blue;
        }
        else
        {
            crosshairImage.color = Color.white;
        }

        if (SceneManager.GetActiveScene().name == Loader.Scene.PuzzleTwoGears.ToString())
        {
            // Check if the player presses the pickup button
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentObject == null)
                {
                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, pickupLayer))
                    {
                        grabbedSource.Play();
                        // Send an RPC to the server to pick up the object
                        PickUpObjectServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
                    }
                }
                else
                {
                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity, snapLayer))
                    {
                        currentSnapPoint = hit.transform.gameObject;
                        SnapObjectServerRpc(hit.transform.position);
                    }
                    else
                    {
                        // Send an RPC to the server to drop the object
                        DropObjectServerRpc(currentObject.NetworkObjectId);
                    }
                }
            }

            if (currentObject != null)
            {
                // Update the position and rotation of the held object to match the object holder
                currentObject.transform.position = Vector3.Lerp(currentObject.transform.position, objectHolder.position, Time.deltaTime * 10f);
                currentObject.transform.rotation = Quaternion.Lerp(currentObject.transform.rotation, objectHolder.rotation, Time.deltaTime * 10f);
            }
        }

        // Periodically update the server with the client's predicted position and rotation
        if (currentObject != null)
        {
            timeSinceLastUpdate += Time.deltaTime;
            if (timeSinceLastUpdate >= serverPositionUpdateTime)
            {
                timeSinceLastUpdate = 0f;
                UpdateObjectPositionServerRpc(currentObject.transform.position, currentObject.transform.rotation);
            }
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
                // Mark the object as picked up
                pickedObject.ChangeOwnership(clientId);
                currentObject = pickedObject;

                currentObject.GetComponent<Rigidbody>().isKinematic = true;

                // Enable ClientNetworkTransform for smooth movement
                var cnt = currentObject.GetComponent<ClientNetworkTransform>();
                if (cnt != null)
                {
                    cnt.enabled = true;
                }

                lastServerPosition = currentObject.transform.position;
                lastServerRotation = currentObject.transform.rotation;

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
            currentObject.GetComponent<Rigidbody>().isKinematic = true;
            // Enable ClientNetworkTransform for smooth movement
            var cnt = currentObject.GetComponent<ClientNetworkTransform>();
            if (cnt != null)
            {
                cnt.enabled = true;
            }

            lastServerPosition = currentObject.transform.position;
            lastServerRotation = currentObject.transform.rotation;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropObjectServerRpc(ulong objectId, ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;

        if (currentObject == null)
            return;

        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            // Unmark the object as picked up and let it drop
            currentObject.GetComponent<Rigidbody>().isKinematic = false;
            var cnt = currentObject.GetComponent<ClientNetworkTransform>();
            if (cnt != null)
            {
                cnt.enabled = false;
            }
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
            var cnt = obj.GetComponent<ClientNetworkTransform>();
            if (cnt != null)
            {
                cnt.enabled = false;
            }
            currentObject = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SnapObjectServerRpc(Vector3 snapPointTransform)
    {
        if (currentObject == null)
            return;

        bool didItWork;

        if (gearPuzzleController.CheckSnapPointFormula(currentSnapPoint, currentObject.GetComponentInChildren<TextMeshProUGUI>().text))
        {
            // Perform actions for correctly placed gears
            currentObject.transform.position = snapPointTransform;
            currentObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
            Debug.Log("GOEDZO 1");
            didItWork = true;
            currentObject.gameObject.layer = 0;
            currentSnapPoint.gameObject.layer = 0;
        }
        else
        {
            currentObject.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            Debug.Log("FOUTZO 1");
            didItWork = false;
        }

        // Send an RPC to all clients to synchronize the changes in the picked-up object
        SnapObjectClientRpc(snapPointTransform, currentObject.NetworkObjectId, didItWork);

        currentObject = null;
    }

    [ClientRpc]
    private void SnapObjectClientRpc(Vector3 snapPointTransform, ulong objectId, bool didItWork)
    {
        // Unmark the object as picked up and let it drop
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj))
        {
            if (didItWork)
            {
                // Perform actions for correctly placed gears
                obj.transform.position = snapPointTransform;
                obj.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                obj.gameObject.layer = 0;
                currentSnapPoint.gameObject.layer = 0;
                Debug.Log("GOEDZO 2");
            }
            else
            {
                obj.GetComponent<Rigidbody>().isKinematic = false;
                Debug.Log("FOUTZO 2");
            }
        }
    }

    [ServerRpc]
    private void UpdateObjectPositionServerRpc(Vector3 position, Quaternion rotation)
    {
        if (currentObject != null)
        {
            currentObject.transform.position = position;
            currentObject.transform.rotation = rotation;
            lastServerPosition = position;
            lastServerRotation = rotation;
            UpdateObjectPositionClientRpc(position, rotation);
        }
    }

    [ClientRpc]
    private void UpdateObjectPositionClientRpc(Vector3 position, Quaternion rotation)
    {
        if (currentObject != null && !IsOwner)
        {
            currentObject.transform.position = position;
            currentObject.transform.rotation = rotation;
        }
    }
}
