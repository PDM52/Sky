using UnityEngine;

public class Paralax : MonoBehaviour
{
    public GameObject camera;
    public float parallaxEffect;
    private float length, haight;
    private Vector2 startpos;

    private void Start()
    {
        camera = GameObject.Find("Main Camera");
        startpos = transform.position;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        haight = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void Update()
    {
        var temp = new Vector2(camera.transform.position.x * (1 - parallaxEffect),
            camera.transform.position.y * (1 - parallaxEffect));
        var dist = new Vector2(camera.transform.position.x * parallaxEffect,
            camera.transform.position.y * parallaxEffect);
        transform.position = startpos + dist;
        if (temp.x > startpos.x + 1.5 * length) startpos.x += 3 * length;
        else if (temp.x < startpos.x - 1.5 * length) startpos.x -= 3 * length;
        if (temp.y > startpos.y + 1.5 * haight) startpos.y += 3 * haight;
        else if (temp.y < startpos.y - 1.5 * haight) startpos.y -= 3 * haight;
    }
}