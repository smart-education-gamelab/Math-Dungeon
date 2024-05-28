using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class PickupObject : NetworkBehaviour
{
    public Transform objectHolder;
    public LayerMask pickupLayer;
    public LayerMask snapLayer;
    public LayerMask activationLayer;

    public Camera cam;

    public AudioClip grabbedClip;
    public AudioSource grabbedSource;

    [SerializeField]
    private NetworkObject player;

    [SerializeField]
    private NetworkObject currentObject;

    private GameObject currentSnapPoint;

    private Image crosshairImage;

    private GearPuzzleController gearPuzzleController;

    [SerializeField]
    private float rayLength;

    private void Start()
    {
        crosshairImage = GameObject.FindWithTag("Crosshair").GetComponent<Image>();
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
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (currentObject == null)
                {
                    if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, rayLength, pickupLayer))
                    {
                        grabbedSource.Play();
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
                        DropObjectServerRpc(currentObject.NetworkObjectId);
                    }
                }
            }

            if (currentObject != null)
            {
                currentObject.transform.position = objectHolder.position;
                currentObject.transform.rotation = objectHolder.rotation;
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
                pickedObject.ChangeOwnership(clientId);
                currentObject = pickedObject;

                currentObject.GetComponent<Rigidbody>().isKinematic = true;

                // Enable SmoothNetworkTransform for smooth movement
                currentObject.GetComponent<SmoothNetworkTransform>().enabled = true;

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
            currentObject.GetComponent<SmoothNetworkTransform>().enabled = true;
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
            currentObject.GetComponent<Rigidbody>().isKinematic = false;
            currentObject.GetComponent<SmoothNetworkTransform>().enabled = false;
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
            obj.GetComponent<SmoothNetworkTransform>().enabled = false;
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

        SnapObjectClientRpc(snapPointTransform, currentObject.NetworkObjectId, didItWork);

        currentObject = null;
    }

    [ClientRpc]
    private void SnapObjectClientRpc(Vector3 snapPointTransform, ulong objectId, bool didItWork)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(objectId, out NetworkObject obj))
        {
            if (didItWork)
            {
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
}
