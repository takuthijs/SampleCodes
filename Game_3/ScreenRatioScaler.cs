using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

//横幅を調整するスクリプト
public class ScreenRatioScaler : MonoBehaviour
{
    public CanvasScaler canvasScaler;

    private void Start()
    {
        float screenRatio = (float)Screen.height / Screen.width;

        if (Mathf.Approximately(screenRatio, 2f))
        {
            canvasScaler.matchWidthOrHeight = 0f;
        }
        else if (screenRatio > 1f)
        {
            //1:1に近づくにつれて1になるように調整
            canvasScaler.matchWidthOrHeight = 1f - (screenRatio - 1f);
            if(canvasScaler.matchWidthOrHeight < 0)
            {
                canvasScaler.matchWidthOrHeight = 0;
            }
        }
    }
}
