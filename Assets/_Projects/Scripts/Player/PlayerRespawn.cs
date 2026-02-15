using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerRespawn : NetworkBehaviour
{
    private static readonly WaitForSeconds waitForSeconds = new(1.5f);

    [Header("Ref Health")]
    [SerializeField] PlayerHealth playerHealth;

    [Header("Components")]
    [SerializeField] GameObject model;
    [SerializeField] GameObject trackingUI;
    [SerializeField] Rigidbody playerRigidbody;
    [SerializeField] CapsuleCollider playerCollider;

    public IEnumerator RespawnSequence()
    {
        // 死亡状態にする（クライアント全員に通知）
        SetActiveClientRpc(false);

        yield return waitForSeconds;

        // HPを回復させて復活（クライアント全員に通知）
        SetActiveClientRpc(true);
        playerHealth.TakeDamage(-playerHealth.MaxHealth);
    }

    [ClientRpc]
    void SetActiveClientRpc(bool isActive)
    {
        // 見た目と当たり判定を切り替える
        model.SetActive(isActive);
        trackingUI.SetActive(isActive);
        playerRigidbody.useGravity = isActive;
        playerCollider.enabled = isActive;
    }
}
