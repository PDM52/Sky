using Interfaces;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Battery : MonoBehaviour, IClicable, IChargable
{
    public int energy = 0;
    public int max_energy = 200;
    private float timer = 0;
    private float delay = 1;

    public string GetInfo()
    {
        return "------------------------------------------------------\n"+
               "Energy: " + energy + "/" + max_energy;
    }

    public int GetEnergy(int deficite)
    {
        if (deficite < energy)
        {
            energy -= deficite;
            return deficite;
        }
        else
        {
            int difference = deficite+(energy-deficite);
            energy -= difference;
            return difference;
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (energy < max_energy && timer > delay)
        {
            energy++;
            timer = 0;
        }
    }
}