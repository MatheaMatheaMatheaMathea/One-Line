using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineGenTest : MonoBehaviour
{
    public Transform a, b, c;
    public GameObject linePrefab;

    GameObject current;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            if (current != null) Destroy(current);
            current = Instantiate(linePrefab, transform.position, Quaternion.identity);
            current.GetComponent<Line>().ConstructFromPoints(a.position, b.position, c.position, LineType.Normal);
        }
    }
}
