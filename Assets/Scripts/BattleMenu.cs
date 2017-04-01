using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GridSystem;

public class BattleMenu : MonoBehaviour {

    public static BattleMenu instance;

    private GameObject attackBtnObj;
    private GameObject moveBtnObj;
    private GameObject idleBtnObj;

    public GameObject AttackBtnObj
    {
        get { return attackBtnObj; }
    }
    public GameObject MoveBtnObj
    {
        get { return moveBtnObj; }
    }
    public GameObject IdleBtnObj
    {
        get { return idleBtnObj; }
    }

    void Start()
    {
        // if the singleton hasn't been initialized yet
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        instance = this;
        gameObject.SetActive(false);


        attackBtnObj = transform.GetChild(0).gameObject;
        moveBtnObj = transform.GetChild(1).gameObject;
        idleBtnObj = transform.GetChild(2).gameObject;

        Button attackBtn = attackBtnObj.GetComponent<Button>();
        attackBtn.onClick.AddListener(onAttackBtnClick);

        Button moveBtn = moveBtnObj.GetComponent<Button>();
        moveBtn.onClick.AddListener(onMoveBtnClick);


        //Debug.Log(instance.gameObject.name);
        //DontDestroyOnLoad(gameObject);
    }

    private void onMoveBtnClick()
    {
        Grid.instance.hightLightUnitMovable();
    }

    private void onAttackBtnClick()
    {
        GameManager.toSelectAttackee = true;
        gameObject.SetActive(false);
    }

    private void onIdleBtnClick()
    {

    }
}
