using UnityEngine;

public class Searcher : MonoBehaviour
{
    public int detectionRange;

    Links links;

    bool IsInRange(Vector3 v) => (v - transform.position).sqrMagnitude <= detectionRange * detectionRange;

    private void Awake()
    {
        links = FindObjectOfType<Links>();
    }

    public void Search()
    {
        foreach (var foundable in links.foundables)
        {
            if (IsInRange(foundable.transform.position))
            {
                Vector2 direction = (foundable.transform.position - transform.position).normalized;
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, detectionRange, LayerMask.GetMask("Obstacle", "Player"));
                if (hit && hit.transform.gameObject.layer == LayerMask.NameToLayer("Player"))
                    Debug.Log("Player detected");
            }
        }
    }
}
