using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    public PhysicsMaterial2D Bounce, Slide;
    public float NormalLength, IceLength, RubberLength;
    public LineType LineType;
    PhysicsMaterial2D material;
    float lengthLimit;
    Color color;
    Vector2 End;
    public Sprite Circle;
    public Sprite Box;
    public float Thickness;
    public LinePiece[] Pieces {get {return pieces.ToArray();}}
    List<LinePiece> pieces;
    List<LinePiece> stuckPieces;
    public int Size {get {return Pieces.Length;}}

    public float Length;
    public float MinLengthForNewPiece;

    public void ConstructFromPoints(Vector2 a, Vector2 b, Vector2 c, LineType lineType)
    {
        Setup(lineType);
        lengthLimit = 500;

        Vector2 ac = c + ((a - c) / 2);
        End = ac;
        Vector2 acDir = Rotate((c - a).normalized, Mathf.Deg2Rad * 90f);
        for(int i = 0; i < 50; ++i)
        {
            Add(End + acDir, false);
        }
        
        Vector2 bc = c + ((b - c) / 2);
        End = bc;
        Vector2 bcDir = Rotate((b - c).normalized, Mathf.Deg2Rad * 90f);
        for(int i = 0; i < 50; ++i)
        {
            Add(End + bcDir, false);
        }
    }

    Vector2 Rotate(Vector2 a, float radians)
    {
        float cs = Mathf.Cos(radians);
        float sn = Mathf.Sin(radians);
        return new Vector2((a.x * cs) - (a.y * sn), (a.x * sn) + (a.y * cs));
    }

    public void ConstructFromCursor(float drawRate, LineType lineType)
    {
        Setup(lineType);
        StartCoroutine(Drawing(drawRate));
    }

    void Setup(LineType lineType)
    {
        End = transform.position;
        pieces = new List<LinePiece>();
        stuckPieces = new List<LinePiece>();
        LineType = lineType;
        switch (lineType)
        {
            case LineType.Normal:
                material = null;
                color = Color.white;
                lengthLimit = NormalLength;
                break;
            case LineType.Ice:
                material = Slide;
                color = Color.cyan;
                lengthLimit = IceLength;
                break;
            case LineType.Rubber:
                material = Bounce;
                color = Color.black;
                lengthLimit = RubberLength;
                break;
        }
    }

    public void Add(Vector2 position, bool start)
    {
        if (Length + Vector2.Distance(End, position) > lengthLimit)
        {
            position = Vector2.Lerp(End, position, (lengthLimit - Length) / Vector2.Distance(End, position));
        }

        if (Vector2.Distance(End, position) < MinLengthForNewPiece && !start) return;

        Length += Vector2.Distance(End, position);



        pieces.Add(new LinePiece(End, position, Thickness, Circle, Box, transform, material, color, stuckPieces));
        End = position;

        if (Length == lengthLimit)
        {
            // Change the end of the last linepiece to an ending sprite
        }
    }

    public IEnumerator Drawing(float drawRate)
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0;
        Add(pos, true);
        while (Input.GetMouseButton(0))
        {
            pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Add(pos, false);
            yield return new WaitForSeconds(drawRate);
        }
    }
    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {

            // Change the end of the last linepiece to an ending sprite


            StopCoroutine(nameof(Drawing));
        }
    }
}

public class LinePiece
{
    GameObject StartCircle;
    GameObject MiddleBox;
    GameObject EndCircle;

    public LinePiece(Vector2 start, Vector2 end, float thickness, Sprite c, Sprite b, Transform parent, PhysicsMaterial2D mat, Color color, List<LinePiece> stuck)
    {
        MiddleBox = new GameObject();
        SpriteRenderer mbr = MiddleBox.AddComponent<SpriteRenderer>();
        mbr.sprite = b;
        mbr.color = color;
        EndCircle = new GameObject();
        SpriteRenderer ecr = EndCircle.AddComponent<SpriteRenderer>();
        ecr.sprite = c;
        ecr.color = color;

        if (start == end)
        {
            StartCircle = new GameObject();
            SpriteRenderer scr = StartCircle.AddComponent<SpriteRenderer>();
            scr.sprite = c;
            scr.color = color;
            StartCircle.transform.position = start;
            StartCircle.transform.localScale = new Vector3(thickness, thickness, 1);
            CircleCollider2D ccs = StartCircle.AddComponent<CircleCollider2D>();
            StartCircle.transform.up = start - end;
            ccs.sharedMaterial = mat;
            StartCircle.transform.parent = parent;
            EndCircle.transform.up = -StartCircle.transform.up;
        } 
        else 
        {
            if (stuck.Count > 0)
            {
                foreach(LinePiece p in stuck) 
                {
                    p.StartCircle.transform.up = start - end;
                    p.MiddleBox.transform.up = end - start;
                    p.EndCircle.transform.up = end - start;
                }
                stuck.Clear();
            }
            StartCircle = null;
            EndCircle.transform.up = end - start;
        }
        
        EndCircle.transform.position = end;
        EndCircle.transform.localScale = new Vector3(thickness, thickness, 1);

        float length = Vector2.Distance(start, end);
        MiddleBox.transform.localScale = new Vector3(thickness, length, 1);
        MiddleBox.transform.position = new Vector2((start.x + end.x) / 2, (start.y + end.y) / 2);
        MiddleBox.transform.up = end - start;

        BoxCollider2D bc = MiddleBox.AddComponent<BoxCollider2D>();
        bc.sharedMaterial = mat;
        CircleCollider2D cce = EndCircle.AddComponent<CircleCollider2D>();
        cce.sharedMaterial = mat;

        MiddleBox.transform.parent = parent;
        EndCircle.transform.parent = parent;

        if (start == end) stuck.Add(this);
    }
}
