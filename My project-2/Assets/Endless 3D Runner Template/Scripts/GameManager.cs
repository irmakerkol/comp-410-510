using UnityEngine;

public class GameManager : MonoBehaviour
{

    protected Manager manager;

    private void Start()
    {
        manager = GameObject.Find("Manager").GetComponent<Manager>();
    }

}

