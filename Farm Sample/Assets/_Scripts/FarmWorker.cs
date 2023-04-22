using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmWorker : MonoBehaviour
{
    public static int ORDER_LAYER_WORKER = 16;

    public bool isWorking = false;
    public int indexFied; // vị trí ô đất mà công nhân đứng

    public State st;

    [SerializeField]
    protected FarmWorkerData farmWorkerData;

    [SerializeField]
    int workingTimer = 0;

    CropData cropData;
    Crop crop;
    Fied objFied;
    bool check;

    public delegate void OnHarvestCrop(Crop crop);
    public static event OnHarvestCrop onHarvestCrop;

    public delegate void SetFiedUse();
    public static event SetFiedUse setFiedIsUsed;

    public delegate void CallWorker();
    public static event CallWorker callWorker;

    public delegate void SetAmountProductHarvested();
    public static event SetAmountProductHarvested setAmountProductHarvested;

    private void Awake()
    {
        farmWorkerData = GameManager.instance.farmWorkerData;
    }

    private void Start()
    {
        StartCoroutine("Counter");
    }

    // đếm thời gian mỗi 1s để tính toán thời gian phát triển
    IEnumerator Counter()
    {
        while (true)
        {
            workingTimer++;
            if(st == State.plant) WorkerPlant(this.cropData, this.objFied, this.indexFied);
            else WorkerHarvest(this.crop);
            yield return new WaitForSeconds(1f);
        }
    }

    public bool IsWorking()
    {
        return isWorking;
    }

    // được gọi khi muốn trồng cây
    public void WorkerPlant(CropData cropData, Fied objFied, int indexFied)
    {
        this.cropData = cropData;
        this.objFied = objFied;
        this.indexFied = indexFied;
        // kiểm tra thời gian mà công nhân làm việc đã đủ chưa
        if(workingTimer >= farmWorkerData.timesToWork)
        {
            // nếu đủ thì sẽ trồng cây
            GameManager.instance.amountFarmerWorking--;
            GameManager.instance.selectedCropToPlant = cropData;
            GameManager.instance.PlantNewCrop(GameManager.instance.selectedCropToPlant, objFied.transform, indexFied);
            
            // counter để quản lý vòng đời của cây ngay tại ô đất vừa trồng
            TimeLifeManager.instance.counter.Add(0);
            StopCoroutine("Counter");
            callWorker?.Invoke();
            // huỷ bỏ công nhân sau khi thực hiện xong nhiệm vụ
            Destroy(gameObject);
            setFiedIsUsed?.Invoke();
        }
        else
        {
            isWorking = true;
        }
    }

    // được gọi khi muốn thu hoạch
    public void WorkerHarvest(Crop crop)
    {
        this.crop = crop;
        if (workingTimer >= farmWorkerData.timesToWork)
        {
            // cập nhật số lượng công nhân rãnh khi làm xong
            GameManager.instance.amountFarmerWorking--;
            // dò xem sản phẩm vừa thu hoạch đã có trong túi chưa
            foreach (Product product in Inventory.instance.productsHarvested)
            {
                if(product.productID == crop.curCrop.cropID)
                {
                    // nếu có rồi thì tăng số lương sản phẩm đó lên 1
                    product.productCount++;
                    setAmountProductHarvested?.Invoke();
                    check = true;
                }
            }
            // nếu không sẽ add thêm sảm phẩm mới vào danh sách
            if (!check)
            {
                Product newProduct = new Product();
                newProduct.productID = crop.curCrop.cropID;
                newProduct.productCount++;
                Inventory.instance.productsHarvested.Add(newProduct);
                setAmountProductHarvested?.Invoke();
            }
            GameManager.instance.amountUnharvestedcrops[crop.indexFied]--;

            StopCoroutine("Counter");
            callWorker?.Invoke();
            Destroy(gameObject);
            setFiedIsUsed?.Invoke();
        }
        else
        {
            isWorking = true;
        }
    }
}

public enum State
{
    plant,
    harvest
};
