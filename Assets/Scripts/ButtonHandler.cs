using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool isButtonDown = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        // The button is being held down.
        isButtonDown = true;
        Debug.Log("Button is held down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // The button is released.
        isButtonDown = false;
        Debug.Log("Button is released");
    }

    private void Update()
    {
        if (isButtonDown)
        {
            // Continue any actions while the button is held down.
            // For example, you can move an object or perform some other action.
        }
    }
}
