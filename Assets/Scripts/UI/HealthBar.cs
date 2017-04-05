using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        // Use this for initialization
        private Slider hpBar;
        private Text hpText;
        void Awake()
        {
            hpText = GetComponentInChildren<Text>();
            hpBar = GetComponentInChildren<Slider>();

            //自动添加到canvas下面，scale设为1
            transform.parent = GameManager.CanvasObj.transform;
            RectTransform healthBarRectTrans = GetComponent<RectTransform>();
            healthBarRectTrans.localScale = Vector3.one;
        }

        public void setHpMax(int value)
        {
            hpBar.maxValue = value;
        }

        public void setHp(int value)
        {
            hpText.text = value.ToString();
            hpBar.value = value;
        }
    }

}
