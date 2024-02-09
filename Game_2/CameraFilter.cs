using UnityEngine;

[ExecuteInEditMode]
public class CameraFilter : MonoBehaviour
{
    [SerializeField] private Material filter;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        Debug.Log("実行");
        Graphics.Blit(src, dest, filter);
    }
}