using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGoalChecker : MonoBehaviour
{
    public bool isPreGoaled = false;

    private void OnTriggerEnter2D( Collider2D col ){
        if(col.gameObject.tag == "Ball" || col.gameObject.tag == "Character"){
            isPreGoaled = true;
            Debug.Log("ぷれゴール！！");
            // hoopAnimator.SetBool("isHoop", false);
            Invoke("DisablePreGoaled", 3);
        }
    }
    private void DisablePreGoaled(){
        isPreGoaled = false;
    }
}
