using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Crop : MonoBehaviour
{
    public static string LAYER_NAME = "Crop";
    public static int BEFORE_CROP = 15;
    public static int AFTER_CROP = 14;

    public CropData curCrop;

    public bool onDead = false;

    [SerializeField]
    private int curTimeLife;

    public int indexFied;

    public SpriteRenderer sr;

    public delegate void SetFiedUse();
    public static event SetFiedUse setFiedIsUsed;

    [SerializeField]
    public int timer; // current time progress
    public float timeSeg;
    [SerializeField]
    public int cropProgIndex;

    [SerializeField]
    bool isOneAddCanHarvest = false;

    public int curTimeToGrown;

    public int Timer
    {
        get { return timer; }
    }

    private void Start()
    {
        FarmWorker.onHarvestCrop += Harvest;

        StartCoroutine ("Counter");
    }
    
    // đếm 1s để tính toán thời gian cây phát triển
    IEnumerator Counter()
    {
        while (true)
        {
            timer++;
            UpdateCropSprite();
            yield return new WaitForSeconds(1f);
        }
    }


    public void Plant(CropData crop)
    {
        curCrop = crop;
        SetTimeToGrow();
        /*if (curCrop.cropID == CropID.tomato) curTimeToGrown = GameManager.instance.curTimeToGrowTomato;
        else if (curCrop.cropID == CropID.blueBerry) curTimeToGrown = GameManager.instance.curTimeToGrowBlueberry;
        else if(curCrop.cropID == CropID.strawberry) curTimeToGrown = GameManager.instance.curTimeToGrowStrawberry;
        else if(curCrop.cropID == CropID.dairyCow) curTimeToGrown = GameManager.instance.curTimeToGrowDairyCow;*/

        UpdateCropSprite();
    }

    void SetTimeToGrow()
    {
        if (curCrop.cropID == CropID.tomato) curTimeToGrown = GameManager.instance.curTimeToGrowTomato;
        else if (curCrop.cropID == CropID.blueBerry) curTimeToGrown = GameManager.instance.curTimeToGrowBlueberry;
        else if (curCrop.cropID == CropID.strawberry) curTimeToGrown = GameManager.instance.curTimeToGrowStrawberry;
        else if (curCrop.cropID == CropID.dairyCow) curTimeToGrown = GameManager.instance.curTimeToGrowDairyCow;
    }

    void UpdateCropSprite()
    {
        sr.sprite = curCrop.readyToHarvestSprite;
        // kiểm tra xem cây có thu hoạch được chưa
        if (CanHarvest())
        {
            // nếu được
            // timer sẽ đếm lại từ đầu cho trái mới
            //SetTimeToGrow();
            timer = 0;
            // số lương sản phẩm có thể thu hoạch tăng lên 
            GameManager.instance.amountUnharvestedcrops[indexFied]++;
            TimeLifeManager.instance.counter[indexFied]++;

            if (!isOneAddCanHarvest)
            {
                GameManager.instance.cropsCanHarvest.Add(gameObject.GetComponent<Crop>());
                isOneAddCanHarvest = true;
            }

            // kiểm tra xem cây có đủ vòng đời chưa
            // nếu đủ thì cây sẽ bị huỷ cùng với số sản phẩm chưa thu hoạch
            if (TimeLifeManager.instance.counter[indexFied] >= curCrop.timesLife)
            {
                onDead = true;
                foreach (Crop crop in GameManager.instance.cropsCanHarvest)
                {
                    if (crop.onDead)
                    {
                        GameManager.instance.cropsCanHarvest.Remove(crop);
                        setFiedIsUsed?.Invoke();
                        break;
                    }
                }
                TimeLifeManager.instance.counter[indexFied] = 0;
                GameManager.instance.amountUnharvestedcrops[indexFied] = 0;
                Destroy(gameObject);
            }
        }
    }

    public void Harvest(Crop crop)
    {
        if (CanHarvest())
        {
            GameManager.instance.amountUnharvestedcrops[crop.indexFied]--;
        }
    }

    // Returns the number of days that the crop has been planted for.
    int CropProgress()
    {
        return timer;
    }

    // Can we currently harvest the crop?
    public bool CanHarvest()
    {
        return CropProgress() >= curTimeToGrown;
    }
}
