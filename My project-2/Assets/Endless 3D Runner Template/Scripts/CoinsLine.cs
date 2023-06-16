using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CoinsLine : MonoBehaviour {

    public GameObject coinPrefab;

    [Range(-180, 180)]
    public int rotation = 0;
    [Range(0, 100)]
    public int coinsNum = 10;
    public float distanceBetweenCoins = 1.5f;

    protected int prevRotation;    
    protected float prevDistance;
    protected int prevCoinsNum;
    protected List<GameObject> trajectoryPoints = new List<GameObject>();    
    protected Transform thisTransform;

    protected virtual void Awake()
    {
        if (Application.isPlaying)
            return;

        thisTransform = transform;
        if (thisTransform.childCount > 0)
        {
            while (thisTransform.childCount != 0)
            {
                DestroyImmediate(thisTransform.GetChild(0).gameObject);
            }
        }
        UpdateCoins();
    }

    protected virtual void Update()
    {
        if (Application.isPlaying)
            return;

        if (prevCoinsNum != coinsNum)
        {
            UpdateCoins();
            prevCoinsNum = coinsNum;
        }
        if (prevDistance != distanceBetweenCoins)
        {
            UpdateCoins();
            prevDistance = distanceBetweenCoins;
        }
        if (prevRotation != rotation)
        {
            UpdateCoins();
            prevRotation = rotation;
        }
        if (thisTransform.hasChanged)
        {
            thisTransform.rotation = Quaternion.identity;
        }
    }

    protected virtual void UpdateCoins()
    {
        for (int i = 0; i < trajectoryPoints.Count; i++)
        {
            DestroyImmediate(trajectoryPoints[i]);
        }
        trajectoryPoints.Clear();

        for (int i = 0; i < coinsNum; i++)
        {
            thisTransform.rotation = Quaternion.Euler(0, rotation, 0);
            GameObject dot = (GameObject)Instantiate(coinPrefab,thisTransform.position + thisTransform.forward  * (i * distanceBetweenCoins), Quaternion.identity);
            trajectoryPoints.Add(dot);
            thisTransform.rotation = Quaternion.identity;
            dot.transform.SetParent(thisTransform, true);
        }
    }
}
