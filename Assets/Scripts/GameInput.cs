using UnityEngine;
using System;
public class GameInput:MonoBehaviour{

    public static event Action<Vector2> OnMouseLeftClick;
    public static float unitMoveSpeed = 5f;
	void Update () {
	    if(Input.GetMouseButtonUp(0))
        {
            OnMouseLeftClick(Input.mousePosition);
        }
	}
}
