using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();
    public int coins = 0;
    public TMP_Text coinText;

    [Header("Coin Sounds")]
    private float coinPickUpVolume = 1.0f;
    private AudioClip coinPickUpSound;


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            if(coinPickUpSound != null)
            {
                AudioSource.PlayClipAtPoint(coinPickUpSound, transform.position, coinPickUpVolume);
            }

            coins += other.GetComponent<CoinBehavior>().coinValue;
            Destroy(other.gameObject);
            coinText.text = "Coins: " + coins;
        }
    }
}
