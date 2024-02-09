using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public List<Vector3> cameraPos;
    public List<Vector3> cameraRotation;
    // Start is called before the first frame update
    void Start()
    {
        //ホーム画面のポジションとローテーションを設定
        SetPosition(0);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetPosition(int index)
    {
        transform.position = cameraPos[index];
        transform.rotation = Quaternion.Euler(cameraRotation[index].x, cameraRotation[index].y, cameraRotation[index].z);
    }
}
