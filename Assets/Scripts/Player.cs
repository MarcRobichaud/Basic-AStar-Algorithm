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

        playerStateMachine.AddTransition(LookDirection.Up, LookDirection.Down, IsDownKeyDown, OnUpKeyDownKey);
        playerStateMachine.AddAction(LookDirection.Up, Up);
        playerStateMachine.AddAction(LookDirection.Down, Down);
    }

    void Update()
    {
        playerStateMachine.Update();
        if (moveable)
            moveable.Move(links.inputSystem.Inputs.movement);
    }

    bool IsDownKeyDown() => Input.GetKeyDown(KeyCode.DownArrow);

    public void OnUpKeyDownKey()
    {
        Debug.Log("key down");
    }

    public void Up()
    {
        Debug.Log("Up");
    }

    public void Down()
    {
        Debug.Log("Down");
    }
}
