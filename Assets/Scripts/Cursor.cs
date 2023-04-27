using Unity.Mathematics;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    public GameObject camera, inspector, background, leftPanel, rightPanel;

    private int edgeSensivity = 5;

    private RaycastHit2D hit;
    private readonly int scrollSpeed = 8;

    private GameObject target;

    private void Update()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetKey(KeyCode.D) || Input.mousePosition.x > Screen.width - edgeSensivity)
            camera.transform.position += new Vector3(1, 0, 0) * (scrollSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.A) || Input.mousePosition.x < edgeSensivity)
            camera.transform.position += new Vector3(-1, 0, 0) * (scrollSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.W) || Input.mousePosition.y > Screen.height - edgeSensivity)
            camera.transform.position += new Vector3(0, 1, 0) * (scrollSpeed * Time.deltaTime);
        if (Input.GetKey(KeyCode.S) || Input.mousePosition.y < edgeSensivity)
            camera.transform.position += new Vector3(0, -1, 0) * (scrollSpeed * Time.deltaTime);

        if (Input.GetAxis("Mouse ScrollWheel") < 0f && camera.GetComponent<Camera>().orthographicSize < 8)
            camera.GetComponent<Camera>().orthographicSize++;
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && camera.GetComponent<Camera>().orthographicSize > 3)
            camera.GetComponent<Camera>().orthographicSize--;

        if (Input.GetKeyDown(KeyCode.Mouse1) && target != null)
        {
            if (target.tag == "Ship" && (hit = Physics2D.Raycast(transform.position, Vector2.zero)))
            {
                if(hit.collider.gameObject.GetComponent<Island>())
                    leftPanel.GetComponent<LeftPanel>().CreateDestination(hit.collider.gameObject, target);
                if(hit.collider.gameObject.GetComponent<Battery>())
                    target.GetComponent<Ship>().AddDestination(new Destination(hit.collider.gameObject, new bool[4] {false, true, false, true}));
            }
            else
                target.GetComponent<Ship>().AddDestination(transform.position);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (hit = Physics2D.Raycast(transform.position, Vector2.zero))
                AssignTarget(hit.collider.gameObject);
            else AssignTarget();
        }
    }

    private void AssignTarget(GameObject assigned = null)
    {
        target = assigned;
        if (target != null)
        {
            if(target.GetComponent<Ship>() || target.GetComponent<Battery>())
                inspector.GetComponent<Inspector>().Assign(target);
            else 
                inspector.GetComponent<Inspector>().Unassign();
            rightPanel.GetComponent<RightPanel>().SetTarget(target);
        }
        else
        {
            rightPanel.GetComponent<RightPanel>().RemoveTarget();
            inspector.GetComponent<Inspector>().Unassign();
        }
    }
}