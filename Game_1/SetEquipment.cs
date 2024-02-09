using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using System.Linq;

public class SetEquipment : MonoBehaviour
{
    //private int unique_id;
    public Image characterImage;
    public GameObject weaponInfo;
    public Image weaponInfoImage;

    public Image selectWeaponImage;
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI skillSpeedText;
    public List<Image> equipImages;
    public List<GameObject> equipEmptyImages;//空のアイコンオブジェクト
    public List<EquipSlot> equipSlots;//キャラクター側のスロット(各ボタン)

    public List<EquipItems> equipItems;//アイテム側のスロット(各ボタン)

    public int selectEquipSlotsNumber;//キャラクター側の選択されている場所
    public int selectEquipItemNumber;//右側の自分が持っているアイテム一覧
    public int genarateNum;//何番目に生成されたオブジェクトか

    public List<Toggle> toggles;

    public Button setButton;

    public EquipObjectPool equipObjectPool;

    private void OnEnable()
    {
        foreach(GameManager.GetEquipParam getEquipParam in GameManager.instance.getEquipParams)
        {
            GameManager.instance.getEquipParamsSort.Add(getEquipParam);
        }

        //LoopScrollの影響か、エディター上をアクティブにしないと装備されていないものも非アクティブになってしまうので一旦ここで全てアクティブにしています。
        foreach (GameObject gameObject in GameManager.instance.generateItems)
        {
            gameObject.SetActive(true);
        }

        //一旦選択されているアイテムは非表示にする
        weaponInfo.SetActive(false);
        //選択中の装備スロットもリセット
        selectEquipSlotsNumber = -1;//-1だったらSETボタンを押せないようにする
        selectEquipItemNumber = -1;//-1だったらSETボタンを押せないようにする

        setButton.interactable = false;

        //選択中の画像を非アクティブにする
        foreach (EquipSlot slot in equipSlots)
        {
            slot.selectImage.SetActive(false);
        }

        //選択中画像を解除する(アイテム側)
        foreach (EquipItems item in GameManager.instance.setEquipment.equipItems)
        {
            item.selectImage.enabled = false;
        }

        //一旦初期状態に戻す
        for (int i = 0; i < 6; i++)
        {
            //空状態のゲームオブジェクトをアクティブにする
            equipImages[i].enabled = false;
            equipEmptyImages[i].SetActive(true);
        }

        //デフォルトはオールなので全てのタブをアクティブにしてAllが選択状態にする
        GameManager.GetCharaParam charaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == GameManager.instance.selectCharaInfoUniqueID);

        characterImage.sprite = charaParam.sprite;

        //キャラクターが装備しているユニークidでgetEquipItemからFind、findしてきた装備の画像をequipImageにセットさせる
        for (int i = 0; i < charaParam.equipItem.Count; i++)
        {
            if (charaParam.equipItem[i] != -1)
            {
                GameManager.GetEquipParam equipParam = GameManager.instance.getEquipParams.Find(x => x.unique_id == charaParam.equipItem[i]);
                equipImages[i].sprite = equipParam.sprite;

                equipImages[i].enabled = true;
                equipEmptyImages[i].SetActive(false);
            }
            else
            {
                //空状態のゲームオブジェクトをアクティブにする
                equipImages[i].enabled = false;
                equipEmptyImages[i].SetActive(true);
            }
        }

        //HomeのトグルをONにする
        foreach(Toggle toggle in toggles)
        {
            toggle.isOn = false;
        }
        toggles[0].isOn = true;
        //ソートする
        EquipItemSort(GameManager.instance.selectGenre);
    }

    //両方選択されていたらSET Buttonで武器をセットさせる
    public void SetEquip()
    {
        //無効にならない時があった場合の保険処理
        if (selectEquipSlotsNumber == -1 || selectEquipItemNumber == -1) return;
        //キャラクターにセットさせる
        GameManager.GetCharaParam charaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == GameManager.instance.selectCharaInfoUniqueID);
        int beforeSetItem = 0;
        if (charaParam.equipItem.Count > selectEquipSlotsNumber)
        {
            beforeSetItem = charaParam.equipItem[selectEquipSlotsNumber];
            charaParam.equipItem[selectEquipSlotsNumber] = selectEquipItemNumber;
        }
        else
        {
            charaParam.equipItem.Add(selectEquipItemNumber);
        }

        //キャラクターに装備されている装備のユニークIDに追加
        GameManager.instance.equipNowUniqueIdList.Add(selectEquipItemNumber);

        //キャラクターが装備しているユニークidでgetEquipItemからFind、findしてきた装備の画像をequipImageにセットさせる

        for (int i = 0; i < charaParam.equipItem.Count; i++)
        {
            if(charaParam.equipItem[i] != 0)
            {
                GameManager.GetEquipParam equipParam = GameManager.instance.getEquipParams.Find(x => x.unique_id == charaParam.equipItem[i]);
                equipImages[i].sprite = equipParam.sprite;

                equipImages[i].enabled = true;
                equipEmptyImages[i].SetActive(false);
            }
            else
            {
                //空状態のゲームオブジェクトをアクティブにする
                equipImages[i].enabled = false;
                equipEmptyImages[i].SetActive(true);
            }  
        }
        //右下の武器情報
        GameManager.instance.setEquipment.weaponInfo.SetActive(false);
        //セットしたオブジェクトを非アクティブにする
        Debug.Log(genarateNum);
        GameManager.instance.generateItems[genarateNum].SetActive(false);
        //もしそこにセットされていたアイテムがあったらアイテム欄の方をアクティブにする
        if (beforeSetItem != 0)
        {
            //キャラクターに装備されている装備のユニークIDに追加
            GameManager.instance.equipNowUniqueIdList.Remove(beforeSetItem);
        }

        //セレクトされているジャンルでソートする
        EquipItemSort(GameManager.instance.selectGenre);

        //選択中を解除する
        GameManager.instance.setEquipment.equipSlots[selectEquipSlotsNumber].selectImage.SetActive(false);
        setButton.interactable = false;
        selectEquipSlotsNumber = -1;//-1だったらSETボタンを押せないようにする
        selectEquipItemNumber = -1;//-1だったらSETボタンを押せないようにする

    }


    public void UnSetEquip()
    {
        //選択中画像を解除する(キャラ側)
        foreach (EquipSlot equipSlot in GameManager.instance.setEquipment.equipSlots)
        {
            equipSlot.selectImage.SetActive(false);
        }
        //選択中画像を解除する(アイテム側)
        foreach (EquipItems item in GameManager.instance.setEquipment.equipItems)
        {
            item.selectImage.enabled = false;
        }
        //右下の武器情報
        GameManager.instance.setEquipment.weaponInfo.SetActive(false);
        setButton.interactable = false;


        if (selectEquipSlotsNumber == -1) return;

        //選択されている装備を解除する
        GameManager.GetCharaParam charaParam = GameManager.instance.getCharaParams.Find(x => x.unique_id == GameManager.instance.selectCharaInfoUniqueID);
        if(selectEquipSlotsNumber < charaParam.equipItem.Count)
        {
            int unique_id = charaParam.equipItem[selectEquipSlotsNumber];
            charaParam.equipItem.RemoveAt(selectEquipSlotsNumber);
            GameManager.instance.equipNowUniqueIdList.Remove(unique_id);
            Debug.Log(GameManager.instance.equipNowUniqueIdList.Count);

            //解除されたらゲームオブジェクトをアクティブ状態ににする
            //Debug.Log("geneNum:"+GameManager.instance.getEquipParams.Find(x => x.unique_id == unique_id).genarateNum);

            //見た目の変更
            //一旦初期状態に戻す
            for (int i = 0; i < 6; i++)
            {
                //空状態のゲームオブジェクトをアクティブにする
                equipImages[i].enabled = false;
                equipEmptyImages[i].SetActive(true);
            }

            //キャラクターが装備しているユニークidでgetEquipItemからFind、findしてきた装備の画像をequipImageにセットさせる
            for (int i = 0; i < charaParam.equipItem.Count; i++)
            {
                if (charaParam.equipItem[i] != -1)
                {
                    GameManager.GetEquipParam equipParam = GameManager.instance.getEquipParams.Find(x => x.unique_id == charaParam.equipItem[i]);
                    equipImages[i].sprite = equipParam.sprite;

                    equipImages[i].enabled = true;
                    equipEmptyImages[i].SetActive(false);
                }
                else
                {
                    //空状態のゲームオブジェクトをアクティブにする
                    equipImages[i].enabled = false;
                    equipEmptyImages[i].SetActive(true);
                }
            }
            
            GameManager.instance.setEquipment.equipSlots[selectEquipSlotsNumber].selectImage.SetActive(false);
            //連打した時に処理されないようにする
            selectEquipSlotsNumber = -1;
            selectEquipItemNumber = -1;
        }
        else
        {
            //連打した時に処理されないようにする
            selectEquipSlotsNumber = -1;
            selectEquipItemNumber = -1;
        }

        //セレクトされているジャンルでソートする
        EquipItemSort(GameManager.instance.selectGenre);
    }

    public void EquipItemSort(string genre)
    {
        //選択されているジャンル
        GameManager.instance.selectGenre = genre;
        GameManager.instance.getEquipParamsSort.Clear();
        if (genre != "All")
        {
            GameManager.instance.getEquipParamsSort = GameManager.instance.getEquipParams.Where(x => x.genre == GameManager.instance.selectGenre).ToList<GameManager.GetEquipParam>();
        }
        else
        {
            foreach (GameManager.GetEquipParam getEquipParam in GameManager.instance.getEquipParams)
            {
                GameManager.instance.getEquipParamsSort.Add(getEquipParam);
            }
        }

        //選択中画像を解除する(アイテム側)
        foreach (EquipItems item in GameManager.instance.setEquipment.equipItems)
        {
            item.selectImage.enabled = false;
        }

        //プール処理をしている箇所で条件を更新して再度表示させる
        //TODO現状の問題点としてユニークID分を差し引いていないため、スクロールしないと装備が表示されない場合がある
        equipObjectPool.UpdateEquipItems();

        for (int i = 0; i < GameManager.instance.generateItems.Count; i++)
        {
            //一旦アクティブにする
            GameManager.instance.generateItems[i].SetActive(true);

            //すでにモンスターが装備しているものは非アクティブにする
            HashSet<int> setCharaEquipuniqueIdsSet = new HashSet<int>(GameManager.instance.equipNowUniqueIdList);
            if (setCharaEquipuniqueIdsSet.Contains(equipItems[i].equipParam.unique_id)) GameManager.instance.generateItems[i].SetActive(false);
        }
    }
}
