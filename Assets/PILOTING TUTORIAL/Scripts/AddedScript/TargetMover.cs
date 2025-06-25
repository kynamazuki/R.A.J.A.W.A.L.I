using UnityEngine;

public class TargetMover : MonoBehaviour
{
    public float speed = 2f;          // Movement speed
    public float distance = 3f;       // Distance from start position
    public bool moveHorizontally = true; // Toggle for horizontal or vertical movement

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        float movement = Mathf.PingPong(Time.time * speed, distance * 2) - distance;

        if (moveHorizontally)
        {
            transform.position = new Vector3(startPos.x + movement, startPos.y, startPos.z);
        }
        else
        {
            transform.position = new Vector3(startPos.x, startPos.y + movement, startPos.z);
        }
    }
}
