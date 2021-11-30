using UnityEngine;

public class Player : MonoBehaviour
{
    Links links;
    Moveable moveable;

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
