using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BMousePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }
    

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        player.OnVirtualMouseDown();
    }

    void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
    {
        player.OnVirtualMouseUp();
    }
}
