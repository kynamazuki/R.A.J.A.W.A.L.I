using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slide : MonoBehaviour
{
    public GameObject Logo;

    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;

    public float slideSpeed = 1000f;

    private bool isSliding = false;

    void Start()
    {
        if (Logo == null) return;

        // Save the target position (where the logo should stop)
        visiblePosition = Logo.transform.position;

        // Calculate the hidden position (off-screen left)
        float logoWidth = Logo.GetComponent<RectTransform>().rect.width;
        hiddenPosition = new Vector3(-logoWidth, visiblePosition.y, visiblePosition.z);

        // Start logo hidden off-screen
        Logo.transform.position = hiddenPosition;
        Logo.SetActive(true);

        isSliding = true;
    }

    void Update()
    {
        if (isSliding)
        {
            Logo.transform.position = Vector3.MoveTowards(
                Logo.transform.position,
                visiblePosition,
                slideSpeed * Time.deltaTime
            );

            // Stop sliding when it reaches the target
            if (Vector3.Distance(Logo.transform.position, visiblePosition) < 0.1f)
            {
                Logo.transform.position = visiblePosition;
                isSliding = false;
            }
        }
    }
}
