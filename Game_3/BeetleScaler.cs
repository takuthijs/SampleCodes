using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleScaler : MonoBehaviour
{
    //成長する部位をしまう
    public List<GameObject> powerGrowObjects;
    public List<GameObject> interGrowObjects;
    public List<GameObject> guardGrowObjects;
    public List<GameObject> speedGrowObjects;

    [HideInInspector] public int power = 0;
    [HideInInspector] public int inter = 0;
    [HideInInspector] public int guard = 0;
    [HideInInspector] public int speed = 0;

    private float growScale = 0;

    Vector3 defaultScale = new Vector3(1, 1, 1);

    public void ScaleChange(bool training= false)
    {
        Debug.Log(gameObject.name);
        //スケールを一旦元通りにする
        ResetScale();

        //足の位置の調整をする
        AdjustParentPosition adjustParentPosition = gameObject.GetComponent<AdjustParentPosition>();
        if (adjustParentPosition is not null) adjustParentPosition.SetInitialPosition(training);

        growScale = 0.00025f;
        //セレクトした昆虫のパワー分powerGrowObjectsのスケールが大きくなる
        foreach (GameObject pbj in powerGrowObjects)
        {
            pbj.transform.localScale += new Vector3(growScale * power, growScale * power, growScale * power);
        }

        growScale = 0.0008f;
        //セレクトした昆虫のインテリ分interGrowObjectsのスケールが大きくなる
        foreach (GameObject pbj in interGrowObjects)
        {
            pbj.transform.localScale += new Vector3(growScale * inter, growScale * inter, growScale * inter);
        }

        growScale = 0.0008f;
        //セレクトした昆虫のガード分guardGrowObjectsのスケールが大きくなる
        foreach (GameObject pbj in guardGrowObjects)
        {
            pbj.transform.localScale += new Vector3(growScale * guard, growScale * guard, growScale * guard);
        }

        growScale = 0.0001f;
        //セレクトした昆虫のスピード分speedGrowObjectsのスケールが大きくなる
        foreach (GameObject pbj in speedGrowObjects)
        {
            pbj.transform.localScale += new Vector3(growScale * speed, growScale * speed, growScale * speed);
        }

        if (adjustParentPosition is not null) adjustParentPosition.ChangePosition();
    }

    void ResetScale()
    {
        foreach (GameObject pbj in powerGrowObjects)
        {
            pbj.transform.localScale = defaultScale;
        }

        foreach (GameObject pbj in interGrowObjects)
        {
            pbj.transform.localScale = defaultScale;
        }

        foreach (GameObject pbj in guardGrowObjects)
        {
            pbj.transform.localScale = defaultScale;
        }

        foreach (GameObject pbj in speedGrowObjects)
        {
            pbj.transform.localScale = defaultScale;
        }
    }
}
