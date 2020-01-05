﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Match3 : MonoBehaviour
{
    public ArrayLayout boardLayout;
    public Sprite[] pieces;
    Node[,] board;
    int width = 9;
    int height = 14;
    System.Random random;

    // Start is called before the first frame update
    void Start()
    {

    }

    void StartGame()
    {
        string seed = getRandomSeed();
        random = new System.Random(seed.GetHashCode());
        InitializeBoard();
    }

    void InitializeBoard() //khoi tao board game va random value tren bang tu tren xuong duoi tu trai qua phai
    {
        board = new Node[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                board[x, y] = new Node(boardLayout.rows[y].row[x] ? -1 : fillPieces(), new Point(x, y));//check tai diem tren layout neu k phai la -1 thi them piece 
            }


        }

    }


    void VerifyBoard()// kiem tra neu board khi khoi tao ma da co match => renew lai
    {
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
                Point next = Point.add(p, Point.mult(p, i)); // dung de xac dinh piece tiep theo tuy theo dir minh dang check
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
            if(next >=4)
            {
                next -=4;
            }
            Point[] check = 
            {
                Point.add(p,directions[i]),
                Point.add(p,directions[next]),
                Point.add(p,Point.add(directions[i],directions[next]))
            };
            foreach (Point dir in check)
            {
                if(getValueAtPoint(dir)==value)
                {
                    square.Add(dir);
                    same ++;
                }
                
            }
            if(same>2)
            {
                AddPoints(ref connected, square);
            }
            
        }
        if(main)
        {
            for (int i = 0; i < connected.Count; i++)
            {
                AddPoints(ref connected, isConnected(connected[i],false));
            }
        }

        return connected;
    }

    void AddPoints(ref List<Point> points, List<Point> add)// add mot list co cac piece match vao mot list connected
    {

    }
    int fillPieces()
    {
        int value = 1;
        value = (random.Next(0, 100) / (100 / pieces.Length)) + 1;

        return value;
    }

    int getValueAtPoint(Point p)// lay gia tri tat mot toa do X Y tren boardLayout
    {
        return board[p.x, p.y].value;
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

    // Update is called once per frame
    void Update()
    {

    }
}


[System.Serializable]
public class Node
{
    public int value;// 0 blank 1 cube 2 sphere 3 cylinder 4 pyramid 5 diamond -1 hole
    public Point index;

    public Node(int v, Point p)
    {
        value = v;
        index = p;
    }


}
