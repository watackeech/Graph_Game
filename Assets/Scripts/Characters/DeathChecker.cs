using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathChecker : MonoBehaviour
{
    // public GameObject hitCheckerObject;
    // HitChecker groundChecker;
    public bool isLiving = true;
    public Rigidbody2D rb;

    // void Start()
    // {
    //     groundChecker = hitCheckerObject.gameObject.GetComponent<HitChecker>();
    // }

    private void Start(){

    }


    void OnTriggerEnter2D( Collider2D col ){
        if(col.gameObject.tag == "DeadArea" || col.gameObject.tag == "Metaball_liquid"){
            Debug.Log("あちゃあ");
            isLiving = false;
            // Invoke("ToggleDisable", 1);

            // CharacterRBToggle();
            // groundChecker.StopMove();
        }
    }
    // private void ToggleDisable(){
    //     StartSimBtnScript.SetIsOnWithoutCallback(false);
    // }

    // public void CharacterRBToggle(){
    //     rb.simulated = !rb.simulated;
    // }

    // void OnTriggerExit2D( Collider2D col ){
    //     if(col.gameObject.tag == "DeadArea" || col.gameObject.tag == "Metaball_liquid"){
    //         isLiving = true;
    //         Debug.Log("ふっかっつつ！");
    //     }
    // }

}
