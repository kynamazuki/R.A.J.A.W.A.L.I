using UnityEngine;
using VSX.UniversalVehicleCombat;
using VSX.ObjectivesSystem;

public class RingTrigger : MonoBehaviour
{
    private Damageable damageable;


    private void Awake()
    {
        damageable = GetComponent<Damageable>();
        if (damageable == null)
        {
            Debug.LogError("Damageable component is missing on " + gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        
        // You may want to check if it's the player first
        if (other.CompareTag("Player"))
        {
            if (damageable != null && !damageable.Destroyed)
            {
                damageable.SetHealth(0); // Make sure this sets health to 0
                Debug.Log("Ring triggered by " + other.name);
           
            }

            
        }
    }
}
