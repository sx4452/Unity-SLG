using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using GridSystem;
using System;

[RequireComponent(typeof(Rigidbody), typeof(Animator))]
public class Unit : MonoBehaviour {
    public int speed = 2;
    public int attackRange = 1;
    public int attackPower = 10;
    public int defence = 5;
    public int hp = 20;

    private Team team;
    public Team Team
    {
        get { return team; }
        set { team = value; }
    }
    private UnitStatus status;
    public UnitStatus Status
    {
        get { return status; }
    }

    private Renderer renderer1;
    private Rigidbody rigBody;
    private Animator animator;

    private GameObject healthBarObj;

    void Start () {

        rigBody = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        renderer1 = GetComponentInChildren<Renderer>();
        GameObject nodeObj = Grid.instance.getNodeObjFromPosition(transform.position);
        Grid.instance.setNodeStatus(nodeObj, NodeStatus.Occupied);
        setStatus(UnitStatus.Ready);
        initHealthBar();
	}

    void Update()
    {
        Vector3 healBarPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);
        healthBarObj.transform.position = healBarPos;
    }

    private void initHealthBar()
    {
        healthBarObj = Instantiate(GameManager.healthBarPrefab);
        Transform healthBar = healthBarObj.transform;
        healthBar.parent = GameManager.CanvasObj.transform;
        RectTransform healthBarRectTrans = healthBar.GetComponent<RectTransform>();
        healthBarRectTrans.localScale = Vector3.one;
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
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1)
        {
            yield return null;
        }
        callback();
    }

    public void setStatus(UnitStatus newStatus)
    {
        switch (newStatus)
        {
            case UnitStatus.Idle:
                status = UnitStatus.Idle;
                renderer1.material.color = Color.red;
                break;
            case UnitStatus.Ready:
                status = UnitStatus.Ready;
                renderer1.material.color = Color.white;
                break;

        }
    }
}

public enum UnitStatus
{
    Idle,
    Ready
}
