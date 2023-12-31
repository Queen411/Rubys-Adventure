using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBoxes : MonoBehaviour
{
    public AudioClip collectgift;
    public GameObject GiftBoxesPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        RubyController controller = other.GetComponent<RubyController>();

        if (controller != null)
        {
            Destroy(gameObject);
            controller.PlaySound(collectgift);
            
            controller.ChangeGiftScore(1);
        }

    }
}
