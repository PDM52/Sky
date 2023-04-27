using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanel : MonoBehaviour
{
    public Button confirmButton, closeButton;
    public Toggle[] checkmarks;
    private List<TransactionRow> rows;
    private GameObject destined, agent;
    private bool repair, charge, repeat, auto;
    public TextMeshProUGUI total;
    public GameObject page;
    private void Start()
    {
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        closeButton.onClick.AddListener(OnCloseButtonClick);
    }

    private void Update()
    {
        Refresh();
    }
    public void CreateDestination(GameObject island, GameObject ship)
    {
        agent = ship;
        destined = island;
        gameObject.SetActive(true);
        rows = new List<TransactionRow>();
        checkmarks[0].onValueChanged.AddListener(b => {repair = b; });
        checkmarks[1].onValueChanged.AddListener(b => {charge = b; });
        checkmarks[2].onValueChanged.AddListener(b => {auto = b; });
        checkmarks[3].onValueChanged.AddListener(b => {repeat = b; });
        transform.Find("Target Name").gameObject.GetComponent<TextMeshProUGUI>().SetText(island.name);
        
        for (int i = 0; i < 7; i++)
            rows.Add(new TransactionRow(page.transform.Find("Row " + i).gameObject, island.GetComponent<Island>(), i));
    }
    
    private void OnConfirmButtonClick()
    {
        gameObject.SetActive(false);
        Destination destination = new Destination(destined, new []{repair, charge, auto, repeat});
        foreach (TransactionRow row in rows)
        {
            if(row.state==TransactionRow.State.BUY)
                destination.AddPurchase(row.resources);
            if(row.state==TransactionRow.State.SELL)
                destination.AddSell(row.resources);
        }

        destination.repeat = repeat;
        destination.charge = charge;
        destination.repair = repair;
        agent.GetComponent<Ship>().AddDestination(destination);
    }

    private void OnCloseButtonClick()
    {
        gameObject.SetActive(false);
    }
    public void Refresh()
    {
        int totalValue=0;
        foreach (TransactionRow row in rows)
        {
            totalValue += int.Parse(row.buyTotal.GetParsedText().Replace("G", ""));
            totalValue -= int.Parse(row.sellTotal.GetParsedText().Replace("G", ""));
        }
        total.SetText(totalValue + "G");
    }
}

public class TransactionRow
{
    public TMP_InputField buyQuantity, sellQuantity;

    public TextMeshProUGUI buyTotal, sellTotal;
    private int buyPrice, sellPrice;
    private int mod;

    public Resources resources;

    private LeftPanel panel;
    public enum State
    {
        SELL,
        BUY,
        NONE
    }

    public State state = State.NONE;
    public TransactionRow(GameObject slot, Island target, int i)
    {
        panel = slot.transform.parent.parent.parent.GetComponent<LeftPanel>();
        resources = target.Market.ElementAt(i).Key;
        slot.transform.Find("Icon").gameObject.GetComponent<Image>().sprite = resources.pic;
        slot.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>()
            .SetText(resources.name);

        mod = target.Market.ElementAt(i).Value;
        slot.transform.Find("General").GetComponent<TextMeshProUGUI>().SetText("G G G".Substring(0, (mod-1)*2+1));
        buyPrice = (int)(resources.price * 1.1 * (1+(mod-3)*0.3));
        sellPrice = (int)(resources.price * 0.9 * (1+(mod-3)*0.3));
        slot.transform.Find("Price Buy").gameObject.GetComponent<TextMeshProUGUI>().SetText("Buy("+buyPrice+"G)");
        slot.transform.Find("Price Sell").gameObject.GetComponent<TextMeshProUGUI>().SetText("Sell("+sellPrice+"G)");
        
        buyQuantity = slot.transform.Find("Quantity Buy").gameObject.GetComponent<TMP_InputField>();
        
        sellQuantity = slot.transform.Find("Quantity Sell").gameObject.GetComponent<TMP_InputField>();
        buyQuantity.SetTextWithoutNotify("0");
        sellQuantity.SetTextWithoutNotify("0");
        buyQuantity.onValueChanged.AddListener(OnBuyQuantityValueChanged);
        sellQuantity.onValueChanged.AddListener(OnSellQuantityValueChanged);
        
        
        buyTotal=slot.transform.Find("Total Buy").gameObject.GetComponent<TextMeshProUGUI>();
        sellTotal=slot.transform.Find("Total Sell").gameObject.GetComponent<TextMeshProUGUI>();
        buyTotal.SetText("0G");
        sellTotal.SetText("0G");
    }
    
    void OnBuyQuantityValueChanged(string input)
    {
        int result;
        if (int.TryParse(input, out result) && result > 0)
        {
            buyTotal.SetText((result * buyPrice)+"G");
            sellTotal.SetText(0+"G");
            resources.quantity = result;
            state = State.BUY;
            resources.price = buyPrice;
            sellQuantity.text = "0";
        }
        else
        {
            buyTotal.SetText(0+"G");
            buyQuantity.text = "0";
            state = State.NONE;
        }
    }
    
    void OnSellQuantityValueChanged(string input)
    {
        int result;
        if (int.TryParse(input, out result) && result > 0)
        {
            sellTotal.SetText((result * sellPrice)+"G");
            buyTotal.SetText(0+"G");
            resources.quantity = result;
            state = State.SELL;
            resources.price = sellPrice;
            buyQuantity.text = "0";
        }
        else
        {
            sellTotal.SetText(0+"G");
            buyQuantity.text = "0";
            state = State.NONE;
        }
    }
}
