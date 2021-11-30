using UnityEngine;

public class Moveable : MonoBehaviour
{
    public float speed = 8;

    Links links;
    Rigidbody2D rb;
    Vector2 previousMovement = Vector2.zero;
    Vector2 movement = Vector2.zero;

    private void Awake()
    {
        
        links = FindObjectOfType<Links>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 _movement)
    {
        movement = _movement;
        rb.velocity = movement * speed;
        if (previousMovement != movement)
        {
            Vector3Int vect = links.backgroundTilemap.WorldToCell(transform.position);
            transform.position = links.backgroundTilemap.GetCellCenterWorld(vect);
        }
        previousMovement = movement;
    }
}
