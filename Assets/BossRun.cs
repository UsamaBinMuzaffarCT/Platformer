using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BossRun : StateMachineBehaviour
{
    public float speed = 2.5f;
    public float attackRange = 3f;
    public bool extraAttack = false;

    List<GameObject> players;
    Rigidbody2D rb;

    BossScriptable extras;

    private GameObject ClosestPlayer(Animator animator)
    {
        float minDistance = float.PositiveInfinity;
        GameObject minDistancePlayer = null;
        foreach (GameObject player in players)
        {
            float thisDistance = Vector2.Distance(player.transform.position, animator.GetComponent<Transform>().parent.transform.position);
            if (thisDistance < minDistance)
            {
                minDistance = thisDistance;
                minDistancePlayer = player;
            }            
        }
        return minDistancePlayer;
    }

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        players = GameObject.FindGameObjectsWithTag("Player").ToList();
        rb = animator.transform.parent.GetComponent<Rigidbody2D>();
        extras = Resources.Load<BossScriptable>("ScriptableObjects/BossExtrasScriptable");
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject player = ClosestPlayer(animator);
        Vector2 target = new Vector2(player.transform.position.x, animator.transform.parent.position.y);
        rb.MovePosition(Vector2.MoveTowards(animator.transform.parent.position, target, speed * Time.fixedDeltaTime));
        rb.GetComponent<Boss>().LookAtPlayer();
        if(Vector2.Distance(player.transform.position, rb.position) <= attackRange)
        {
            animator.SetTrigger("Attack");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger("Attack");
    }
}
