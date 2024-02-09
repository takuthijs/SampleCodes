using System;
using System.Collections.Generic;
//using static GameManager;

[Serializable]
public class SaveData
{
    public string breederName ;//プレイヤーの名前

    //public int currentPlayerExp;
    //public int nextPlayerExp;
    //public List<int> lobbyCharaTrainingIds;
    public int coin;//お金
    //public List<GetCharaParam> getCharaParams;
    public BeetleManager.Beetle selectBeetle;//選択中の昆虫情報
    ////public int selectCardNumber;//選択中のカード番号
    public List<BeetleManager.Beetle> myBeetle = new List<BeetleManager.Beetle>();//自分の昆虫情報
    public List<GameManager.SkillInfo> mySkills ;//持っているスキル情報
    //public List<StagePlayerInfo> stageInfo;
    //public List<string> miniGameInfoKey;
    //public List<int> miniGameInfoValue;
}
