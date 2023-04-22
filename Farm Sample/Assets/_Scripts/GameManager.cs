using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    const int AMOUNT_BUY_SEED = 10;
    const int PRICE_LAND = 500;
    const int PRICE_UPGRADE = 500;
    const float PERCENT_UPGRADE = 0.1f;

    public int curTimeToGrowTomato;
    public int curTimeToGrowBlueberry;
    public int curTimeToGrowStrawberry;
    public int curTimeToGrowDairyCow;

    public int curMoney;

    public int curLevel;

    public int amountFarmerWorking;

    public CropData selectedCropToPlant;
    public Crop crop;
    public FarmWorkerData farmWorkerData;

    [SerializeField]
    public int[] amountUnharvestedcrops = new int[100];

    public List<CropData> crops = new List<CropData>();
    public List<Fied> fieds = new List<Fied>();

    public List<Crop> cropsCanHarvest = new List<Crop>();

    public delegate void OnHarvestCrop(Crop crop);
    public event OnHarvestCrop onHarvestCrop;

    public delegate void OnSellProduct();
    public event OnSellProduct onSell;

    public delegate void OpenShop();
    public event OpenShop openShop;

    public delegate void CallWorker();
    public event CallWorker callWorker;

    public delegate void Win();
    public event Win win;

    public delegate void SetFiedUse();
    public event SetFiedUse setFiedIsUsed;

    public delegate void SetAmountProductHarvested();
    public event SetAmountProductHarvested setAmountProductHarvested;

    public delegate void Upgrade();
    public event Upgrade onUpgrade;

    // các thông tin để tính toán vị trí đặt ô đất
    private const float originalPosX = -8f;
    float posX, posY, disBeetweenFied;
    public Vector2 size;
    int i = 0, j = 0;
    public GameObject fiedPrefab;
    Vector2 posFiedNext;

    public static GameManager instance;

    private void Start()
    {
        // cập nhật tiền khi khởi động game
        onSell?.Invoke();
        // set up vị trí ban đầu của ô đầu tiên
        posX = -8f;
        posY = 4f;
        disBeetweenFied = 1.5f;
        size = new Vector2(10, 5);
        posFiedNext = new Vector2(0f, 0f);

        // lưu lại các thời gian phát triển của các sản phẩm để tính sau này tính toán
        foreach(CropData cropData in crops)
        {
            if (cropData.cropID == CropID.tomato) curTimeToGrowTomato = cropData.timesToGrow;
            else if (cropData.cropID == CropID.blueBerry) curTimeToGrowBlueberry = cropData.timesToGrow;
            else if(cropData.cropID == CropID.strawberry) curTimeToGrowStrawberry = cropData.timesToGrow;
            else if(cropData.cropID == CropID.dairyCow) curTimeToGrowDairyCow = cropData.timesToGrow;
        }

        // tạo 3 ô đất đầu tiền
        for(int i=0; i<3; i++)
        {
            CreateFied();
        }
    }

    private void FixedUpdate()
    {
        if(Inventory.instance.money != curMoney)
        {
            onSell?.Invoke();
        }
    }

    void OnEnable()
    {
        Fied.onAddFied += AddFied;
    }

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

    // kiểm tra có thể trồng cây được chưa
    public bool CanPlantCrop(CropID productID)
    {
        // kiểm số công nhân đang rãnh
        if(Inventory.instance.totalNumberOfWorkersAvailable - amountFarmerWorking > 0)
        {
            // kiểm số hạt giống đang có
            foreach (Product seed in Inventory.instance.seeds)
            {
                if (seed.productID == productID)
                {
                    if (seed.productCount > 0)
                    {
                        seed.productCount--; // nếu còn hạt giống thì giảm số lượng lại
                        return true;
                    }
                }
            }
        }
        return false;
    }
    // Called when the buy crop button is pressed.

    // tạo ra một đối tượng sản phẩm vào trong ô đất 
    public Crop PlantNewCrop(CropData cropData, Transform posFied, int indexFied)
    {
        Crop curCrop = Instantiate(cropData.cropPrefab, posFied).GetComponent<Crop>() as Crop;
        curCrop.Plant(cropData);
        curCrop.indexFied = indexFied;
        curCrop.sr.sortingOrder = Crop.AFTER_CROP;

        curCrop.sr.sortingLayerName = Crop.LAYER_NAME;
        return curCrop;
    }
    
    // kiểm tra xem ô đất đó có đang trống không
    Fied IsEmptyFied(ref int indexFied)
    {
        foreach (Fied fied in fieds)
        {
            //print(fied.name + " " + fied.IsUsed);
            indexFied++;
            if (!fied.IsUsed)
            {
                fied.IsUsed = true;
                return fied;
            }
        }
        print("All Fieds are used");
        return null;
    }

    // thêm ô đất mới vào danh sách đất hiện tại
    void AddFied(Fied fied)
    {
        fieds.Add(fied);
    }

    // khi nhấn vào nút trồng cà chua thì hàm này được gọi
    public void OnPlantTomato()
    {
        setFiedIsUsed?.Invoke();
        if (CanPlantCrop(CropID.tomato))
        {
            int indexFied = -1;
            Fied objFied = IsEmptyFied(ref indexFied);
            if (objFied != null)
            {
                // dò để lấy thông tin cây mà ta muốn trồng
                foreach (CropData crop in crops)
                {
                    if (crop.cropID == CropID.tomato)
                    {
                        CallWorkerPlant(crop, objFied, indexFied);
                        break;
                    }
                }
            }
        }
    }

    public void OnPlantBlueberry()
    {
        if (CanPlantCrop(CropID.blueBerry))
        {
            int indexFied = -1;
            Fied objFied = IsEmptyFied(ref indexFied);
            if (objFied != null)
            {
                foreach (CropData crop in crops)
                {
                    if (crop.cropID == CropID.blueBerry)
                    {
                        CallWorkerPlant(crop, objFied, indexFied);
                        break;
                    }
                }
            }
        }
    }

    public void OnPlantStrawberry()
    {
        if (CanPlantCrop(CropID.strawberry))
        {
            int indexFied = -1;
            Fied objFied = IsEmptyFied(ref indexFied);
            if (objFied != null)
            {
                foreach (CropData crop in crops)
                {
                    if (crop.cropID == CropID.strawberry)
                    {
                        CallWorkerPlant(crop, objFied, indexFied);
                        break;
                    }
                }
            }
        }
    }

    public void OnRaiseCow()
    {
        if (CanPlantCrop(CropID.dairyCow))
        {
            int indexFied = -1;
            Fied objFied = IsEmptyFied(ref indexFied);
            if (objFied != null)
            {
                foreach (CropData crop in crops)
                {
                    if (crop.cropID == CropID.dairyCow)
                    {
                        CallWorkerPlant(crop, objFied, indexFied);
                        break;
                    }
                }
            }
        }
    }

    // tạo ra một công nhân trong ô đất để trồng cây
    public void CallWorkerPlant(CropData cropData, Fied objFied, int indexFied)
    {
        FarmWorker farmWorker = Instantiate(farmWorkerData.workerPrefab, objFied.transform).GetComponent<FarmWorker>() as FarmWorker;
        farmWorker.GetComponent<SpriteRenderer>().sortingOrder = FarmWorker.ORDER_LAYER_WORKER;
        farmWorker.GetComponent<SpriteRenderer>().sortingLayerName = Crop.LAYER_NAME;
        farmWorker.WorkerPlant(cropData, objFied, indexFied);
        farmWorker.st = State.plant;
        amountFarmerWorking++;
        callWorker?.Invoke();
    }

    // tạo ra một công nhân trong ô đất để thu hoạch
    public void CallWorkerHarvest(Crop crop)
    {
        if (Inventory.instance.totalNumberOfWorkersAvailable - amountFarmerWorking <= 0) return;
        FarmWorker farmWorker = Instantiate(farmWorkerData.workerPrefab, crop.transform).GetComponent<FarmWorker>() as FarmWorker;
        farmWorker.GetComponent<SpriteRenderer>().sortingOrder = FarmWorker.ORDER_LAYER_WORKER;
        farmWorker.GetComponent<SpriteRenderer>().sortingLayerName = Crop.LAYER_NAME;
        farmWorker.WorkerHarvest(crop);
        farmWorker.st = State.harvest;
        amountFarmerWorking++;
        callWorker?.Invoke();
    }

    // thu hoạch sản phẩm
    public void OnHarvestTomato()
    {
        foreach(Crop crop in cropsCanHarvest)
        {
            if(crop.curCrop.cropID == CropID.tomato && amountUnharvestedcrops[crop.indexFied] > 0 && crop.gameObject.transform.childCount <= 0)
            {
                CallWorkerHarvest(crop);
                break;
            }
        }
    }

    public void OnHarvestBlueberry()
    {
        foreach (Crop crop in cropsCanHarvest)
        {
            if (crop.curCrop.cropID == CropID.blueBerry && amountUnharvestedcrops[crop.indexFied] > 0 && crop.gameObject.transform.childCount <= 0)
            {
                CallWorkerHarvest(crop);
                break;
            }
        }
    }

    public void OnHarvestStrawberry()
    {
        foreach (Crop crop in cropsCanHarvest)
        {
            if (crop.curCrop.cropID == CropID.strawberry && amountUnharvestedcrops[crop.indexFied] > 0 && crop.gameObject.transform.childCount <= 0)
            {
                CallWorkerHarvest(crop);
                break;
            }
        }
    }

    public void OnMilk()
    {
        foreach (Crop crop in cropsCanHarvest)
        {
            if (crop.curCrop.cropID == CropID.dairyCow && amountUnharvestedcrops[crop.indexFied] > 0 && crop.gameObject.transform.childCount <= 0)
            {
                CallWorkerHarvest(crop);
                break;
            }
        }
    }

    // được gọi khi nhấn các nút bán
    public void OnSellTomato()
    {
        // kiểm tra trong túi có sản phẩm đã thu hoạch không
        foreach(Product product in Inventory.instance.productsHarvested)
        {
            if(product.productID == CropID.tomato)
            {
                if (product.productCount > 0)
                {
                    product.productCount--; // nếu có thì ta sẽ trừ lại số lượng
                    setAmountProductHarvested?.Invoke();
                    foreach (CropData cropData in crops)
                    {
                        if (cropData.cropID == CropID.tomato)
                        {
                            Inventory.instance.money += cropData.sellPrice; // cộng thêm tiền 
                            onSell?.Invoke();
                            // mỗi lần thêm tiền thì ta sẽ check đã đủ tiền win game chưa
                            OnWinGame();
                        }
                    }
                }
            }
        }
    }

    public void OnSellBlueberry()
    {
        foreach (Product product in Inventory.instance.productsHarvested)
        {
            if (product.productID == CropID.blueBerry)
            {
                if (product.productCount > 0)
                {
                    product.productCount--;
                    setAmountProductHarvested?.Invoke();
                    foreach (CropData cropData in crops)
                    {
                        if (cropData.cropID == CropID.blueBerry)
                        {
                            Inventory.instance.money += cropData.sellPrice;
                            onSell?.Invoke();
                            OnWinGame();
                        }
                    }
                }
            }
        }
    }

    public void OnSellStrawberry()
    {
        foreach (Product product in Inventory.instance.productsHarvested)
        {
            if (product.productID == CropID.strawberry)
            {
                if (product.productCount > 0)
                {
                    product.productCount--;
                    setAmountProductHarvested?.Invoke();
                    foreach (CropData cropData in crops)
                    {
                        if (cropData.cropID == CropID.strawberry)
                        {
                            Inventory.instance.money += cropData.sellPrice;
                            onSell?.Invoke();
                            OnWinGame();
                        }
                    }
                }
            }
        }
    }

    public void OnSellMilk()
    {
        foreach (Product product in Inventory.instance.productsHarvested)
        {
            if (product.productID == CropID.dairyCow)
            {
                if (product.productCount > 0)
                {
                    product.productCount--;
                    setAmountProductHarvested?.Invoke();
                    foreach (CropData cropData in crops)
                    {
                        if (cropData.cropID == CropID.dairyCow)
                        {
                            Inventory.instance.money += cropData.sellPrice;
                            onSell?.Invoke();
                            OnWinGame();
                        }
                    }
                }
            }
        }
    }

    // được gọi khi nhấn nút mua hạt giống trong shop
    public void OnBuyTomatoSeed()
    {
        foreach (CropData cropData in crops)
        {
            if (cropData.cropID == CropID.tomato)
            {
                // kiểm tra xem có đủ tiền không
                if (Inventory.instance.money >= cropData.purchasePrice)
                {
                    // nếu đủ thì trừ tương ứng
                    Inventory.instance.money -= cropData.purchasePrice;
                    onSell?.Invoke();
                    foreach (Product seed in Inventory.instance.seeds)
                    {
                        if (seed.productID == CropID.tomato)
                        {
                            // khi mua xong thì cộng vào số hạt giống vào túi
                            seed.productCount += AMOUNT_BUY_SEED;
                        }
                    }
                }
            }
        }
        
    }

    public void OnBuyBlueberrySeed()
    {
        foreach (CropData cropData in crops)
        {
            if (cropData.cropID == CropID.blueBerry)
            {
                if (Inventory.instance.money >= cropData.purchasePrice)
                {
                    Inventory.instance.money -= cropData.purchasePrice;
                    onSell?.Invoke();
                    foreach (Product seed in Inventory.instance.seeds)
                    {
                        if (seed.productID == CropID.blueBerry)
                        {
                            seed.productCount += AMOUNT_BUY_SEED;

                        }
                    }
                }
            }
        }
    }

    public void OnBuyStrawberrySeed()
    {
        foreach (CropData cropData in crops)
        {
            if (cropData.cropID == CropID.strawberry)
            {
                if (Inventory.instance.money >= cropData.purchasePrice)
                {
                    Inventory.instance.money -= cropData.purchasePrice;
                    onSell?.Invoke();
                    foreach (Product seed in Inventory.instance.seeds)
                    {
                        if (seed.productID == CropID.strawberry)
                        {
                            seed.productCount += AMOUNT_BUY_SEED;

                        }
                    }
                }
            }
        }
    }

    public void OnBuyDairyCowSeed()
    {
        foreach (CropData cropData in crops)
        {
            if (cropData.cropID == CropID.dairyCow)
            {
                if (Inventory.instance.money >= cropData.purchasePrice)
                {
                    Inventory.instance.money -= cropData.purchasePrice;
                    onSell?.Invoke();
                    foreach (Product seed in Inventory.instance.seeds)
                    {
                        if (seed.productID == CropID.dairyCow)
                        {
                            seed.productCount += 1;
                        }
                    }
                }
            }
        }
    }

    // gọi khi nhấn nút shop
    public void OnShoping()
    {
        openShop?.Invoke();
    }

    // gọi khi nhấn nút thuê công nhân
    public void OnHireWorker()
    {
        if(Inventory.instance.money >= farmWorkerData.purchasePrice)
        {
            Inventory.instance.money -= farmWorkerData.purchasePrice;
            onSell?.Invoke();
            // khi mua xong thì tổng số lượng công nhân đang sở hữu tăng lên
            Inventory.instance.totalNumberOfWorkersAvailable++;
            callWorker?.Invoke();
        }
    }

    // gọi khi nhấn mua ô đất
    public void OnPurchaseFied()
    {
        if(Inventory.instance.money >= PRICE_LAND)
        {
            Inventory.instance.money -= PRICE_LAND;
            onSell?.Invoke();
            CreateFied();
        }
    }

    // gọi khi nhấn nút upgrade
    public void OnUpgrade()
    {
        if(Inventory.instance.money >= PRICE_UPGRADE)
        {
            // số level tăng lên 1
            curLevel++;
            onUpgrade?.Invoke();
            // tiền giảm tương ứng
            Inventory.instance.money -= PRICE_UPGRADE;
            onSell?.Invoke();
            // set lại thời gian lớn của sản phẩm (tăng năng suất)
            curTimeToGrowTomato -= (int)(curTimeToGrowTomato * PERCENT_UPGRADE);
            curTimeToGrowBlueberry -= (int)(curTimeToGrowBlueberry * PERCENT_UPGRADE);
            curTimeToGrowStrawberry -= (int)(curTimeToGrowStrawberry * PERCENT_UPGRADE);
            curTimeToGrowDairyCow -= (int)(curTimeToGrowDairyCow * PERCENT_UPGRADE);
        }
    }

    // tạo ra một ô đất
    void CreateFied()
    {
        if (j >= size.y) return;
        posFiedNext = new Vector2(posX, posY);
        Instantiate(fiedPrefab, posFiedNext, Quaternion.identity);
        if (i < size.x)
        {
            posX += disBeetweenFied;
            i++;
        }
        else
        {
            posY -= disBeetweenFied;
            posX = originalPosX;
            j++;
            i = 0;
        }
    }

    // win game khi đủ 1000000 tiền
    void OnWinGame()
    {
        if(Inventory.instance.money >= 1000000)
        {
            win?.Invoke();
        }
    }
}
