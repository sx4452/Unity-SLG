using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GridSystem;

public class Unit : MonoBehaviour {

    public int speed = 2;

    private LayerMask unitLayer = 1 << 9;
    private Rigidbody rigBody;
	void Start () {

        GameInput.OnMouseLeftClick += OnMouseLeftClick;
        rigBody = GetComponent<Rigidbody>();
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
                StartCoroutine(move(Grid.instance.path));
            }
        }
        else
        {
            Grid.instance.clear();
        }
	}

    public IEnumerator move(List<GameObject> path)
    {
        foreach(GameObject node in path)
        {
            Vector3 startPos = rigBody.position;
            Vector3 target = node.transform.position;
            target.y = startPos.y;
            float timePassed = 0;
            while (!rigBody.position.Equals(target))
            {
                timePassed += Time.deltaTime;
                rigBody.MovePosition(Vector3.Lerp(startPos, target, timePassed / GameInput.unitMoveSpeed));
                yield return null;
            }
        }
    }
}
