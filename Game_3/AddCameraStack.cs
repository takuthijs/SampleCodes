using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

//シーン移動の際に3DモデルをUI上に出すカメラをメインカメラのstackに入れるスクリプト
public class AddCameraStack : MonoBehaviour
{
    [SerializeField]Camera UIcamera;
    void Start()
    {
        //シーン遷移でInspectorでセットしたものが消えてしまうものがあるので
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(UIcamera);
    }
}
