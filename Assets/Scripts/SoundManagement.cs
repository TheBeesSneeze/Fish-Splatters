using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SoundManagement : MonoBehaviour
{
    Rigidbody rb;

   

    [Header("Sounds")]
    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioSource BackgroundAudio;
    [SerializeField] private AudioClip backgroundSound;
    [SerializeField] private float backgroundVolume = .25f;
    [SerializeField] private AudioClip hitBottomSound;
    [SerializeField] private float hitBottomVolume = 1f;
    [SerializeField] private AudioClip swimSound;
    [SerializeField] private float swimVolume = 1f;
    [SerializeField] private AudioClip sprintSound;
    [SerializeField] private float sprintVolume = 1f;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private float jumpVolume = 1f; 
    [SerializeField] private AudioClip smallSplashSound;
    [SerializeField] private float smallSplashVolume = 1f;
    [SerializeField] private AudioClip largeSplashSound;
    [SerializeField] private float largeSplashVolume = 1f;
    [SerializeField] private AudioClip enterBubbleSound;
    [SerializeField] private float enterBubbleVolume = 1f;
    [SerializeField] private AudioClip pickUpCoinSound;
    [SerializeField] private float pickUpCoinVolume = 1f;
    [SerializeField] private AudioClip checkpointSound;
    [SerializeField] private float checkpointVolume = 1f;
    [SerializeField] private AudioClip railSound;
    [SerializeField] private float railVolume = 1f;

   

    private bool enteredWhirlpool =false;
    private bool canJump = true;


    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Water Bottom"))
        {
           if(hitBottomSound != null)
           {
                Audio.PlayOneShot(hitBottomSound, hitBottomVolume);
           }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Water Bottom"))
        {
            Audio.Stop(); 
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bubble")
        {
            canJump = false; 
            if(enterBubbleSound != null)
            {
                Audio.PlayOneShot(enterBubbleSound, enterBubbleVolume);
            }
        }
        else if(other.gameObject.tag == "Whirlpool")
        {
            enteredWhirlpool = true; 
        }
        else if(other.gameObject.tag == "Coin")
        {
            if (pickUpCoinSound != null )
            {
                Audio.PlayOneShot(pickUpCoinSound, pickUpCoinVolume);
            }
        } 
        else if(other.gameObject.tag == "Rail")
        {
            if(railSound != null && !Audio.isPlaying)
            {
                Audio.PlayOneShot(railSound, railVolume);
            }
        }

        Checkpoint checkpoint = other.GetComponent<Checkpoint>();

        if (checkpoint != null && checkpoint.hitCheckpoint)
        {
            if(checkpointSound != null)
            {
                Audio.PlayOneShot(checkpointSound, checkpointVolume);
            }
            checkpoint.hitCheckpoint = false;  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Bubble")
        {
            canJump = true;
        }
    }

    private void Splash()
    {
        if(InputManager.Instance.splash && !Audio.isPlaying)
        {
            if(enteredWhirlpool && largeSplashSound != null)
            {
                Audio.PlayOneShot(largeSplashSound, largeSplashVolume);
                enteredWhirlpool=false;
            }
            if(smallSplashSound != null)
            {
                Audio.PlayOneShot(smallSplashSound, smallSplashVolume);
            }
            
            InputManager.Instance.splash = false;
        }
    }

    private void IsMoving()
    {
       

        if(InputManager.Instance.CurrentlyMoving && !InputManager.Instance.hasEnteredAir)
        {
            
            if(!InputManager.Instance.isHoldingSprint && swimSound != null && !Audio.isPlaying)
            {
                Audio.PlayOneShot(swimSound, swimVolume);
            }
            else if(InputManager.Instance.isHoldingSprint && sprintSound != null && !Audio.isPlaying) 
            {
                Audio.PlayOneShot(sprintSound, sprintVolume);
            }
        }

    }

    private void JumpNoise()
    {
        if (InputManager.Instance.isHoldingJump && !Audio.isPlaying && canJump)
        {
            if (jumpSound != null)
            {
                Audio.PlayOneShot(jumpSound, jumpVolume);
            }
        }
        if (InputManager.Instance.justletgo)
        {
            Audio.Stop();
        }

        
        
    }

    public void Update()
    {
        Splash();
        JumpNoise();
        IsMoving();
        
        
        if(!BackgroundAudio.isPlaying && backgroundSound != null)
        {
            BackgroundAudio.PlayOneShot(backgroundSound, backgroundVolume);
        }
    }


}
