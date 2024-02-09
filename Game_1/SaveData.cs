using System;
using System.Collections.Generic;
using static GameManager;

[Serializable]
public class SaveData
{
    public string playerName ;
    public int playerRank;//プレイヤーのランク

    public int currentPlayerExp;
    public int nextPlayerExp;
    public List<int> lobbyCharaTrainingIds;
    public int jem ;//ジェム
    public int maxStamina;//MAXスタミナ
    public int currentStamina;//MAXスタミナ
    public int gold;//お金
    public int getCharacterIndex = 10;//今までゲットしたキャラの数11からスタート
    public int getEquipIndex = 10;
    public List<GetCharaParam> getCharaParams;
    public int selectPartyNumber;//選択中のパーティー番号
    //public int selectCardNumber;//選択中のカード番号
    public List<GetEquipParam> getEquipParams ;//持っている装備情報
    public List<Party> playerParty = new List<Party>();//プレイヤーのパーティー情報
    public List<StagePlayerInfo> stageInfo;
    public List<string> miniGameInfoKey;
    public List<int> miniGameInfoValue;
    //public Dictionary<string, int> miniGamePlayInfo = new Dictionary<string, int>();//ミニゲームのクリア状況
}
