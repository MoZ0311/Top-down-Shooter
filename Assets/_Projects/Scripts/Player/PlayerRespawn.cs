using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;

public class PlayerRespawn : NetworkBehaviour
{
    [Header("Setting")]
    [SerializeField] int dropItemCount; // 一度にドロップするアイテムの数
    [SerializeField] float respawnTime; // リスポーンの所要時間(現実時間)

    [Header("PickupItemSO")]
    [SerializeField] PickupItemSO pickupItem;

    [Header("Ref Level")]
    [SerializeField] PlayerLevel playerLevel;

    [Header("Ref Health")]
    [SerializeField] PlayerHealth playerHealth;

    [Header("Components")]
    [SerializeField] ClientNetworkTransform clientNetworkTransform;
    [SerializeField] GameObject model;
    [SerializeField] GameObject trackingUI;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] CapsuleCollider playerCollider;

    public IEnumerator RespawnSequence()
    {
        // 死亡状態にする（クライアント全員に通知）
        SetActiveClientRpc(false);

        // レベルダウン
        playerLevel.CurrentLevel.Value = 1;

        // 経験値をばらまく
        Vector3 dropPosition = transform.position;
        dropPosition.y = 1;
        PickupSpawner.Instance.SpawnBurst(
            dropPosition,
            transform.localScale.x,
            playerLevel.CurrentLevel.Value * dropItemCount,
            pickupItem.PickupItemList[1]
        );

        TeleportClientRpc();

        yield return new WaitForSecondsRealtime(respawnTime);

        // HPを回復させて復活（クライアント全員に通知）
        SetActiveClientRpc(true);
        playerHealth.TakeDamage(-playerHealth.MaxHealth);
    }

    /// <summary>
    /// クライアント側で、見た目と当たり判定を設定する処理
    /// </summary>
    /// <param name="isActive">有効化するか</param>
    [ClientRpc]
    void SetActiveClientRpc(bool isActive)
    {
        // 見た目と当たり判定を切り替える
        model.SetActive(isActive);
        trackingUI.SetActive(isActive);
        playerRigidbody.useGravity = isActive;
        playerCollider.enabled = isActive;
    }

    /// <summary>
    /// クライアント側でテレポートする処理
    /// </summary>
    [ClientRpc]
    void TeleportClientRpc()
    {
        if (IsOwner)
        {
            int index = Random.Range(0, GameManager.Instance.SpawnPositions.Length);

            clientNetworkTransform.Teleport(
                GameManager.Instance.SpawnPositions[index].position,
                Quaternion.identity,
                transform.localScale
            );
        }
    }
}
