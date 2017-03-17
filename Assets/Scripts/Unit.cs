using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GridSystem;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Unit : MonoBehaviour {

    public int speed = 2;

    private LayerMask unitLayer = 1 << 9;
    private Rigidbody rigBody;
    private Animator animator;
	void Start () {

        GameInput.OnMouseLeftClick += OnMouseLeftClick;
        rigBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
	}

    void OnMouseLeftClick(Vector2 clickPos)
    {
        Ray ray = Camera.main.ScreenPointToRay(clickPos);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, 1000, unitLayer))
        {
            GameObject hitGo = hit.collider.gameObject;
            if(hitGo.Equals(gameObject))
            {
                Grid.instance.hightLightUnitMovable(transform.position, speed);
                StartCoroutine(move(Grid.instance.getPath()));
            }
        }
        else
        {
            Grid.instance.clear();
        }
	}

    public IEnumerator move(List<GameObject> path)
    {
        playRunAnim();
        for (int i = 0; i < path.Count - 1; i++ )
        {
            Vector3 startPos = path[i].transform.position;
            Vector3 endPos = path[i+1].transform.position;
            float dist = Vector3.Distance(startPos, endPos);
            float distTraveled = 0;
            endPos.y = startPos.y = rigBody.position.y;
            while (!rigBody.position.Equals(endPos))
            {
                distTraveled += GameInput.unitMoveSpeed * Time.deltaTime;
                rigBody.position = Vector3.Lerp(startPos, endPos, distTraveled / dist);
                yield return null;
            }
        }
        playIdleAnim();
    }

    private  void playRunAnim()
    {
        animator.SetBool("run",true);
    }

    private void playIdleAnim()
    {
        animator.SetBool("run", false);
    }

}
