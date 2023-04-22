using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeLifeManager : MonoBehaviour
{
    [SerializeField]
    public List<int> counter = new List<int>();
    public static TimeLifeManager instance;

    public bool[] isRemoveIndexFied;
    void Awake()
    {
        isRemoveIndexFied = new bool[100];
        // Initialize the singleton.
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }
}
