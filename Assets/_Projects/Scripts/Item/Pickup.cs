using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public abstract class Pickup : NetworkBehaviour
{
    bool hasPickedUp;

    public override void OnNetworkSpawn()
    {
        hasPickedUp = false;
        NetworkObject.DestroyWithScene = true;
        transform.DOMoveY(transform.localScale.y / 2, 1f)
            .SetEase(Ease.OutBounce)
            .SetLink(gameObject);
    }

    public override void OnNetworkDespawn()
    {
        // ネットワークから切り離されたら、画面上からも隠す
        transform.DOComplete();
        gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        // サーバー以外での衝突判定や、既に取得フラグが立っている場合はreturn
        if (!IsServer || hasPickedUp)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            hasPickedUp = true;
            OnPickedUp();
            NetworkObject.Despawn();
        }
    }

    void LateUpdate()
    {
        // 常にカメラの正面を向かせる
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }

    protected abstract void OnPickedUp();
}
