using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Crop Data", menuName = "New Crop Data")]
public class CropData : ScriptableObject
{
    public string cropName;
    public CropID cropID;
    public int timesToGrow;
    public int timesLife; // die when harvest enough amout product
    public Sprite[] growProgressSprites;
    public Sprite readyToHarvestSprite;
    public int purchasePrice; // once purchase 10 product
    public int sellPrice;
    public GameObject cropPrefab;
    
}
