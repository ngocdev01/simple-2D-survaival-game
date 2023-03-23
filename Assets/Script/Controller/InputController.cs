using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    
    public Vector2 controlVector => GetControlVector();
    public Vector3 mousePosition => GetMousePos();
    public bool fireButton => Input.GetMouseButton(0);

    public bool pickUpPresses => Input.GetKeyDown(KeyCode.E);
    private Vector2 GetControlVector()
    {
        float vecX = Input.GetAxisRaw("Horizontal");
        float vecY = Input.GetAxisRaw("Vertical");
        return new Vector2(vecX, vecY);
    }
   
    private Vector2 GetMousePos()
    {
        return GameManager.Instance.MainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
