using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

public class Island : MonoBehaviour, IClicable, IChargable
{
    public Resources[] highDemand, lowDemand;
    public Dictionary<Resources, int> Market { get; set; }

    public int GetEnergy(int deficite)
    {
        return deficite;
    }
    private void Start()
    {
        var allResources = GameObject.Find("AssetBase").GetComponent<AssetBase>().allResources;
        Market = new Dictionary<Resources, int>();
        foreach (var item in allResources)
            if (highDemand.Contains(item))
                Market.Add(item, 3);
            else if (lowDemand.Contains(item))
                Market.Add(item, 1);
            else Market.Add(item, 2);
    }

    public string GetInfo()
    {
        return "[Placeholeder]";
    }
}