using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;
    [Header("UI Elements")]
    public Sprite[] pieces;
    public RectTransform gameBoard;
    [Header("Prefabs")]
    public GameObject nodePiece;
    List<NodePiece> update;
    List<FlippedPieces> flipped;
    List<NodePiece> dead;


    Node[,] board;
    int width = 9;
    int height = 14;
    System.Random random;



    // Start is called before the first frame update
    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        update = new List<NodePiece>();
        flipped = new List<FlippedPieces>();
        dead = new List<NodePiece>();
        InitializeBoard();
        VerifyBoard();
        InstantiateBoard();
    }
    void Update()
    {
        List<NodePiece> finishedUpdating = new List<NodePiece>();
        for (int i = 0; i < update.Count; i++)
        {
            NodePiece piece = update[i];
            bool updating = piece.UpdatePiece();
            if (!updating)
            {
                finishedUpdating.Add(piece);
            }
        }
        for (int i = 0; i < finishedUpdating.Count; i++)
        {
            NodePiece piece = finishedUpdating[i];
            FlippedPieces flip = getFlipped(piece);
            NodePiece flippedPiece = null;

            List<Point> connected = isConnected(piece.index, true);
            bool wasFlipped = (flip != null);

            if (wasFlipped) // if we flipped to make this update
            {
                flippedPiece = flip.getOtherPiece(piece);
                AddPoints(ref connected, isConnected(flippedPiece.index, true));
            }

            if (connected.Count == 0)// if we didnt have a match
            {
                if (wasFlipped)// if we flipped
                    FlipPieces(piece.index, flippedPiece.index, false);// flip back 
            }
            else // if we make a match 
            {
                foreach (Point pnt in connected)// remove node pieces when connected
                {
                    Node node = getNodeAtPoint(pnt);
                    NodePiece nodePiece = node.GetPiece();
                    if (nodePiece != null)
                    {
                        nodePiece.gameObject.SetActive(false);
                        dead.Add(piece);
                    }
                    node.SetPiece(null);
                }
                ApplyGravityToBoard();
            }
            flipped.Remove(flip);// remove the flip after update 
            update.Remove(piece);
        }
    }

    void ApplyGravityToBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = (height - 1); y >= 0; y--)
            {
                Point p = new Point(x, y);
                Node node = getNodeAtPoint(p);
                int val = getValueAtPoint(p);
                if (val != 0) continue; // if it is not a hole do nothing
                for (int ny = (y - 1); ny >= -1; ny--)
                {
                    Point next = new Point(x, ny);
                    int nextVal = getValueAtPoint(next);
                    if (nextVal == 0) continue;
                    if (nextVal != -1) //if we did not hit an end, but its not 0 then use this to fill the hole
                    {
                        Node got = getNodeAtPoint(next);
                        NodePiece piece = got.GetPiece();
                        //set the hole
                        node.SetPiece(piece);
                        update.Add(piece);
                        //replace the hole
                        got.SetPiece(null);
                    }
                    else//hit an end
                    {
                        // fill the hole
                        int newVal = fillPieces();
                        NodePiece piece;
                        if (dead.Count > 0)
                        {
                            NodePiece revived = dead[0];
                            revived.gameObject.SetActive(true);
                            revived.rect.anchoredPosition = getPositionFromPoint(new Point(x, -1));
                            piece = revived;

                            dead.RemoveAt(0);
                        }
                        else
                        {
                            GameObject obj = Instantiate(nodePiece, gameBoard);
                            NodePiece n = obj.GetComponent<NodePiece>();
                            RectTransform rect = obj.GetComponent<RectTransform>();
                            rect.anchoredPosition = getPositionFromPoint(new Point(x, -1));
                            piece = n;

                        }
                        piece.Initialize(newVal, p, pieces[newVal - 1]);

                        Node hole = getNodeAtPoint(p);
                        hole.SetPiece(piece);
                        ResetPiece(piece);
                    }
                    break;
                }

            }
        }
    }

    FlippedPieces getFlipped(NodePiece p)
    {
        FlippedPieces flip = null;
        for (int i = 0; i < flipped.Count; i++)
        {
            if (flipped[i].getOtherPiece(p) != null)
            {
                flip = flipped[i];
                break;
            }
        }
        return flip;
    }

    void InitializeBoard() //khoi tao board game va random value tren bang tu tren xuong duoi tu trai qua phai
    {
        board = new Node[width, height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                board[x, y] = new Node(boardLayout.rows[y].row[x] ? -1 : fillPieces(), new Point(x, y));//check tai diem tren layout neu k phai la -1 thi them piece 
            }


        }

    }


    void VerifyBoard()// kiem tra neu board khi khoi tao ma da co match => renew lai
    {
        List<int> remove;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Point p = new Point(x, y);
                int value = getValueAtPoint(p);//lay gia tri cua piece ra check 
                if (value <= 0)// new Piece la mot blank hoac la hole thi se khong lam gi 
                {
                    continue;
                }
                else
                {
                    remove = new List<int>();
                    while (isConnected(p, true).Count > 0)
                    {
                        value = getValueAtPoint(p);
                        if (!remove.Contains(value))// check neu value la mot match thi phai remove no khoi list random
                        {
                            remove.Add(value);
                        }
                        setValueAtPoint(p, newValue(ref remove)); //add 1 value moi vao pieces khong co la value match
                    }
                }


            }

        }

    }

    //create all the pieces
    void InstantiateBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Node node = getNodeAtPoint(new Point(x, y));
                int val = node.value;
                if (val <= 0)
                {
                    continue;
                }
                GameObject p = Instantiate(nodePiece, gameBoard);
                NodePiece piece = p.GetComponent<NodePiece>();
                RectTransform rect = p.GetComponent<RectTransform>();
                rect.anchoredPosition = new Vector2(32 + (64 * x), -32 - (64 * y));
                piece.Initialize(val, new Point(x, y), pieces[val - 1]);
                node.SetPiece(piece);
            }
        }
    }
    List<Point> isConnected(Point p, bool main)// neu la mot match thi tat ca se bo vao  List de follow
    {
        List<Point> connected = new List<Point>();
        int value = getValueAtPoint(p);
        Point[] directions =// direction cua 4 huong up down left right tu class Point 
        {
            Point.up,
            Point.right,
            Point.down,
            Point.left
        };
        //check neu co 2 piece giong nhu o mot huong [0]00
        foreach (Point dir in directions)
        {
            List<Point> line = new List<Point>();
            int same = 0;
            for (int i = 1; i < 3; i++)// 1 la de check value tiep theo chu khong phai value minh dang dung
            {
                Point next = Point.add(p, Point.mult(dir, i)); // dung de xac dinh piece tiep theo tuy theo dir minh dang check
                if (getValueAtPoint(next) == value)// check xem piece dang dung voi  2 piece tiep theo co phai la 1 match neu la match thi add vao list
                {
                    line.Add(next);
                    same++;

                }
            }
            if (same > 1)// neu co 2 piece tiep theo la mot match thi add vao list connected
            {
                AddPoints(ref connected, line);
            }
        }
        //check 2 piece benh canh piece hien tai co match hay khong 0[0]0
        for (int i = 0; i < 2; i++)
        {
            List<Point> line = new List<Point>();
            int same = 0;
            Point[] next = { Point.add(p, directions[i]), Point.add(p, directions[i + 2]) };// tao 1 array chua' 2 point 2 doi chien vi tri hien tai de check
            foreach (Point dir in next)
            {
                if (getValueAtPoint(dir) == value)// neu match thi them vao list
                {
                    line.Add(dir);
                    same++;
                }
            }
            if (same > 1)
            {
                AddPoints(ref connected, line);
            }

        }

        // check xem 2x2 co phai la 1 match thi check theo hinh vuoong va o cheo
        //  0 0
        // [0]0
        for (int i = 0; i < 4; i++)//check 4 huong vi trong array direction bat dau tu 0-3
        {
            List<Point> square = new List<Point>();
            int same = 0;
            int next = i + 1;
            if (next >= 4)
            {
                next -= 4;
            }
            Point[] check =
            {
                Point.add(p,directions[i]),
                Point.add(p,directions[next]),
                Point.add(p,Point.add(directions[i],directions[next]))
            };
            foreach (Point dir in check)
            {
                if (getValueAtPoint(dir) == value)
                {
                    square.Add(dir);
                    same++;
                }

            }
            if (same > 2)
            {
                AddPoints(ref connected, square);
            }

        }
        if (main)
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, isConnected(connected[i], false));
            }
        }


        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)// add mot list co cac piece match vao mot list connected
    {
        foreach (Point p in add)
        {
            bool doAdd = true;
            for (int i = 0; i < points.Count; i++)
            {
                if (points[i].Equals(p))
                {
                    doAdd = false;
                    break;
                }
            }

            if (doAdd)
            {
                points.Add(p);
            }

        }
    }
    int fillPieces()
    {
        int value = 1;
        value = (random.Next(0, 100) / (100 / pieces.Length)) + 1;

        return value;
    }

    int getValueAtPoint(Point p)// lay gia tri tat mot toa do X Y tren boardLayout
    {
        if (p.x < 0 || p.x >= width || p.y < 0 || p.y >= height) return -1; //de dam bao no k bi out of range
        return board[p.x, p.y].value;
    }

    void setValueAtPoint(Point p, int v)
    {
        board[p.x, p.y].value = v;
    }

    int newValue(ref List<int> remove)
    {
        List<int> available = new List<int>();
        for (int i = 0; i < pieces.Length; i++)
        {
            available.Add(i + 1);
        }
        foreach (int item in remove)
        {
            available.Remove(item);
        }

        if (available.Count <= 0)
        {
            return 0;
        }
        return available[random.Next(0, available.Count)];
    }

    private string getRandomSeed()
    {
        string seed = "";
        string acceptableChars = "QWERTYUIOPASDFGHJKLZXCBVBNMqwertyuiopasdfghjklzxcvbnm123456789!@#$%^&*()";
        for (int i = 0; i < 20; i++)
        {
            seed += acceptableChars[Random.Range(0, acceptableChars.Length)];
        }
        return seed;
    }
    public void ResetPiece(NodePiece piece)
    {
        piece.ResetPosition();
        update.Add(piece);
    }

    public void FlipPieces(Point one, Point two, bool main)
    {
        if (getValueAtPoint(one) < 0) return;
        Node nodeOne = getNodeAtPoint(one);
        NodePiece pieceOne = nodeOne.GetPiece();
        if (getValueAtPoint(two) > 0)
        {
            Node nodeTwo = getNodeAtPoint(two);
            NodePiece pieceTwo = nodeTwo.GetPiece();
            nodeTwo.SetPiece(pieceOne);
            nodeOne.SetPiece(pieceTwo);

            if (main)
                flipped.Add(new FlippedPieces(pieceOne, pieceTwo));

            update.Add(pieceTwo);
            update.Add(pieceOne);
        }
        else
            ResetPiece(pieceOne);

    }
    public Vector2 getPositionFromPoint(Point p)
    {
        return new Vector2(32 + (64 * p.x), -32 - (64 * p.y));
    }

    // Update is called once per frame
    Node getNodeAtPoint(Point p)
    {
        return board[p.x, p.y];
    }

}


[System.Serializable]
public class Node
{
    public int value;// 0 blank 1 cube 2 sphere 3 cylinder 4 pyramid 5 diamond -1 hole
    public Point index;
    NodePiece piece;

    public Node(int v, Point p)
    {
        value = v;
        index = p;
    }

    public void SetPiece(NodePiece p)
    {
        piece = p;
        value = (piece == null) ? 0 : piece.value;
        if (piece == null) return;
        piece.SetIndex(index);
    }

    public NodePiece GetPiece()
    {
        return piece;
    }


}

[System.Serializable]
public class FlippedPieces
{
    public NodePiece one;
    public NodePiece two;

    public FlippedPieces(NodePiece o, NodePiece t)
    {
        one = o;
        two = t;
    }

    public NodePiece getOtherPiece(NodePiece p)
    {
        if (p == one)
            return two;
        else if (p == two)
            return one;
        else
            return null;
    }
}
