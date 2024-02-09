using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3[] positions;
    public Vector3[] rotations;


    public bool isFocusBall = false;
    public GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(isFocusBall && ball != null)
        {
            var ballPos = new Vector3(ball.transform.position.x, ball.transform.position.y, ball.transform.position.z -10);
            transform.position = ballPos;
        }
    }

    public void SetPosition(int index)
    {
        transform.position = positions[index];
        transform.rotation = Quaternion.Euler(rotations[index].x, rotations[index].y, rotations[index].z);
    }

    public void foucusBall(GameObject _ball)
    {
        isFocusBall = true;
        ball = _ball;
        transform.rotation = Quaternion.Euler(Vector3.zero);

    }
}
