using UnityEngine;

public class Waving : MonoBehaviour
{
    private float shiftX;
    private float shiftY;

    private void Start()
    {
        shiftX = Random.Range(0, 10);
        shiftY = Random.Range(0, 10);
    }

    private void Update()
    {
        var xPos = Mathf.Sin(Time.time / 2.5f + shiftX) / 10;
        var yPos = Mathf.Sin(Time.time + shiftY) / 10;
        transform.position = transform.parent.transform.position + new Vector3(xPos, yPos, 0);
    }
}