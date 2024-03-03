using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();
    public int coins = 0;
    public TMP_Text coinText;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Coin")
        {
            //if(coin noise != null)
            //{ make the noise}
            Debug.LogWarning("Coin Noise"); 
            coins += other.GetComponent<CoinBehavior>().coinValue;
            Destroy(other.gameObject);
            coinText.text = "Coins: " + coins;
        }
    }
}
