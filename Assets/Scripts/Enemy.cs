using System.Collections.Generic;
using UnityEngine;

public enum EnemyState
{
    Waiting,
    Wandering
}

public class Enemy : MonoBehaviour
{
    Links links;
    Vector3Int IntPosition => links.backgroundTilemap.WorldToCell(transform.position);

    bool IsTileBackground(Vector3Int position) => links.backgroundTilemap.GetTile(position) != null;
    bool IsTileWater(Vector3Int position) => links.waterTileMap.GetTile(position) != null;
    bool IsTileBounds(Vector3Int position) => links.boundsTileMap.GetTile(position) != null;
    bool IsTileAvailable(Vector3Int position)
    {
        return IsTileBackground(position) && !IsTileWater(position) && !IsTileBounds(position);
    }

    EnemyState enemyState;
    Moveable moveable;
    Searcher searcher;

    public float timeStarted;
    public float waitingLength = 1f;
    public int newDestRadius = 3;
    public Vector3Int destination;

    List<Vector3> path;
    int nextNode;

    bool destinationReached = true;

    private void Awake()
    {
        links = FindObjectOfType<Links>();
        moveable = GetComponent<Moveable>();
        searcher = GetComponent<Searcher>();
    }

    private void Start()
    {
        StartWaiting();
    }


    public void NewDest()
    {
        while (!IsTileAvailable(CalculateNewDest()));
    }

    private Vector3Int CalculateNewDest()
    {
        int randX = Random.Range(-newDestRadius, newDestRadius + 1);
        int randY = Random.Range(-newDestRadius, newDestRadius + 1);
        Vector3Int position = IntPosition;
        position.x += randX;
        position.y += randY;
        destination = position;
        return position;
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Waiting:
                Waiting();
                break;
            case EnemyState.Wandering:
                Wandering();
                break;
            default:
                break;
        }
    }

    private void StartWaiting()
    {
        timeStarted = Time.time;
        enemyState = EnemyState.Waiting;
    }

    private void Waiting()
    {
        if (Time.time > timeStarted + waitingLength)
            StartWandering();
    }

    private void StartWandering()
    {
        if (destinationReached)
        {
            NewDest();
            Debug.Log(links.backgroundTilemap.GetCellCenterWorld(destination));
            path = links.pathFinding.FindPath(IntPosition, destination);
            nextNode = path.Count - 1;
            destinationReached = false;
            enemyState = EnemyState.Wandering;
        }
    }

    private void Wandering()
    {
        moveable.Move((path[nextNode] - transform.position).normalized);
        if ((path[nextNode] - transform.position).magnitude < .1f)
        {
            if (nextNode > 0)
                nextNode--;
            else
                destinationReached = true;
        }

        if (searcher)
            searcher.Search();

        if (destinationReached)
            StartWaiting();
    }
}
