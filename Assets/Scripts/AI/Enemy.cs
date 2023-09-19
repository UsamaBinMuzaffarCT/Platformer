using Unity.Netcode;
using UnityEngine;

public class Enemy : NetworkBehaviour
{
    public NetworkVariable<int> roomID = new NetworkVariable<int>(-3,readPerm:NetworkVariableReadPermission.Everyone,writePerm:NetworkVariableWritePermission.Server);
}
