using UnityEngine;
using GridSystem;
using System.Collections.Generic;

public enum Team
{
    Team1,
    Team2
}

public class GameManager : MonoBehaviour {
    //public static int refWidth = 800;
    public static float unitMoveSpeed = 5f;
    public static GameManager instance;
    public static Unit selectedUnit;
    public static GameObject selectedUnitNodeObj;

    public static GameObject CanvasObj;
    public static GameObject healthBarPrefab;

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

        ////区分阵营
        //GameObject.Find("WK_heavy_infantry").GetComponent<Unit>().Team = Team.Team1;
        //GameObject.Find("WK_light_infantry").GetComponent<Unit>().Team = Team.Team2;
        //GameObject.Find("WK_archer").GetComponent<Unit>().Team = Team.Team1;
        //GameObject.Find("Enermy (1)").GetComponent<Unit>().Team = Team.Team2;

        GameInput.OnClick += OnClick;
        toSelectAttackee = false;
        wait = false;

        initUnits();

        CanvasObj = GameObject.FindGameObjectWithTag("Canvas") as GameObject;
        healthBarPrefab = Resources.Load("Prefabs/UI/HealthBar") as GameObject;
    }

    /// <summary>
    ///创建角色
    /// </summary>
    private void initUnits()
    {
        GameObject swordManPrefab = Resources.Load("Prefabs/Units/SwordMan") as GameObject;
        GameObject spearManPrefab = Resources.Load("Prefabs/Units/SpearMan") as GameObject;
        GameObject archerPrefab = Resources.Load("Prefabs/Units/Archer") as GameObject;

        Quaternion team1Rot = Quaternion.Euler(0, 90, 0);
        GameObject swordMan1 = Instantiate(swordManPrefab, Grid.instance.NodeObjs[0, 3].transform.position, team1Rot);
        swordMan1.GetComponent<Unit>().Team = Team.Team1;
        GameObject spearMan1 = Instantiate(spearManPrefab, Grid.instance.NodeObjs[0, 4].transform.position, team1Rot);
        spearMan1.GetComponent<Unit>().Team = Team.Team1;
        GameObject archer1 = Instantiate(archerPrefab, Grid.instance.NodeObjs[0, 5].transform.position, team1Rot);
        archer1.GetComponent<Unit>().Team = Team.Team1;

        Quaternion team2Rot = Quaternion.Euler(0, 270, 0);
        Texture team2Tex = Resources.Load("Textures/WK_StandardUnits_Red") as Texture;

        
        GameObject swordMan2 = Instantiate(swordManPrefab, Grid.instance.NodeObjs[9, 3].transform.position, team2Rot);
        swordMan2.GetComponent<Unit>().Team = Team.Team2;
        swordMan2.GetComponentInChildren<Renderer>().material.mainTexture = team2Tex;
        GameObject spearMan2 = Instantiate(spearManPrefab, Grid.instance.NodeObjs[9, 4].transform.position, team2Rot);
        spearMan2.GetComponent<Unit>().Team = Team.Team2;
        spearMan2.GetComponentInChildren<Renderer>().material.mainTexture = team2Tex;
        GameObject archer2 = Instantiate(archerPrefab, Grid.instance.NodeObjs[9, 5].transform.position, team2Rot);
        archer2.GetComponent<Unit>().Team = Team.Team2;
        archer2.GetComponentInChildren<Renderer>().material.mainTexture = team2Tex;

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
                if (!toSelectAttackee && hitUnit.Status == UnitStatus.Ready)
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
        Grid.instance.clear();
        GameManager.selectedUnit.setStatus(UnitStatus.Idle);
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

        //float resolutionRatio = (float)Screen.width / refWidth;
        //menuPos.x = menuPos.x / resolutionRatio + 50;
        //menuPos.y = menuPos.y / resolutionRatio + 50;

        menuPos.x = menuPos.x + 50;
        menuPos.y = menuPos.y + 50;

        BattleMenu.instance.gameObject.SetActive(true);
        RectTransform battleMenuRectTrans = BattleMenu.instance.gameObject.GetComponent<RectTransform>();
        battleMenuRectTrans.anchoredPosition = menuPos;

        BattleMenu.instance.AttackBtnObj.SetActive(AttackableNodeObjs != null && AttackableNodeObjs.Count > 0);
        BattleMenu.instance.MoveBtnObj.SetActive(!moved);
    }

}
