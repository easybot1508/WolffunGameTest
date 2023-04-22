using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
public class Fied : MonoBehaviour
{
    public static event UnityAction<Fied> onAddFied;

    int timer;

    public TextMeshPro amountUnHarvestText;
    public TextMeshPro nameCropText;
    public TextMeshPro timerText;

    public static int count;
    private void Start()
    {
        // mỗi 1s sẽ check ô đát có đang được dùng hay không
        StartCoroutine("CheckFiedIsUsed");
        IsUsed = false;

        onAddFied?.Invoke(gameObject.GetComponent<Fied>());
        Fied.count++;

        FarmWorker.setFiedIsUsed += SetFiedIsUsed;
        Crop.setFiedIsUsed += SetFiedIsUsed;
    }

    [SerializeField]
    private bool isUsed;
    public bool IsUsed
    {
        get { return isUsed; }   
        set { isUsed = value; }  
    }

    public void SetFiedIsUsed()
    {
        //print("f");
        if (this.gameObject.transform.childCount <= 1) IsUsed = false;
        else IsUsed = true;
    }

    // kiểm tra ô đất có đang sử dụng hay không
    IEnumerator CheckFiedIsUsed()
    {
        while (true)
        {
            if (this.gameObject.transform.childCount <= 1) IsUsed = false;
            else IsUsed = true;
            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator DeCounter()
    {
        while (true)
        {
            timer--;
            yield return new WaitForSeconds(1f);
        }
    }

    // khi đưa chuột vào ô đát sẽ hiện các thông tin trong ô
    void OnMouseOver()
    {
        InforTableLand();
    }

    void OnMouseExit()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    // xử lý các thông tin trong ô đất
    void InforTableLand()
    {
        if (transform.childCount > 1)
        {
            // nếu con của ô đất là 1 sản phẩm thì ta sẽ lấy thông tin về sản phẩm đó để xử lý thời gian
            if (transform.GetChild(1).CompareTag("Farmer")) return;
            Crop crop = transform.GetChild(1).GetComponent<Crop>();

            /*if (crop.curCrop.cropID == CropID.tomato) timer = GameManager.instance.curTimeToGrowTomato - crop.Timer;
            else if (crop.curCrop.cropID == CropID.blueBerry) timer = GameManager.instance.curTimeToGrowBlueberry - crop.Timer;
            else if (crop.curCrop.cropID == CropID.strawberry) timer = GameManager.instance.curTimeToGrowStrawberry - crop.Timer;
            else if (crop.curCrop.cropID == CropID.dairyCow) timer = GameManager.instance.curTimeToGrowDairyCow - crop.Timer;*/
            timer = crop.curTimeToGrown - crop.Timer;

            StartCoroutine("DeCounter");

            

            timerText.text = timer.ToString();
            nameCropText.text = crop.curCrop.cropName;
            amountUnHarvestText.text = GameManager.instance.amountUnharvestedcrops[crop.indexFied].ToString();
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            StopCoroutine("DeCounter");
            transform.GetChild(0).gameObject.SetActive(true);
            nameCropText.text = "Emty";
            amountUnHarvestText.text = "0";
            timerText.text = "0";
        }
    }
}
