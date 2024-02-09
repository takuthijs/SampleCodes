using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedUpButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    // ボタンを押した瞬間の処理
    public void OnPointerDown(PointerEventData eventData)
    {
        GameManager.instance.isSpeedUp = true;
        GameManager.instance.cameraSpeed = GameManager.instance.speedUp;
    }

    // ボタンを離した瞬間の処理
    public void OnPointerUp(PointerEventData eventData)
    {
        GameManager.instance.isSpeedUp = false;
        GameManager.instance.cameraSpeed = GameManager.instance.currentCameraSpeed;
    }
}
