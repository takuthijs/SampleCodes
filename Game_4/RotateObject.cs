using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 30f; // 回転速度

    void Update()
    {
        // 子オブジェクトが存在する場合に回転
        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                // 子オブジェクトをy軸で回転
                child.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
            }
        }
    }
}
