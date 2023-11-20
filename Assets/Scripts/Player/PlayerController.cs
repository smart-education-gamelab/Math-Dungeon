using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    private PlayerMotor motor;

    private Vector2 mouseDelta = Vector2.zero;
    private bool isCollidingWithWall = false;

    private Camera mainCamera;
    private RaycastHit wallHit;

    void Start() {
        motor = GetComponent<PlayerMotor>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        mainCamera = Camera.main;
    }

    void Update() {
        float _xMov = Input.GetAxisRaw("Horizontal");
        float _zMov = Input.GetAxisRaw("Vertical");

        Vector3 _movHorizontal = transform.right * _xMov;
        Vector3 _movVertical = transform.forward * _zMov;

        Vector3 _velocity = (_movHorizontal + _movVertical).normalized * speed;

        motor.Move(_velocity);

        float _yRot = Input.GetAxisRaw("Mouse X");
        Vector3 _rotation = new Vector3(0f, _yRot, 0f) * lookSensitivity;
        motor.Rotate(_rotation);

        float _xRot = Input.GetAxisRaw("Mouse Y");

        mouseDelta = new Vector2(_yRot, _xRot) * -lookSensitivity;

        if(Mathf.Abs(mouseDelta.magnitude) < 0.01f) {
            motor.RotateCamera(Vector3.zero);
        } else {
            if(!isCollidingWithWall) {
                motor.RotateCamera(new Vector3(-mouseDelta.y, 0f, 0f));
            }
        }
    }

    void FixedUpdate() {
        CheckWallCollision();
    }

    void CheckWallCollision() {
        if(mainCamera != null) {
            Vector3 cameraPosition = mainCamera.transform.position;
            Vector3 cameraDirection = mainCamera.transform.forward;

            if(Physics.Raycast(cameraPosition, cameraDirection, out wallHit, Mathf.Infinity)) {
                if(wallHit.collider.CompareTag("Wall")) {
                    isCollidingWithWall = true;
                }
            } else {
                isCollidingWithWall = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("EndLevel")) {
            // De speler is in aanraking gekomen met een object met de tag "EndLevel"
            Debug.Log("Player reached end of level!");
            // Hier kun je code toevoegen om acties uit te voeren wanneer de speler het einde van het level bereikt.
            OnEndSceneServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void OnEndSceneServerRpc(ServerRpcParams serverRpcParams = default)
    {
        OnEndSceneClientRpc(serverRpcParams.Receive.SenderClientId);
        Loader.LoadNetwork(Loader.Scene.PuzzleTwoGears);
    }

    [ClientRpc]
    private void OnEndSceneClientRpc(ulong clientId)
    {
        OnEndSceneServerRpc();
    }
}
