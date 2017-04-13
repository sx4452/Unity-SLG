using UnityEngine;
using UnityEngine.UI;
using GridSystem;
using Character;

namespace UI
{
    public class BattleMenu : MonoBehaviour
    {
        public static BattleMenu instance;

        
        private GameObject idleBtnObj;
        private GameObject cancelObj;
        private Vector3 returnPos;
        private Quaternion returnRot;

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

            idleBtnObj = transform.GetChild(0).gameObject;
            cancelObj = transform.GetChild(1).gameObject;

            Button idleBtn = idleBtnObj.GetComponent<Button>();
            idleBtn.onClick.AddListener(onIdleBtnClick);

            Button cancelBtn = cancelObj.GetComponent<Button>();
            cancelBtn.onClick.AddListener(onCancelBtnClick);
        }

        private void onIdleBtnClick()
        {
            gameObject.SetActive(false);
            GameManager.selectedUnit.setStatus(UnitStatus.Idle);
        }

        private void onCancelBtnClick()
        {
            gameObject.SetActive(false);

                GameObject nodeObj = Grid.instance.getNodeObjFromPosition(GameManager.selectedUnit.transform.position);
                Grid.instance.setNodeStatus(nodeObj, NodeStatus.Normal);
                nodeObj = Grid.instance.getNodeObjFromPosition(returnPos);
                Grid.instance.setNodeStatus(nodeObj, NodeStatus.Occupied);

                GameManager.selectedUnit.transform.position = returnPos;
                GameManager.selectedUnit.transform.rotation = returnRot;
                Grid.instance.clear();
        }

        public void showCancelOnly()
        {
            //attackBtnObj.SetActive(false);
            //moveBtnObj.SetActive(false);
            IdleBtnObj.SetActive(false);
            RectTransform rectTrans = GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(50,50);
        }

        public void show()
        {
            gameObject.SetActive(true);
            RectTransform rectTrans = GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(50, 50);
        }
    }
}
