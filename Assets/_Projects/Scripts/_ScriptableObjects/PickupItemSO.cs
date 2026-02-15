using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PickupItemSO", menuName = "Scriptable Objects/PickupItemSO")]
public class PickupItemSO : ScriptableObject
{
    [SerializeField] List<NetworkObject> pickupItemList = new();
    public List<NetworkObject> PickupItemList => pickupItemList;
}
