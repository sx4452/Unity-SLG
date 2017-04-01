using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GridSystem;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Unit : MonoBehaviour {

    public int speed = 2;
    public int attackRange = 1;


    private Rigidbody rigBody;
    private Animator animator;

    private Team team;

    public Team Team
    {
        get { return team; }
        set { team = value; }
    }

	void Start () {

        rigBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        GameObject nodeObj = Grid.instance.getNodeObjFromPosition(transform.position);
        Grid.instance.setNodeStatus(nodeObj, NodeStatus.Occupied);
	}

    public IEnumerator move(List<GameObject> path)
    {
        playRunAnim();
        for (int i = 0; i < path.Count - 1; i++ )
        {
            Vector3 startPos = path[i].transform.position;
            Vector3 endPos = path[i+1].transform.position;

            GameObject nodeObj = Grid.instance.getNodeObjFromPosition(startPos);
            Grid.instance.setNodeStatus(nodeObj, NodeStatus.Normal);

            Vector3 newDirection = (endPos - startPos).normalized;
            newDirection.y = 0;
            transform.forward = newDirection;

            float dist = Vector3.Distance(startPos, endPos);
            float distTraveled = 0;
            endPos.y = startPos.y = rigBody.position.y;
            while (!rigBody.position.Equals(endPos))
            {
                distTraveled += GameManager.unitMoveSpeed * Time.deltaTime;
                rigBody.position = Vector3.Lerp(startPos, endPos, distTraveled / dist);
                yield return null;
            }
            nodeObj = Grid.instance.getNodeObjFromPosition(endPos);
            Grid.instance.setNodeStatus(nodeObj, NodeStatus.Occupied);
        }
        playIdleAnim();
        GameManager.instance.showBattleMenu(true);
    }
    

    public void attack(Action callback)
    {
        StartCoroutine(playAttackAnim(callback));
    }

    private  void playRunAnim()
    {
        animator.SetBool("run",true);
    }

    private void playIdleAnim()
    {
        animator.SetBool("run", false);
    }

    private IEnumerator playAttackAnim(Action callback)
    {
        animator.SetTrigger("attack");
        yield return null;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.95)
        {
            yield return null;
        }
        callback();
    }

}
