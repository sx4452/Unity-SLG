using UnityEngine;
using System.Collections;
using GridSystem;
using System.Collections.Generic;

public enum Team
{
    Team1,
    Team2
}

public class GameManager : MonoBehaviour {
    public static int refWidth = 800;
    public static float unitMoveSpeed = 5f;
    public static GameManager instance;
    public static Unit selectedUnit;
    public static GameObject selectedUnitNodeObj;

    private LayerMask unitLayerMask = 1 << 9;

    public static bool toSelectAttackee;
    private bool wait;
	void Start()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        //区分阵营
        //GameObject.Find("Player").GetComponent<Unit>().Team = Team.Team1;
        //GameObject.Find("Enermy").GetComponent<Unit>().Team = Team.Team2;
        //GameObject.Find("Enermy (1)").GetComponent<Unit>().Team = Team.Team2;

        GameInput.OnClick += OnClick;
        toSelectAttackee = false;
        wait = false;
    }

    void OnClick(Vector2 clickPos)
    {
        if(!wait)
        {
            Ray ray = Camera.main.ScreenPointToRay(clickPos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, unitLayerMask))
            {
                GameObject hitGo = hit.collider.gameObject;
                Unit hitUnit = hitGo.GetComponent<Unit>();

                if (!toSelectAttackee)
                {
                    setSelectedUnit(hitUnit);
                    showBattleMenu(false);
                }
                else
                {
                    Node hitNode = Grid.instance.getNodeObjFromPosition(hitGo.transform.position).GetComponent<Node>();
                    if (hitNode.Status == NodeStatus.Attackable)
                    {
                        Vector3 faceDir = (hitGo.transform.position - selectedUnit.transform.position).normalized;
                        selectedUnit.transform.forward = faceDir;
                        wait = true;
                        selectedUnit.attack(onAttackComplete);
                    }
                }
            }
            else//没有点击中攻击人物的话，取消攻击状态
            {
                toSelectAttackee = false;
            }
        }
    }

    private  void onAttackComplete()
    {
        wait = false;
        toSelectAttackee = false;
    }

    private void setSelectedUnit(Unit unit)
    {
        Grid.instance.clear();
        selectedUnit = unit;
        selectedUnitNodeObj = Grid.instance.getNodeObjFromPosition(unit.transform.position);
    }

    public void showBattleMenu(bool moved)
    {
        List<GameObject> AttackableNodeObjs = Grid.instance.getAttackableNodeObjs();

        Vector3 menuPos = Camera.main.WorldToScreenPoint(selectedUnit.transform.position);

        float resolutionRatio = (float)Screen.width / refWidth;
        menuPos.x = menuPos.x / resolutionRatio + 50;
        menuPos.y = menuPos.y / resolutionRatio + 50;

        BattleMenu.instance.gameObject.SetActive(true);
        RectTransform battleMenuRectTrans = BattleMenu.instance.gameObject.GetComponent<RectTransform>();
        battleMenuRectTrans.anchoredPosition = menuPos;

        BattleMenu.instance.AttackBtnObj.SetActive(AttackableNodeObjs != null && AttackableNodeObjs.Count > 0);
        BattleMenu.instance.MoveBtnObj.SetActive(!moved);
    }

}
