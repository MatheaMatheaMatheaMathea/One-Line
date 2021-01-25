using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LineType
{
    Normal,
    Ice,
    Rubber
}
public class GameControl : MonoBehaviour
{
    LineType lineType;
    public GameObject GameCursor;
    public Sprite CursorNormal, CursorIce, CursorRubber;
    public GameObject LinePrefab;
    public float LifeTimeAfterNewLine;
    public float DrawRateSeconds;
    GameObject lastLine;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        GameCursor.transform.position = new Vector3(mousePos.x + 20, mousePos.y - 40, mousePos.z);

        if (!Input.GetMouseButton(0))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                lineType = LineType.Normal;
                GameCursor.GetComponent<Image>().sprite = CursorNormal;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                lineType = LineType.Ice;
                GameCursor.GetComponent<Image>().sprite = CursorIce;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                lineType = LineType.Rubber;
                GameCursor.GetComponent<Image>().sprite = CursorRubber;
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D rayhit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePos), Camera.main.transform.forward, 20f, 1 << 8);
            if (rayhit.collider != null) StartCoroutine(Dragging(rayhit.collider.gameObject));
            else
            {
                GameObject line = Instantiate(LinePrefab, Camera.main.ScreenToWorldPoint(mousePos), Quaternion.identity);
                if (lastLine != null) Destroy(lastLine, LifeTimeAfterNewLine);
                lastLine = line;
                line.GetComponent<Line>().ConstructFromCursor(DrawRateSeconds, lineType);
            }
        }
    }

    IEnumerator Dragging(GameObject ball)
    {
        Rigidbody2D rb = ball.GetComponent<Rigidbody2D>();
        float initG = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        while (Input.GetMouseButton(0))
        {
            Vector3 pos = ball.transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            ball.transform.position = pos;
            yield return new WaitForEndOfFrame();
        }
        rb.gravityScale = initG;
    }
}
