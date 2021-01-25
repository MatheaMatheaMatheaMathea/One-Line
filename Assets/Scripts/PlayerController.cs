using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float BumpForce;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.transform.parent != null && col.gameObject.transform.parent.GetComponent<Line>().LineType == LineType.Rubber)
        {
            GetComponent<Rigidbody2D>().AddForce(new Vector2(0, BumpForce));
        }
    }
}
