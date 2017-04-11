using UnityEngine;
using UnityEngine.UI;
using GridSystem;
using Character;

namespace UI
{
    public class BattleMenu : MonoBehaviour
    {
        public static BattleMenu instance;

        private GameObject attackBtnObj;
        private GameObject moveBtnObj;
        private GameObject idleBtnObj;
        private GameObject cancelObj;
        private Vector3 returnPos;
        private Quaternion returnRot;
        private Command lastCommand;//记录上次点击的按钮
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
        public GameObject CancelBtnObj
        {
            get { return cancelObj; }
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
            cancelObj = transform.GetChild(3).gameObject;

            Button attackBtn = attackBtnObj.GetComponent<Button>();
            attackBtn.onClick.AddListener(onAttackBtnClick);

            Button moveBtn = moveBtnObj.GetComponent<Button>();
            moveBtn.onClick.AddListener(onMoveBtnClick);

            Button idleBtn = idleBtnObj.GetComponent<Button>();
            idleBtn.onClick.AddListener(onIdleBtnClick);

            Button cancelBtn = cancelObj.GetComponent<Button>();
            cancelBtn.onClick.AddListener(onCancelBtnClick);

            lastCommand = Command.Null;

        }

        private void onMoveBtnClick()
        {
            Grid.instance.hightLightUnitMovable();
            returnPos = GameManager.selectedUnit.transform.position;
            returnRot = GameManager.selectedUnit.transform.rotation;
            GameManager.selectionTarget = SelectionTarget.Node;
            showCancelOnly();
            lastCommand = Command.Move;
        }

        private void onAttackBtnClick()
        {
            gameObject.SetActive(false);
            setNodeStatus(nodeObj, NodeStatus.Attackable);


            GameManager.selectionTarget = SelectionTarget.Attackee;
            lastCommand = Command.Attack;
        }

        private void onIdleBtnClick()
        {
            gameObject.SetActive(false);
            GameManager.selectedUnit.setStatus(UnitStatus.Idle);
            GameManager.selectionTarget = SelectionTarget.Unit;
            lastCommand = Command.Idle;
        }

        private void onCancelBtnClick()
        {
            gameObject.SetActive(false);

            if(lastCommand == Command.Move)
            {
                GameObject nodeObj = Grid.instance.getNodeObjFromPosition(GameManager.selectedUnit.transform.position);
                Grid.instance.setNodeStatus(nodeObj, NodeStatus.Normal);
                nodeObj = Grid.instance.getNodeObjFromPosition(returnPos);
                Grid.instance.setNodeStatus(nodeObj, NodeStatus.Occupied);

                GameManager.selectedUnit.transform.position = returnPos;
                GameManager.selectedUnit.transform.rotation = returnRot;
                Grid.instance.clear();
            }
            GameManager.selectionTarget = SelectionTarget.Unit;
            lastCommand = Command.Cancel;
        }

        public void showCancelOnly()
        {
            attackBtnObj.SetActive(false);
            moveBtnObj.SetActive(false);
            IdleBtnObj.SetActive(false);

            RectTransform rectTrans = GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(50,50);
        }

        private enum Command
        {
            Attack,
            Move,
            Idle,
            Cancel,
            Null
        }

    }



}
