using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum DirectionToCheck
{
    Up,
    Down,
    Left,
    Right
}

public class AStar : MonoBehaviour
{
    public static Vector2Int NeighbourPos(DirectionToCheck direction, Vector2Int pos) => direction switch
    {
        DirectionToCheck.Up => pos + Vector2Int.up,
        DirectionToCheck.Right => pos + Vector2Int.right,
        DirectionToCheck.Down => pos + Vector2Int.down,
        DirectionToCheck.Left => pos + Vector2Int.left,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), $"Not expected direction value: {direction}"),
    };

    public static Vector2Int MaxVector2Int = new Vector2Int(int.MaxValue, int.MaxValue);
    public static Vector3Int Vect3IntToVect2(Vector2Int vect3)
    {
        Vector3Int vect = new Vector3Int
        {
            x = vect3.x,
            y = vect3.y,
            z = 0
        };
        return vect;
    }

    public Tilemap NonWalkableTile;
    public Tilemap DrawableTile;

    public TileBase pathTile;
    public TileBase closedListTile;

    Dictionary<Vector2Int, Tile> closedList = new Dictionary<Vector2Int, Tile>();
    Dictionary<Vector2Int, Tile> openList = new Dictionary<Vector2Int, Tile>();

    bool IsAlreadyInList(Vector2Int pos) => openList.ContainsKey(pos) || closedList.ContainsKey(pos);
    bool IsCellWalkable(Vector2 pos) => NonWalkableTile.GetTile(NonWalkableTile.WorldToCell(pos)) == null;
    Vector2Int SmallestFPosition()
    {
        Vector2Int smallestF = MaxVector2Int;
        foreach (Vector2Int pos in openList.Keys)
        {
            if (smallestF == MaxVector2Int || openList[pos].F < openList[smallestF].F)
                smallestF = pos;
        }
        return smallestF;
    }

    Vector2Int startingPosition = MaxVector2Int;
    Vector2Int destinationPosition = MaxVector2Int;
    Vector2Int currentPosition;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3Int vect3 = NonWalkableTile.WorldToCell(pos);
            Vector2Int vect2 = new Vector2Int
            {
                x = vect3.x,
                y = vect3.y
            };
            if (startingPosition == MaxVector2Int)
            {
                startingPosition = vect2;
            }
            else if (destinationPosition == MaxVector2Int)
            {
                destinationPosition = vect2;
                StartCoroutine(FindPath());
            }
        }
    }

    IEnumerator FindPath()
    {
        Init();
        AddNeighboursToOpenList();
        while (openList.Count > 0 && currentPosition != destinationPosition)
        {
            FindSmallestF();
            AddNeighboursToOpenList();
            yield return new WaitForSeconds(0.1f);
        }

        if (currentPosition == destinationPosition)
        {
            while (closedList[currentPosition].lastTile != null)
            {
                DrawableTile.SetTile(Vect3IntToVect2(currentPosition), pathTile);
                currentPosition = closedList[currentPosition].lastTile.position;
            }
        }
        ResetPathFinding();
    }

    private void Init()
    {
        Tile tile = new Tile(startingPosition ,null, Tile.GetH(startingPosition, destinationPosition));
        closedList.Add(startingPosition, tile);
        currentPosition = startingPosition;
    }

    private void FindSmallestF()
    {
        Vector2Int smallestFPos = SmallestFPosition();
        closedList.Add(smallestFPos, openList[smallestFPos]);
        openList.Remove(smallestFPos);
        currentPosition = smallestFPos;
        DrawableTile.SetTile(Vect3IntToVect2(smallestFPos), closedListTile);
    }

    public void AddNeighboursToOpenList()
    {
        closedList.TryGetValue(currentPosition, out Tile lastTile);

        foreach (DirectionToCheck direction in (DirectionToCheck[])Enum.GetValues(typeof(DirectionToCheck)))
        {
            AddNeighbourToOpenList(NeighbourPos(direction, currentPosition), lastTile);
        }
    }

    public void AddNeighbourToOpenList(Vector2Int neighbourPos, Tile lastTile)
    {
        bool isAlreadyInList = IsAlreadyInList(neighbourPos);

        if (!isAlreadyInList && IsCellWalkable(neighbourPos))
        {
            Tile tile = new Tile(neighbourPos, lastTile, Tile.GetH(neighbourPos, destinationPosition));
            openList.Add(neighbourPos, tile);
        }
        else if (openList.ContainsKey(neighbourPos) && openList[neighbourPos].lastTile.G > lastTile.G)
        {
            openList[neighbourPos].lastTile = lastTile;
            openList[neighbourPos].G = lastTile.G + 1;
        }
    }

    public void ResetPathFinding()
    {
        startingPosition = MaxVector2Int;
        destinationPosition = MaxVector2Int;
        openList.Clear();
        closedList.Clear();
    }
}

public class Tile
{
    public Vector2Int position;
    public Tile lastTile;
    public int H;
    public int G;
    public int F => H + G;

    public static int GetH(Vector2Int pos, Vector2Int dest) 
    {
        int deltaX = Math.Abs(pos.x - dest.x);
        int deltaY = Math.Abs(pos.y - dest.y);

        return deltaX + deltaY;
    }

    public Tile(Vector2Int pos, Tile _lastTile, int _H)
    {
        position = pos;
        lastTile = _lastTile;
        H = _H;
        G = (lastTile == null) ? 0 : lastTile.G + 1;
    }
}
