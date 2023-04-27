using UnityEngine;

public class Inspector : MonoBehaviour
{
    public GameObject inspector;

    public void Assign(GameObject target)
    {
        transform.SetParent(target.transform, false);
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<Renderer>().enabled = true;
    }

    public void Unassign()
    {
        transform.SetParent(null, false);
        for (var i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<Renderer>().enabled = false;
    }
}