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

            RectTransform rectTrans = GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(50, 50);
        }

        private void onIdleBtnClick()
        {
            gameObject.SetActive(false);
            GameManager.selectedUnit.setStatus(UnitStatus.Idle);
            Grid.instance.clear();
        }

        private void onCancelBtnClick()
        {
            gameObject.SetActive(false);
            GameManager.selectedUnit.cancelMove();
            Grid.instance.clear();
        }

        public void showCancelOnly()
        {
            //attackBtnObj.SetActive(false);
            //moveBtnObj.SetActive(false);
            IdleBtnObj.SetActive(false);
            RectTransform rectTrans = GetComponent<RectTransform>();
            rectTrans.anchoredPosition = new Vector2(50, 50);
        }

        public void show()
        {
            gameObject.SetActive(true);

        }

        public void hide()
        {
            gameObject.SetActive(false);
        }

        public void showIdle()
        {
            gameObject.SetActive(true);
            CancelBtnObj.SetActive(false);
        }

        public void hideIdle()
        {
            CancelBtnObj.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
