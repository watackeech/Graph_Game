using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalChecker : MonoBehaviour
{
    public GameObject hoopObject;
    public GameObject preGoalCheckerObject;
    private PreGoalChecker preGoalCheckerScript;
    private Animator hoopAnimator;
    // private bool isGoal;
    // Start is called before the first frame update
    void Start()
    {
        hoopAnimator = hoopObject.GetComponent<Animator>();
        preGoalCheckerScript = preGoalCheckerObject.GetComponent<PreGoalChecker>();
    }

    void OnTriggerEnter2D( Collider2D col ){
        if((col.gameObject.tag == "Ball" || col.gameObject.tag == "Character") && preGoalCheckerScript.isPreGoaled){
            // isGoal = true;
            hoopAnimator.SetBool("isHoop", true);
            preGoalCheckerScript.isPreGoaled = false;
            Debug.Log("ごーーーる！！");
            // hoopAnimator.SetBool("isHoop", false);
            Invoke("EndGoalAnim", 1);
        }
    }
    void EndGoalAnim(){
        hoopAnimator.SetBool("isHoop", false);
    }
}
