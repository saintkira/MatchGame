using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePieces : MonoBehaviour
{
    public static MovePieces instance;
    Match3 game;
    NodePiece moving;
    Point newIndex;
    Vector2 mouseStart;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        game = GetComponent<Match3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (moving != null)
        {
            Vector2 dir = ((Vector2)Input.mousePosition - mouseStart);
            Vector2 nDir = dir.normalized;
            Vector2 aDir = new Vector2(Mathf.Abs(dir.x), Mathf.Abs(dir.y));

            newIndex = Point.clone(moving.index);
            Point add = Point.zero;

            if (dir.magnitude > 32) //if the mouse is 32 pixel away from the starting of the mouse
            {
                //change add into (1,0)||(-1,0)||(0,1)||(0,-1)
                if (aDir.x > aDir.y)
                    add = (new Point((nDir.x > 0) ? 1 : -1, 0));
                else if (aDir.y > aDir.x)
                    add = (new Point(0, (nDir.y > 0) ? -1 : 1));

            }
            newIndex.add(add);

            Vector2 pos = game.getPositionFromPoint(moving.index);
            if (!newIndex.Equals(moving.index))
                pos += Point.mult(new Point(add.x, -add.y), 16).toVector();

            moving.MovePositionTo(pos);
        }
    }

    public void MovePiece(NodePiece piece)
    {
        if (moving != null) return;
        moving = piece;
        mouseStart = Input.mousePosition;
    }

    public void DropPiece()
    {
        if (moving == null) return;
        if(!newIndex.Equals(moving.index))
        game.FlipPieces(moving.index, newIndex, true);
        //Fliup the piece around in the game board
        //else
        //Reset the piece the original position
        else
        game.ResetPiece(moving);
        moving = null;




    }
}
