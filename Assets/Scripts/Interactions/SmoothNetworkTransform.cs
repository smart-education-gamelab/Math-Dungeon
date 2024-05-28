using UnityEngine;
using Unity.Netcode;

public class SmoothNetworkTransform : NetworkBehaviour
{
    public NetworkVariable<Vector3> NetworkPosition = new NetworkVariable<Vector3>();
    public NetworkVariable<Quaternion> NetworkRotation = new NetworkVariable<Quaternion>();

    private void Update()
    {
        if (IsOwner)
        {
            NetworkPosition.Value = transform.position;
            NetworkRotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, NetworkPosition.Value, Time.deltaTime * 10f);
            transform.rotation = Quaternion.Lerp(transform.rotation, NetworkRotation.Value, Time.deltaTime * 10f);
        }
    }
}