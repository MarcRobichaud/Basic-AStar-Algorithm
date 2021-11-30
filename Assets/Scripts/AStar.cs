using System;
using System.Collections.Generic;
using UnityEngine;

public enum DirectionToCheck
{
    Up,
    Down,
    Left,
    Right
}

public class AStar : MonoBehaviour
{
    Links links;

    private void Awake()
    {
        links = FindObjectOfType<Links>();
    }

    public static Vector3Int NeighbourPos(DirectionToCheck direction, Vector3Int pos) => direction switch
    {
        DirectionToCheck.Up => pos + Vector3Int.up,
        DirectionToCheck.Right => pos + Vector3Int.right,
        DirectionToCheck.Down => pos + Vector3Int.down,
        DirectionToCheck.Left => pos + Vector3Int.left,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), $"Not expected direction value: {direction}"),
    };

    public static Vector3Int MaxVector3Int = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);

    Dictionary<Vector3Int, Tile> closedList = new Dictionary<Vector3Int, Tile>();
    Dictionary<Vector3Int, Tile> openList = new Dictionary<Vector3Int, Tile>();

    bool IsAlreadyInList(Vector3Int pos) => openList.ContainsKey(pos) || closedList.ContainsKey(pos);
    bool IsCellWalkable(Vector3Int pos) => links.waterTileMap.GetTile(pos) == null && links.boundsTileMap.GetTile(pos) == null;
    Vector3Int SmallestFPosition()
    {
        Vector3Int smallestF = MaxVector3Int;
        foreach (Vector3Int pos in openList.Keys)
        {
            if (smallestF == MaxVector3Int || openList[pos].F < openList[smallestF].F)
                smallestF = pos;
        }
        return smallestF;
    }

    Vector3Int startingPosition = MaxVector3Int;
    Vector3Int destinationPosition = MaxVector3Int;
    Vector3Int currentPosition;

    public List<Vector3> FindPath(Vector3Int startingPos, Vector3Int destinationPos)
    {
        startingPosition = startingPos;
        destinationPosition = destinationPos;
        Init();
        AddNeighboursToOpenList();
        while (openList.Count > 0 && currentPosition != destinationPosition)
        {
            FindSmallestF();
            AddNeighboursToOpenList();
        }

        List<Vector3> path = new List<Vector3>();
        if (currentPosition == destinationPosition)
        {
            Vector3 vect3 = links.backgroundTilemap.GetCellCenterWorld(closedList[currentPosition].position);
            path.Add(vect3);
            while (closedList[currentPosition].lastTile != null)
            {
                currentPosition = closedList[currentPosition].lastTile.position;
                vect3 = links.backgroundTilemap.GetCellCenterWorld(closedList[currentPosition].position);
                path.Add(vect3);
            }
        }
        ResetPathFinding();
        return path;
    }

    private void Init()
    {
        Tile tile = new Tile(startingPosition ,null, Tile.GetH(startingPosition, destinationPosition));
        closedList.Add(startingPosition, tile);
        currentPosition = startingPosition;
    }

    private void FindSmallestF()
    {
        Vector3Int smallestFPos = SmallestFPosition();
        closedList.Add(smallestFPos, openList[smallestFPos]);
        openList.Remove(smallestFPos);
        currentPosition = smallestFPos;
    }

    public void AddNeighboursToOpenList()
    {
        closedList.TryGetValue(currentPosition, out Tile lastTile);

        foreach (DirectionToCheck direction in (DirectionToCheck[])Enum.GetValues(typeof(DirectionToCheck)))
        {
            AddNeighbourToOpenList(NeighbourPos(direction, currentPosition), lastTile);
        }
    }

    public void AddNeighbourToOpenList(Vector3Int neighbourPos, Tile lastTile)
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
        startingPosition = MaxVector3Int;
        destinationPosition = MaxVector3Int;
        openList.Clear();
        closedList.Clear();
    }
}

public class Tile
{
    public Vector3Int position;
    public Tile lastTile;
    public int H;
    public int G;
    public int F => H + G;

    public static int GetH(Vector3Int pos, Vector3Int dest) 
    {
        int deltaX = Math.Abs(pos.x - dest.x);
        int deltaY = Math.Abs(pos.y - dest.y);

        return deltaX + deltaY;
    }

    public Tile(Vector3Int pos, Tile _lastTile, int _H)
    {
        position = pos;
        lastTile = _lastTile;
        H = _H;
        G = (lastTile == null) ? 0 : lastTile.G + 1;
    }
}
