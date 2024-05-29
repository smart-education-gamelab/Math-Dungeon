using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using System.Collections.Generic;

public class PickupObject : NetworkBehaviour
{
    [SerializeField]
    private Transform objectHolder; // The Transform that represents the position of the object holder
    [SerializeField]
    private LayerMask pickupLayer; // The layer of the objects that can be picked up
    [SerializeField]
    private LayerMask snapLayer; // The layer of snapping points
    [SerializeField]
    private LayerMask activationLayer; // The layer of activation points

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private AudioClip grabbedClip;
    [SerializeField]
    private AudioSource grabbedSource;

    [SerializeField]
    private NetworkObject player;

    [SerializeField]
    private NetworkObject currentObject; // The current picked-up object

    private GameObject currentSnapPoint; // The current snap-point in use

    private Image crosshairImage; // Reference to the image component of the crosshair

    private GearPuzzleController gearPuzzleController;

    [SerializeField]
    private float rayLength;

    private float syncInterval = 0.1f; // Interval in seconds to sync with the server
    private float lastSyncTime;

    private struct State
    {
        public float time;
        public Vector3 position;
        public Quaternion rotation;
    }

    private List<State> stateBuffer = new List<State>();

    private void Start()
    {
        crosshairImage = GameObject.FindWithTag("Crosshair").GetComponent<Image>();
        grabbedSource.clip = grabbedClip;

        if (SceneManager.GetActiveScene().name == Loader.Scene.PuzzleTwoGears.ToString())
        {
            gearPuzzleController = FindObjectOfType<GearPuzzleController>();
        }
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;

        UpdateCrosshairColor();

        if (SceneManager.GetActiveScene().name == Loader.Scene.PuzzleTwoGears.ToString())
        {
            HandlePickupAndDrop();
            UpdateHeldObjectPosition();
        }

        InterpolateObjectPosition();
    }

    private void UpdateCrosshairColor()
    {
        RaycastHit hit;

        if (TryRaycast(rayLength, pickupLayer, out hit))
        {
            crosshairImage.color = Color.green;
        }
        else if (TryRaycast(rayLength, snapLayer, out hit))
        {
            crosshairImage.color = Color.magenta;
        }
        else if (TryRaycast(rayLength, activationLayer, out hit))
        {
            crosshairImage.color = Color.blue;
        }
        else
        {
            crosshairImage.color = Color.white;
        }
    }

    private void HandlePickupAndDrop()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (currentObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                TrySnapOrDropObject();
            }
        }
    }

    private void TryPickUpObject()
    {
        RaycastHit hit;
        if (TryRaycast(rayLength, pickupLayer, out hit))
        {
            grabbedSource.Play();
            PickUpObjectServerRpc(hit.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    private void TrySnapOrDropObject()
    {
        RaycastHit hit;
        if (TryRaycast(rayLength, snapLayer, out hit))
        {
            currentSnapPoint = hit.transform.gameObject;
            SnapObjectServerRpc(hit.transform.position);
        }
        else
        {
            DropObjectServerRpc(currentObject.NetworkObjectId);
        }
    }

    private void UpdateHeldObjectPosition()
    {
        if (currentObject != null)
        {
            // Client-side prediction
            currentObject.transform.position = objectHolder.position;
            currentObject.transform.rotation = objectHolder.rotation;

            if (Time.time - lastSyncTime > syncInterval)
            {
                SyncObjectPositionServerRpc(objectHolder.position, objectHolder.rotation);
                lastSyncTime = Time.time;
            }
        }
    }

    private void InterpolateObjectPosition()
    {
        if (currentObject == null || stateBuffer.Count < 2) return;

        float targetTime = Time.time - 0.1f; // Target time to interpolate towards
        State latestState = stateBuffer[stateBuffer.Count - 1];
        State previousState = stateBuffer[stateBuffer.Count - 2];

        if (targetTime < previousState.time) return;

        float t = (targetTime - previousState.time) / (latestState.time - previousState.time);
        currentObject.transform.position = Vector3.Lerp(previousState.position, latestState.position, t);
        currentObject.transform.rotation = Quaternion.Slerp(previousState.rotation, latestState.rotation, t);
    }

    private bool TryRaycast(float length, LayerMask layer, out RaycastHit hit)
    {
        return Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, length, layer);
    }

    [ServerRpc(RequireOwnership = false)]
    private void PickUpObjectServerRpc(ulong objectId, ServerRpcParams serverRpcParams = default)
    {
        if (currentObject != null) return;

        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject pickedObject))
            {
                pickedObject.ChangeOwnership(clientId);
                currentObject = pickedObject;
                currentObject.GetComponent<Rigidbody>().isKinematic = true;
                currentObject.GetComponent<ClientNetworkTransform>().enabled = true;

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
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject pickedObject))
        {
            currentObject = pickedObject;
            currentObject.GetComponent<Rigidbody>().isKinematic = true;
            currentObject.GetComponent<ClientNetworkTransform>().enabled = true;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DropObjectServerRpc(ulong objectId, ServerRpcParams serverRpcParams = default)
    {
        if (currentObject == null) return;

        var clientId = serverRpcParams.Receive.SenderClientId;
        if (NetworkManager.ConnectedClients.ContainsKey(clientId))
        {
            currentObject.GetComponent<Rigidbody>().isKinematic = false;
            currentObject.GetComponent<ClientNetworkTransform>().enabled = false;
            currentObject.RemoveOwnership();

            DropObjectClientRpc(currentObject.NetworkObjectId);
            currentObject = null;
        }
    }

    [ClientRpc]
    private void DropObjectClientRpc(ulong objectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj))
        {
            obj.GetComponent<Rigidbody>().isKinematic = false;
            obj.GetComponent<ClientNetworkTransform>().enabled = false;
            currentObject = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SnapObjectServerRpc(Vector3 snapPointTransform)
    {
        if (currentObject == null) return;

        bool isSuccess = gearPuzzleController.CheckSnapPointFormula(currentSnapPoint, currentObject.GetComponentInChildren<TextMeshProUGUI>().text);

        if (isSuccess)
        {
            currentObject.transform.position = snapPointTransform;
            currentObject.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
            currentObject.gameObject.layer = 0;
            currentSnapPoint.gameObject.layer = 0;
        }
        else
        {
            currentObject.GetComponent<Rigidbody>().isKinematic = false;
        }

        SnapObjectClientRpc(snapPointTransform, currentObject.NetworkObjectId, isSuccess);
        currentObject = null;
    }

    [ClientRpc]
    private void SnapObjectClientRpc(Vector3 snapPointTransform, ulong objectId, bool isSuccess)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj))
        {
            if (isSuccess)
            {
                obj.transform.position = snapPointTransform;
                obj.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
                obj.gameObject.layer = 0;
                currentSnapPoint.gameObject.layer = 0;
            }
            else
            {
                obj.GetComponent<Rigidbody>().isKinematic = false;
            }
        }
    }

    [ServerRpc]
    private void SyncObjectPositionServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default)
    {
        if (currentObject != null)
        {
            currentObject.transform.position = position;
            currentObject.transform.rotation = rotation;

            // Add new state to buffer for interpolation
            State newState = new State
            {
                time = Time.time,
                position = position,
                rotation = rotation
            };
            stateBuffer.Add(newState);
            if (stateBuffer.Count > 20) stateBuffer.RemoveAt(0); // Keep buffer size manageable

            SyncObjectPositionClientRpc(position, rotation, Time.time);
        }
    }

    [ClientRpc]
    private void SyncObjectPositionClientRpc(Vector3 position, Quaternion rotation, float timestamp)
    {
        if (currentObject != null)
        {
            State newState = new State
            {
                time = timestamp,
                position = position,
                rotation = rotation
            };
            stateBuffer.Add(newState);
            if (stateBuffer.Count > 20) stateBuffer.RemoveAt(0); // Keep buffer size manageable
        }
    }
}
