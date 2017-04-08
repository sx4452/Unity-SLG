using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class TurnIdicator : MonoBehaviour
    {
        public static TurnIdicator instance;

        public float acceleration = 500f;
        // Use this for initialization
        private Transform textTrans;
        void Start()
        {

            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            textTrans = transform.GetChild(0);
            gameObject.SetActive(false);
        }

        public void showTurn(string msg, Color color)
        {
            Text turnText = textTrans.GetComponent<Text>();
            turnText.color = color;
            turnText.text = msg;

            gameObject.SetActive(true);
            StartCoroutine(show());
        }

        private IEnumerator show()
        {
            float x = 0;

            RectTransform textRectTrans = textTrans.GetComponent<RectTransform>();

            textRectTrans.anchoredPosition = new Vector2(0, 0);

            float minSpeed = Screen.width / 20;
            float maxSpeed = Screen.width;
            float speed = maxSpeed;
            x = 0;
            while (x != Screen.width / 2)
            {
                speed = Mathf.Max(speed - acceleration * Time.deltaTime, minSpeed);
                x = Mathf.MoveTowards(x, Screen.width / 2, speed * Time.deltaTime);
                textRectTrans.anchoredPosition = new Vector2(x, 0);
                yield return null;
            }
            yield return new WaitForSeconds(1f);
            while (x != Screen.width)
            {
                speed = Mathf.Min(speed + acceleration * Time.deltaTime, maxSpeed);
                x = Mathf.MoveTowards(x, Screen.width, speed * Time.deltaTime);
                textRectTrans.anchoredPosition = new Vector2(x, 0);
                yield return null;
            }

            gameObject.SetActive(false);
        }

    }
    
}
