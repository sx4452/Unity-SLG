using UnityEngine;
using GridSystem;
using System.Collections.Generic;
using Character;
using UI;

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

    public static SelectionTarget selectionTarget;//用于指示下一个点击选啥

    public static GameObject healthBarContainerObj;
    public static GameObject healthBarPrefab;


    private Team currentTurnTeam;//当前行动的阵营
    private List<Unit> currentNotPlayedUnits;
    private List<Unit> team1Units;
    private List<Unit> team2Units;

    private int turnCount;
    private int NodeLayer = 8;
    private int UnitLayer = 9;

	void Start()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        GameInput.OnClick += OnClick;

        initUnits();

        healthBarContainerObj = GameObject.FindGameObjectWithTag("HealthBarContainer") as GameObject;
        healthBarPrefab = Resources.Load("Prefabs/UI/HealthBar") as GameObject;

        currentTurnTeam = Team.Team1;
        currentNotPlayedUnits = new List<Unit>(team1Units);
        turnCount = 1;
        TurnIdicator.instance.showTurn("Turn " + turnCount, Color.blue);

        selectionTarget = SelectionTarget.Unit;
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
        team1Units = new List<Unit>();
        addUnit(swordManPrefab, Team.Team1, Grid.instance.NodeObjs[0, 3].transform.position,team1Rot,team1Units);
        addUnit(spearManPrefab, Team.Team1, Grid.instance.NodeObjs[0, 4].transform.position, team1Rot, team1Units);
        addUnit(archerPrefab, Team.Team1, Grid.instance.NodeObjs[0, 5].transform.position, team1Rot, team1Units);

        Quaternion team2Rot = Quaternion.Euler(0, 270, 0);
        Texture team2Tex = Resources.Load("Textures/WK_StandardUnits_Red") as Texture;
        team2Units = new List<Unit>();
        addUnit(swordManPrefab, Team.Team2, Grid.instance.NodeObjs[2, 3].transform.position, team2Rot, team2Units,team2Tex);
        addUnit(spearManPrefab, Team.Team2, Grid.instance.NodeObjs[2, 4].transform.position, team2Rot, team2Units, team2Tex);
        addUnit(archerPrefab, Team.Team2, Grid.instance.NodeObjs[2, 5].transform.position, team2Rot, team2Units, team2Tex);

        Unit.OnAttackComplete = onUnitAttackComplete;
        Unit.OnUnitIdle = onUnitIdle;
    }

    private void addUnit(GameObject unitPrefab, Team team,Vector3 pos, Quaternion rot, List<Unit> teamUnits, Texture texture = null)
    {
        GameObject unitObj = Instantiate(unitPrefab, pos, rot);
        Unit unit = unitObj.GetComponent<Unit>();
        unit.Team = team;
        if (texture != null)
            unitObj.GetComponentInChildren<Renderer>().material.mainTexture = texture;
        teamUnits.Add(unit);
    }

    void OnClick(Vector2 clickPos)
    {
        switch(selectionTarget)
        {
            case SelectionTarget.Unit:
                Unit hitUnit = getHitObject<Unit>(clickPos, 1<<UnitLayer);
                if (hitUnit !=null && hitUnit.Status == UnitStatus.Ready && hitUnit.Team == currentTurnTeam)
                {
                    setSelectedUnit(hitUnit);
                    showBattleMenu(false);
                }
                break;
            case SelectionTarget.Attackee:
                hitUnit = getHitObject<Unit>(clickPos, 1 << UnitLayer);
                if (hitUnit != null)
                {
                    Node hitNode = Grid.instance.getNodeObjFromPosition(hitUnit.transform.position).GetComponent<Node>();
                    if (hitNode.Status == NodeStatus.Attackable)
                    {
                        Vector3 faceDir = (hitUnit.transform.position - selectedUnit.transform.position).normalized;
                        selectedUnit.transform.forward = faceDir;
                        selectedUnit.attack(hitUnit);
                    }
                }
                break;
            case SelectionTarget.Node:
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(clickPos), out hit, 1000))
                {
                    if (hit.collider.gameObject.layer == NodeLayer)
                    {
                        GameObject hitNodeObj = Grid.instance.getNodeObjFromPosition(hit.point);

                        if (selectedUnitNodeObj != null && hitNodeObj != selectedUnitNodeObj)
                        {
                            Node hitNode = hitNodeObj.GetComponent<Node>();
                            if (hitNode.Status == NodeStatus.Movable)
                            {
                                List<GameObject> path = Grid.instance.getShortestPath(selectedUnitNodeObj, hitNodeObj);
                                StartCoroutine(GameManager.selectedUnit.move(path));
                                Grid.instance.clear();
                            }
                        }
                    }
                }
                break;
        }
    }

    private T getHitObject<T>(Vector2 clickPos, LayerMask layermask)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, layermask))
        {
            GameObject hitGo = hit.collider.gameObject;
            return hitGo.GetComponent<T>();
        }
        return default(T);
    }
    private  void onUnitAttackComplete()
    {
        selectionTarget = SelectionTarget.Unit;
        Grid.instance.clear();
        selectedUnit.setStatus(UnitStatus.Idle);
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

    private void onUnitIdle()
    {
        currentNotPlayedUnits.Remove(selectedUnit);
        if (currentNotPlayedUnits.Count == 0)
            switchTeam();
    }

    private void switchTeam()
    {
        switch(currentTurnTeam)
        {
            case Team.Team1:
                resetTeamStatus(team1Units);
                currentTurnTeam = Team.Team2;
                currentNotPlayedUnits = new List<Unit>(team2Units);
                TurnIdicator.instance.showTurn("Enermy's Turn", Color.red);
                break;
            case Team.Team2:
                resetTeamStatus(team2Units);
                currentTurnTeam = Team.Team1;
                currentNotPlayedUnits = new List<Unit>(team1Units);
                TurnIdicator.instance.showTurn("Turn " + ++turnCount, Color.blue);
                break;
        }
    }

    private void resetTeamStatus(List<Unit> teamUnits)
    {
        foreach(Unit unit in teamUnits)
        {
            unit.setStatus(UnitStatus.Ready);
        }
    }

}

public enum SelectionTarget
{
    Unit,
    Attackee,
    Node
}

