using UnityEngine;

[CreateAssetMenu(fileName = "Resources", menuName = "Resources")]
public class Resources : ScriptableObject
{
    public int price, weight, quantity;
    public string name;
    public Sprite pic;
}