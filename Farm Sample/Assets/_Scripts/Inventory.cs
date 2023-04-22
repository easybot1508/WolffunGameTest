using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int money;

    public List<Product> productsHarvested = new List<Product>();

    public int totalNumberOfWorkersAvailable;

    public List<Product> seeds = new List<Product>();

    public static Inventory instance;

    void Awake()
    {
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
