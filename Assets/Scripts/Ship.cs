using System.Collections.Generic;
using System.Linq;
using Interfaces;
using UnityEngine;

public class Ship : MonoBehaviour, IClicable
{
    // Podstawowe zmienne
    public GameObject mainBase;
    public UpperBar upperBar;

    // Ekwipunek i przedmoity
    public List<Resources> inventory;
    public Ammunition bulletSlot;
    private Animator anim;

    private readonly bool bussy = false;
    
    private List<Destination> destinations;
    private int energy = 1000;
    private int health = 50;
    private RaycastHit hit;
    
    private int costs = 0;
    private int income = 0;

    // zmienne pomocnicze
    private Destination currentDestination;
    private readonly int max_energy = 1000;
    private readonly int max_health = 100;
    private readonly int max_weight = 250;
    private readonly int morales = 100;
    private float totalDistance = 0;
    private float timer = 0;
    private float delay = 1;

    // Wlasciwosci
    private readonly int speed = 5;
    private readonly int weight = 0;

    private void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
        destinations = new List<Destination>();
        destinations.Add(new Destination(mainBase, new []{true, false, false, true}));
        destinations.ElementAt(0).repeat=true;
        upperBar.Ships.Add(this);
    }

    private void Update()
    {
        timer += Time.deltaTime;
        
        if (currentDestination!=null)
        {
            if (currentDestination.position.x > transform.position.x) anim.SetInteger("state", 0);
            else anim.SetInteger("state", 2);

            if (anim.GetCurrentAnimatorStateInfo(0).IsName("Boat_State_0") ||
                anim.GetCurrentAnimatorStateInfo(0).IsName("Boat_State_2"))
            {
                transform.position = Vector3.MoveTowards(transform.position, currentDestination.position,
                    speed * Time.deltaTime * (energy>0?1:0.5f));
                if (timer > delay)
                {
                    energy--;
                    timer = 0;
                }
            }


            if ((Vector2)transform.position == currentDestination.position)
            {
                int profit = 0;
                if (currentDestination.repair) health = max_health;
                if (currentDestination.charge)
                    energy += currentDestination.target.GetComponent<IChargable>().GetEnergy(max_energy-energy);
                
                int oldIndex = destinations.IndexOf(currentDestination);
                currentDestination =
                    destinations.ElementAt((destinations.IndexOf(currentDestination) + 1) % destinations.Count());
                if (destinations.Count() == 1)
                    currentDestination = null;
                else
                {
                    foreach (Resources r in destinations.ElementAt(oldIndex).toBuy)
                    {
                        if (inventory.Contains(r))
                            inventory.ElementAt(inventory.IndexOf(r)).quantity += r.quantity;
                        else 
                            inventory.Add(r);
                        profit -= r.quantity * r.price;
                    }
                    foreach (Resources r in destinations.ElementAt(oldIndex).toSell)
                        if (inventory.Contains(r))
                        {
                            int deficite = r.quantity>inventory.ElementAt(inventory.IndexOf(r)).quantity?
                                inventory.ElementAt(inventory.IndexOf(r)).quantity:r.quantity;
                            inventory.ElementAt(inventory.IndexOf(r)).quantity -= deficite;
                            profit += deficite * r.price;
                            if (inventory.ElementAt(inventory.IndexOf(r)).quantity == 0)
                                inventory.Remove(r);
                        }

                    upperBar.budget += profit;
                    if (!destinations.ElementAt(oldIndex).repeat)
                        RemoveDestination(oldIndex);
                }
            }
        }
        
        if (Physics.Raycast(transform.position, Vector3.forward, out hit, 1f))
            if (hit.transform.gameObject.tag == "Battery")
            {
                energy += hit.transform.gameObject.GetComponent<Battery>().energy;
                hit.transform.gameObject.GetComponent<Battery>().energy = 0;
            }
    }

    public string GetInfo()
    {
        return "------------------------------------------------------"
               + "HP: " + health + "/" + max_health + " pts\n"
               + "Energy: " + energy + "/" + max_energy + "\n"
               + "Weight: " + weight + "/" + max_weight + "\n"
               + "Speed: " + speed + " kt\n"
               + "Damege: " + (bulletSlot != null ? bulletSlot.baseDamage : 0) + " pts/sec\n"
               + "Morales: " + (morales > 66 ? "Good" : morales > 33 ? "Medium" : "Bad") + "\n"
               + "------------------------------------------------------"
               + "Total Profit: " + income + " G\n"
               + "Total Costs: " + costs + " G\n"
               + "Balance: " + (income - costs) + " G\n"
               + "------------------------------------------------------"
               + "Action:  " + (bussy ? "Trading" : "Floating");
    }

    public void AddDestination(Vector2 pos)
    {
        if (destinations.Count < 4)
        {
            destinations.Add(new Destination(pos));
            if (destinations.Count == 2)
                currentDestination = new Destination(pos);
            UpdateDistance();
        }
    }

    public void AddDestination(Destination newDestination)
    {
        if (destinations.Count < 4)
            destinations.Add(newDestination);
        if (destinations.Count == 2)
            currentDestination = newDestination;
        income += newDestination.income;
        costs += newDestination.costs;
        UpdateDistance();

    }

    private void RemoveDestination(int i)
    {
        income -= destinations.ElementAt(i).income;
        costs -= destinations.ElementAt(i).costs;
        destinations.RemoveAt(i);
        UpdateDistance();
    }

    private void UpdateDistance()
    {
        totalDistance = 0;
        for (int i = 1; i < destinations.Count; i++)
            totalDistance += Vector2.Distance(destinations.ElementAt(i - 1).position, destinations.ElementAt(i).position);
    }
    
    public float GetBalance()
    {
        return (income-costs) / (totalDistance+0.01f)*speed;
    }
}

public class Destination
{
    public GameObject target { get; set; }
    public List<Resources> toBuy, toSell;
    public Vector2 position { get; }
    public int income { get; set; }
    public int costs { get; set; }
    public bool repeat { get; set; }
    public bool repair { get; set; }
    public bool charge { get; set; }
    public bool auto { get; set; }
    public Destination(Vector2 pos)
    {
        position = pos;
        income = 0;
        costs = 0;
    }

    public Destination(GameObject t, bool [] options)
    {
        toBuy = new List<Resources>();
        toSell = new List<Resources>();
        target = t;
        position = target.transform.position;
        income = 0;
        costs = 0;
        repair = options[0];
        charge = options[1];
        auto = options[2];
        repeat = options[3];
    }

    public void AddPurchase(Resources toAdd)
    {
        toBuy.Add(toAdd);
        costs += toAdd.price * toAdd.quantity;
    }

    public void AddSell(Resources toAdd)
    {
        toSell.Add(toAdd);
        income += toAdd.price * toAdd.quantity;
    }

    public override bool Equals(object obj)
    {
        return this.position == ((Destination)obj).position;
    }
}