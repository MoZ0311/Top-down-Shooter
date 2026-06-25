using UnityEngine;
using Unity.Netcode;
using System.Collections;
using Unity.Multiplayer.Center.NetcodeForGameObjectsExample;
using UnityEngine.UIElements;

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
    [SerializeField] PanelRenderer trackingUI;
    [SerializeField] PanelRenderer playerUI;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] CapsuleCollider playerCollider;

    int prevRespawnIndex;   // 直前のリスポーンポイントのインデックス

    public IEnumerator RespawnSequence()
    {
        if (!IsServer)
        {
            yield break;
        }

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

        yield return null;

        TeleportToRandomPosition();

        yield return new WaitForSecondsRealtime(respawnTime);

        // HPを回復させて復活（クライアント全員に通知）
        playerHealth.RestoreHealth();
        SetActiveClientRpc(true);

        // カメラも戻す
        ResetCameraClientRpc();
    }

    void TeleportToRandomPosition()
    {
        int length = GameManager.Instance.SpawnPositions.Length;

        // スポーンポイントが2つ以上ない場合は、スキップ処理ができないため通常の処理をする
        if (length <= 1)
        {
            prevRespawnIndex = 0;
            if (length == 1)
            {
                TeleportClientRpc(GameManager.Instance.SpawnPositions[0].position);
            }
            return;
        }

        // 総数 - 1の範囲でランダムな値を決める
        int index = Random.Range(0, length - 1);

        // 選ばれた値が「前回と同じかそれ以上」なら、1つずらして前回分をスキップする
        if (index >= prevRespawnIndex)
        {
            index++;
        }

        // 次回のために現在のインデックスを保存
        prevRespawnIndex = index;
        TeleportClientRpc(GameManager.Instance.SpawnPositions[index].position);
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
        playerRigidbody.isKinematic = !isActive;
        playerCollider.enabled = isActive;

        trackingUI.enabled = isActive;

        // オーナーであれば、UIも消す
        if (IsOwner)
        {
            playerUI.gameObject.SetActive(isActive);
        }
    }

    /// <summary>
    /// クライアント側でテレポートする処理
    /// </summary>
    [ClientRpc]
    void TeleportClientRpc(Vector3 targetPosition)
    {
        if (IsOwner)
        {
            //transform.position = targetPosition;

            if (clientNetworkTransform != null)
            {
                clientNetworkTransform.Teleport(targetPosition, transform.rotation, transform.localScale);
            }
            else
            {
                // 万が一コンポーネントが参照できない場合のフォールバック
                transform.position = targetPosition;
            }
        }
    }

    /// <summary>
    /// クライアント側でカメラを戻す処理
    /// </summary>
    [ClientRpc]
    void ResetCameraClientRpc()
    {
        if (IsOwner)
        {
            CameraManager.Instance.SwitchCamera(CameraMode.Player);
        }
    }
}
