using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct GearData : IEquatable<GearData>, INetworkSerializable
{
    public FixedString64Bytes gearText;
    public FixedString64Bytes gearId;
    public FixedString64Bytes solutionText;
    public FixedString64Bytes solutionId;

    public bool Equals(GearData other)
    {
        return
            gearText == other.gearText;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref gearText);
    }
}
