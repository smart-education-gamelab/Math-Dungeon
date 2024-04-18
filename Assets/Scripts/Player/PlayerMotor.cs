using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    private Vector3 velocity = Vector3.zero;
    private Vector3 rotation = Vector3.zero;
    private Vector3 cameraRotation = Vector3.zero;

    private Rigidbody rb;

    private bool canWalk;

    public bool CanWalk {
        get => canWalk;
        set => canWalk = value;
    }

   void Start()
    {
        rb = GetComponent<Rigidbody>();
        canWalk = true;
    }

    public void Move(Vector3 _velocity)
    {
        velocity = _velocity;
    }

    public void Rotate(Vector3 _rotation)
    {
        rotation = _rotation;
    }

    public void RotateCamera(Vector3 _cameraRotation)
    {
        cameraRotation = _cameraRotation;
    }

    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    void PerformMovement()
    {
        if(velocity != Vector3.zero && canWalk)
        {
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
            //Debug.Log(this.transform.position);
        }
    }

    void PerformRotation()
    {
        if(canWalk) {
            rb.MoveRotation(transform.rotation * Quaternion.Euler(rotation));
        }
        if(cam != null && canWalk)
        {
            cam.transform.Rotate(-cameraRotation);
        }
    }
}
