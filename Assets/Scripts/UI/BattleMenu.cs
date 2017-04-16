using UnityEngine;
using UnityEngine.UI;
using GridSystem;
using Character;
using UnityEngine.EventSystems;

namespace UI
{
    public class BattleMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    { 
        public static BattleMenu instance;
        public static bool isMouseover;

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
            isMouseover = false;
        }

        private void onIdleBtnClick()
        {
            gameObject.SetActive(false);
            GameManager.selectedUnit.setStatus(UnitStatus.Idle);
            Grid.instance.clear();
            isMouseover = false;
        }

        private void onCancelBtnClick()
        {
            gameObject.SetActive(false);
            GameManager.selectedUnit.cancelMove();
            Grid.instance.clear();
            isMouseover = false;
        }

        public void showCancelOnly()
        {
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
            isMouseover = false;
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
            isMouseover = false;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isMouseover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isMouseover = false;
        }
    }
}
