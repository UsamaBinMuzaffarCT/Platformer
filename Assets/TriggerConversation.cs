using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TriggerConversation : MonoBehaviour
{
    public GameObject usableGameObject = null;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var usable = collision.GetComponent<Usable>();
        if(usable != null)
        {
            usableGameObject = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var usable = collision.GetComponent<Usable>();
        if (usable != null)
        {
            usableGameObject = null;
        }
    }

    public void Use()
    {
        if(usableGameObject != null)
        {
            usableGameObject.SendMessage("OnUse", transform, SendMessageOptions.DontRequireReceiver);
        }
    }
}
