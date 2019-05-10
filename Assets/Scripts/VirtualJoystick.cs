using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler {

    private Image bg_Joystick;
    private Image joystick;
    private Vector3 inputVector;


    private void Start()
    {
        bg_Joystick = GetComponent<Image>();
        joystick = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnDrag(PointerEventData eventData)
    {

        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bg_Joystick.rectTransform, eventData.position, eventData.pressEventCamera, out pos))
        {
            pos.x = (pos.x / bg_Joystick.rectTransform.sizeDelta.x);
            pos.y = (pos.y / bg_Joystick.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2, pos.y * 2,0 );
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            //Move Joystick Img
            joystick.rectTransform.anchoredPosition = new Vector3(inputVector.x * (bg_Joystick.rectTransform.sizeDelta.x / 3), inputVector.y * (bg_Joystick.rectTransform.sizeDelta.y / 3));

            Debug.Log(inputVector);
        }
        

    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector3.zero;
        joystick.rectTransform.anchoredPosition = Vector3.zero;
    }

    public float Horizontal()
    {
        if (inputVector.x != 0)
            return inputVector.x;
        else
            return Input.GetAxis("Horizontal");
    }

    public float Vertical()
    {
        if (inputVector.y != 0)
            return inputVector.y;
        else
            return Input.GetAxis("Vertical");
    }

}
