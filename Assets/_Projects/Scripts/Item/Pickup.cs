using UnityEngine;
using Unity.Netcode;
using DG.Tweening;

public abstract class Pickup : NetworkBehaviour
{
    [SerializeField] float moveDuration;    // スポーン時の演出の長さ
    Transform cameraTransform;              // ビルボードで参照するカメラのTransform
    bool hasPickedUp;                       // 既に拾われているか
    const string PlayerTag = "Player";

    public override void OnNetworkSpawn()
    {
        cameraTransform = Camera.main.transform;
        hasPickedUp = false;
        NetworkObject.DestroyWithScene = true;
        transform.DOMoveY(transform.localScale.y / 2, moveDuration)
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

        if (other.CompareTag(PlayerTag))
        {
            hasPickedUp = true;
            OnPickedUp(other);
            NetworkObject.Despawn();
        }
    }

    void LateUpdate()
    {
        // 常にカメラの正面を向かせる
        transform.LookAt(transform.position + cameraTransform.forward);
    }

    protected abstract void OnPickedUp(Collider collider);
}
