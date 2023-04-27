using System.Diagnostics;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpperBar : MonoBehaviour
{
    public List<Ship> Ships = new();
    public int budget { get; set; }
    private float tefreshTime = 60;
    private TextMeshProUGUI budgetText;
    private TextMeshProUGUI balanceText;
    public Button exit, help;

    void Start()
    {
        budget = 1000;
        budgetText = transform.Find("Budget").gameObject.GetComponent<TextMeshProUGUI>();
        balanceText = transform.Find("Balance").gameObject.GetComponent<TextMeshProUGUI>();
        exit.gameObject.GetComponent<Button>().onClick.AddListener(Exit);
        help.gameObject.GetComponent<Button>().onClick.AddListener(Help);
    }
    void Update()
    {
        if (Time.frameCount% tefreshTime==0)
        {
            float balance = 0;
            foreach (Ship ship in Ships)
                balance += ship.GetBalance();
            budgetText.SetText(budget+"G");
            balanceText.SetText(balance.ToString("F2")+"G");
        }
        
    }
    
    private void Exit()
    {
        Application.Quit();
    }
    
    private void Help()
    {
        Process.Start("User_Manual.pdf");
    }
}
