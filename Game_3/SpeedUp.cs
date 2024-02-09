using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedUp : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (GameManager.instance.battleManager.StatusBounusCanvasObj.activeInHierarchy)
        {
            Time.timeScale = 10f;
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        // タップを離したときに速度を元に戻す
        Time.timeScale = 1f;
    }
}
