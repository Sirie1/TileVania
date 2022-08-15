using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int coinValue =100;
    bool wasCallected = false;
    void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && !wasCallected)
        {
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            FindObjectOfType<GameSession>().playerScore = FindObjectOfType<GameSession>().playerScore + coinValue;
            FindObjectOfType<GameSession>().DisplayScore();
            Destroy (gameObject);
        }    
    }


}
