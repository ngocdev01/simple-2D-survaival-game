using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : MonoBehaviour
{
    private PlayerController playerController;
    public LayerMask interactLayer;

    private void Awake()
    {
        playerController = gameObject.GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & interactLayer) != 0)
        {
           
            if (playerController != null)
            {
                playerController.pickUp = collision.gameObject;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & interactLayer) != 0)
        {

            if (playerController != null)
            {
                if(playerController.pickUp == collision.gameObject)
                {
                    playerController.pickUp = null;
                }
            }
        }
    }

}
