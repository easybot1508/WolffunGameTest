using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    const int PRICE_LAND = 500;

    public TextMeshProUGUI moneyText; 

    public GameObject seedTable; 

    public GameObject shopTable;
    public GameObject wingameTable;

    // text số lượng các hạt giống
    public TextMeshProUGUI amountSeedTomatoTxt; 
    public TextMeshProUGUI amountSeedBlueberryTxt;
    public TextMeshProUGUI amountSeedStrawberryTxt;
    public TextMeshProUGUI amountSeedCowTxt;

    // text số lượng công nhân
    public TextMeshProUGUI amountFarmerWork;
    public TextMeshProUGUI amountFarmerIdle;

    // text giá các hạt giống
    public TextMeshProUGUI priceTomatoSeedTxt;
    public TextMeshProUGUI priceBlueberrySeedTxt;
    public TextMeshProUGUI priceStrawberrySeedTxt;
    public TextMeshProUGUI pricedDairyCowSeedTxt;
    public TextMeshProUGUI pricedLandTxt;

    // text số sản phẩm đã thu hoạch
    public TextMeshProUGUI amountTomatoHarvested;
    public TextMeshProUGUI amountBlueberryHarvested;
    public TextMeshProUGUI amountStrawberryHarvested;
    public TextMeshProUGUI amountMilkHarvested;

    // text cấp độ
    public TextMeshProUGUI levelTxt;

    // kiểm tra bật tắt các bảng
    bool isSeedTable;
    bool isOpenShop;

    private void Start()
    {
        // Đăng ký các sự kiện để cập nhật UI
        GameManager.instance.onSell += this.OnSellProduct;
        GameManager.instance.callWorker += this.CallWorker;
        FarmWorker.callWorker += this.CallWorker;
        GameManager.instance.openShop += this.SetPriceTxtProductInShop;
        GameManager.instance.win += this.OnWinGame;
        FarmWorker.setAmountProductHarvested += SetProductHarvested;
        GameManager.instance.setAmountProductHarvested += SetProductHarvested;
        GameManager.instance.onUpgrade += OnUpgrade;
    }

    // cập nhật level
    void OnUpgrade()
    {
        levelTxt.text = GameManager.instance.curLevel.ToString();
    }

    // cập nhật số sản phẩm thu hoạch được
    void SetProductHarvested()
    {
        foreach (Product product in Inventory.instance.productsHarvested)
        {
            CropID productID = product.productID;
            switch (productID)
            {
                case CropID.tomato:
                    amountTomatoHarvested.text = product.productCount.ToString();
                    break;
                case CropID.blueBerry:
                    amountBlueberryHarvested.text = product.productCount.ToString();
                    break;
                case CropID.strawberry:
                    amountStrawberryHarvested.text = product.productCount.ToString();
                    break;
                case CropID.dairyCow:
                    amountMilkHarvested.text = product.productCount.ToString();
                    break;
                default:

                    break;
            }
        }
    }

    // cập nhật giá các hạt giống trong shop
    void SetPriceTxtProductInShop()
    {
        isOpenShop = !isOpenShop;
        pricedLandTxt.text = PRICE_LAND.ToString();
        foreach (CropData cropData in GameManager.instance.crops)
        {
            CropID cropID = cropData.cropID;
            switch (cropID)
            {
                case CropID.tomato:
                    priceTomatoSeedTxt.text = cropData.purchasePrice.ToString();
                    break;
                case CropID.blueBerry:
                    priceBlueberrySeedTxt.text = cropData.purchasePrice.ToString();
                    break;
                case CropID.strawberry:
                    priceStrawberrySeedTxt.text = cropData.purchasePrice.ToString();
                    break;
                case CropID.dairyCow:
                    pricedDairyCowSeedTxt.text = cropData.purchasePrice.ToString();
                    break;
                default:
                    
                    break;
            }
        }
        shopTable.SetActive(isOpenShop);
    }

    // cập nhật trạng thái các công nhân
    void CallWorker()
    {
        amountFarmerWork.text = GameManager.instance.amountFarmerWorking.ToString();
        amountFarmerIdle.text = (Inventory.instance.totalNumberOfWorkersAvailable - GameManager.instance.amountFarmerWorking).ToString();
    }



    // cập nhật số tiền hiện tại
    void OnSellProduct()
    {
        GameManager.instance.curMoney = Inventory.instance.money;
        moneyText.text = Inventory.instance.money.ToString();
    }

    // mở bảng hạt giống
    public void OnSeedBnt()
    {
        isSeedTable = !isSeedTable;
        foreach(Product seed in Inventory.instance.seeds)
        {
            if (seed.productID == CropID.tomato) amountSeedTomatoTxt.text = seed.productCount.ToString();
            else if (seed.productID == CropID.blueBerry) amountSeedBlueberryTxt.text = seed.productCount.ToString();
            else if (seed.productID == CropID.strawberry) amountSeedStrawberryTxt.text = seed.productCount.ToString();
            else if (seed.productID == CropID.dairyCow) amountSeedCowTxt.text = seed.productCount.ToString();
        }
        seedTable.SetActive(isSeedTable);
    }

    // cập nhật trạng thái win game
    void OnWinGame()
    {
        wingameTable.SetActive(true);
    }
}

