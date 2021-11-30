using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Links : MonoBehaviour
{
    public AStar pathFinding;
    public InputSystem inputSystem;
    public Tilemap backgroundTilemap;
    public Tilemap waterTileMap;
    public Tilemap boundsTileMap;
    public List<Foundable> foundables;
}
