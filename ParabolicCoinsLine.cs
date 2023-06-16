using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ParabolicCoinsLine : MonoBehaviour {

    public bool update = true;

    public GameObject coinPrefab;

    [Range(2, 30)]
    public int coinsNum = 10;
    public float height = 20f;
    public float length = 20f;

    protected float prevHeight;
    protected float prevLength;
    protected int prevCoinsNum;
    protected List<GameObject> trajectoryPoints = new List<GameObject>();    
    protected Vector3 b; //Vector position for end
    protected Transform thisTransform;
    protected Manager manager;

    protected virtual void Start()
    {
        if (Application.isPlaying)
        {
            b = thisTransform.position + ((Vector3.forward * length) * (manager.player.speed / manager.startPlayerSpeed));
            if (thisTransform.childCount > 0)
            {
                int i = 0;
                foreach (Transform child in thisTransform)
                {
                    child.position = SampleParabola(thisTransform.position, b, height, ((float)i / (float)(coinsNum - 1)));
                    i++;
                }
            }
        }
    }

    protected virtual void Awake()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
        thisTransform = transform;

        if (Application.isPlaying)
            return;        
        else
            b = thisTransform.position + Vector3.forward * length;
                
        if (thisTransform.childCount > 0)
        {
            while (thisTransform.childCount != 0)
            {
                DestroyImmediate(thisTransform.GetChild(0).gameObject);
            }
        }
        UpdateCoins();
    }

    protected virtual void UpdateCoins()
    {
        if (!update)
            return;       

        for (int i = 0; i < trajectoryPoints.Count; i++)
        {
            DestroyImmediate(trajectoryPoints[i]);
        }
        trajectoryPoints.Clear();

        for (int i = 0; i < coinsNum; i++)
        {
            GameObject dot = (GameObject)Instantiate(coinPrefab, SampleParabola(thisTransform.position, b, height, ((float)i / (float)(coinsNum - 1))), Quaternion.identity);
            trajectoryPoints.Add(dot);
            dot.transform.parent = thisTransform;
        }
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
        if (prevHeight != height)
        {
            UpdateCoins();
            prevHeight = height;
        }
        if (prevLength != length)
        {
            UpdateCoins();
            prevLength = length;
        }       
    }

    protected virtual Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t)
    {
        float parabolicT = t * 2 - 1;
        if (Mathf.Abs(start.y - end.y) < 0.1f)
        {
            //start and end are roughly level, pretend they are - simpler solution with less steps
            Vector3 travelDirection = end - start;
            Vector3 result = start + t * travelDirection;
            result.y += (-parabolicT * parabolicT + 1) * height;
            return result;
        }
        else
        {
            //start and end are not level, gets more complicated
            Vector3 travelDirection = end - start;
            Vector3 levelDirecteion = end - new Vector3(start.x, end.y, start.z);
            Vector3 right = Vector3.Cross(travelDirection, levelDirecteion);
            Vector3 up = Vector3.Cross(right, travelDirection);
            if (end.y > start.y) up = -up;
            Vector3 result = start + t * travelDirection;
            result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
            return result;
        }
    }
}
