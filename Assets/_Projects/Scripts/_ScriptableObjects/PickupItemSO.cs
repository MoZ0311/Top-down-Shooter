using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PickupItemSO", menuName = "Scriptable Objects/PickupItemSO")]
public class PickupItemSO : ScriptableObject
{
    // ステージ上での生成アイテムをリスト型で格納
    [field:SerializeField] public List<NetworkObject> PickupItemList { get; private set; } = new();
}
