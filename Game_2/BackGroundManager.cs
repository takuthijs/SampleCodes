using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public List<GameObject> backgroundObjects;
    public GameManager gameManager;
    public Vector3 swapPos = new Vector3(1300, 0, 0);

    public void SwapBackgroundObjects()
    {
        if(gameManager.intDiff >= 1300)
        {
            gameManager.swapBackgroundObjCount++;

            //配列の最初のオブジェクトのx座座標を1300m*配列数して最初の配列を最後に移動する
            backgroundObjects[0].transform.position += swapPos * backgroundObjects.Count;
            backgroundObjects.Add(backgroundObjects[0]);
            backgroundObjects.RemoveAt(0);
        }
    }
}