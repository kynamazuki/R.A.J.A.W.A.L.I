using UnityEngine;

public class DroneRandomMovement : MonoBehaviour
{
    public float moveRadius = 10f;        // Max distance from starting point
    public float moveSpeed = 3f;          // Movement speed
    public float waitTime = 1f;           // Wait time at each point

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private float waitTimer = 0f;
    private bool waiting = false;

    void Start()
    {
        startPosition = transform.position;
        PickNewTarget();
    }

    void Update()
    {
        if (waiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                waiting = false;
                PickNewTarget();
            }
            return;
        }

        // Move toward the target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // If reached the target
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            waiting = true;
            waitTimer = 0f;
        }
    }

    void PickNewTarget()
    {
        // Pick a new random point within moveRadius
        Vector3 randomOffset = new Vector3(
            Random.Range(-moveRadius, moveRadius),
            Random.Range(-moveRadius, moveRadius),
            Random.Range(-moveRadius, moveRadius)
        );

        targetPosition = startPosition + randomOffset;
    }
}
