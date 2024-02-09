using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainColider : MonoBehaviour
{
    public MeshCollider meshCollider;

    public void coliderSwitchTrigger()
    {
        StartCoroutine(coliderSwitch());
    }

    IEnumerator coliderSwitch()
    {
        meshCollider.isTrigger = true;
        yield return new WaitForSeconds(1);
        meshCollider.isTrigger = false;
    }
}
