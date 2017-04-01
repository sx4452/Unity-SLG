using UnityEngine;
using UnityEngine.EventSystems;
using System;
using GridSystem;
public class GameInput:MonoBehaviour{

    public static event Action<Vector2> OnClick;
	void Update () {
	    if(Input.GetMouseButtonUp(0))
        {
            
            if (BattleMenu.instance.gameObject.activeSelf)
            {
                BattleMenu.instance.gameObject.SetActive(false);
            }

            if (!EventSystem.current.IsPointerOverGameObject())//没有点击UI
            {
                OnClick(Input.mousePosition);
            }
        }
	}
}
