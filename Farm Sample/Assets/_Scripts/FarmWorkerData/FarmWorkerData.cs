using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Farm Worker Data", menuName = "New Farm Worker Data")]
public class FarmWorkerData : ScriptableObject
{
    public string workerName;
    public int timesToWork;
    public int purchasePrice; // once purchase 10 product
    public GameObject workerPrefab;
}
