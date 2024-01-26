using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Bamboo") 
            || collision.CompareTag("Obstacle") || collision.CompareTag("Coin") || collision.CompareTag("Trap"))
        {
            collision.gameObject.SetActive(false);
        }
    }
}
