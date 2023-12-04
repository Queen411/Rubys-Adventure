using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCandyCollectible : MonoBehaviour
{
    public AudioClip collectedClip;
    public GameObject CandyPickupPrefab;

    private float speedUp = 9.0f;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();
        if (controller != null)
        { 
            controller.boosting = true;
            controller.speed = speedUp;
            controller.PlaySound(collectedClip);
            Destroy(gameObject);

        }
    }
}