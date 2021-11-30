using UnityEngine;

public class InputSystem : MonoBehaviour
{
    public InputPkg Inputs { get; private set; }

    private KeyCode up = KeyCode.W;
    private KeyCode down = KeyCode.S;
    private KeyCode left = KeyCode.A;
    private KeyCode right = KeyCode.D;

    private void Awake()
    {
        Inputs = new InputPkg();
    }

    private void Update()
    {
        Inputs.UpdateMovement(Input.GetKey(up), Input.GetKey(down), Input.GetKey(left), Input.GetKey(right));
    }
}

public class InputPkg
{
    public Vector2 movement;

    public void UpdateMovement(bool up, bool down, bool left, bool right)
    {
        movement = Vector2.zero;
        if (up)
            movement += Vector2.up;
        if (down)
            movement += Vector2.down;
        if (left)
            movement += Vector2.left;
        if (right)
            movement += Vector2.right;
    }
}
