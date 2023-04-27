using System.Collections;
using System.Collections.Generic;
using Interfaces;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RightPanel : MonoBehaviour
{
    private GameObject target;
    public GameObject inventoryPanel;
    public TextMeshProUGUI text;
    private void Update()
    {
        if (target != null)
            RefreshInspectionPanel();
    }
    public void RefreshInspectionPanel()
    {
        text.SetText(target.GetComponent<IClicable>().GetInfo());
        for (int i = 0; i < 15; i++)
        {
            GameObject slot = inventoryPanel.transform.Find("Slot " + i).gameObject;
            if (i < target.GetComponent<Ship>().inventory.Count) // do zmiany na IClicable
            {
                slot.SetActive(true);
                slot.transform.Find("Icon").gameObject.GetComponent<Image>().sprite =
                    target.GetComponent<Ship>().inventory[i].pic;
                slot.transform.Find("Quantity").gameObject.GetComponent<TextMeshProUGUI>()
                    .SetText((target.GetComponent<Ship>().inventory[i].quantity).ToString());
            }
            else
                slot.SetActive(false);
        }
    }

    public void SetTarget(GameObject obj)
    {
        target = obj;
        gameObject.SetActive(true);
        transform.Find("ObjectName").gameObject.GetComponent<TextMeshProUGUI>().SetText(target.name);
    }
    public void RemoveTarget()
    {
        target = null;
        gameObject.SetActive(false);
    }
}
