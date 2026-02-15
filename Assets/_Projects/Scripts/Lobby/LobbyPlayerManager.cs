using UnityEngine;

public class LobbyPlayerManager : MonoBehaviour
{
    [SerializeField] GameObject[] lobbyPlayers = new GameObject[4];

    /// <summary>
    /// 接続人数に合わせてモデルの表示を更新する
    /// </summary>
    /// <param name="count">現在の接続人数</param>
    public void UpdatePlayerVisuals(int count)
    {
        for (int i = 0; i < lobbyPlayers.Length; ++i)
        {
            // 人数以下のインデックスのモデルを有効化
            lobbyPlayers[i].SetActive(i < count);
        }
    }
}
