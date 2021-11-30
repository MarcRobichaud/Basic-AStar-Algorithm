using UnityEngine;

public class Player : MonoBehaviour
{
    Links links;
    Moveable moveable;

    public StateMachine<LookDirection> playerStateMachine = new StateMachine<LookDirection>(LookDirection.Up);

    private void Awake()
    {
        links = FindObjectOfType<Links>();
        moveable = GetComponent<Moveable>();
    }

    void Update()
    {
        if (moveable)
            moveable.Move(links.inputSystem.Inputs.movement);
    }
}
