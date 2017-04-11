using UnityEngine;
using UnityEngine.EventSystems;
using System;
using GridSystem;
using UI;
public class GameInput:MonoBehaviour{

    public static event Action<Vector2> OnClick;
	void Update () {
	    if(Input.GetMouseButtonUp(0))
        {
            //战斗菜单没显示；没有点击UI，防止点击移动后，马上就移动，因为点击移动的同时，raycast hit到了ui后面的node
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                OnClick(Input.mousePosition);
            }
        }
	}
}
