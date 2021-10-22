using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitChecker : MonoBehaviour
{
    public bool canJump = false;
    public bool moveRight = false;
    public bool isRunning = false;
    // public bool isLiving = true;
    public bool isGoal = false;
    // public bool isFront = false;

    // public bool isLand = false;
    // Start is called before the first frame update
    // void Start()
    // {
    // }

    void OnTriggerEnter2D( Collider2D col ){
        // if(col.gameObject.tag == "DeadArea" || col.gameObject.tag == "Metaball_liquid"){
        //     Debug.Log("ああああああああああ");
        //     isLiving = false;
        //     StopMove();
        // }else
        if(col.gameObject.tag == "JumpGround"){ //ジャンプ板か完成されたグラフの上
        // if(isFront){
            Debug.Log("とべる！！！！！！！");
            Jump();
        }else if(col.gameObject.tag == "NormalGround" || col.gameObject.tag == "onSimulate" || col.gameObject.tag == "Ball"){
            // Debug.Log("地面についた");
            Run();
            // Debug.Log("あるけるよ");
        }else if(col.gameObject.tag == "GoalFlag" || col.gameObject.tag == "BasketHoop"){
            isGoal = true;
            StopMove();
        }else{
            moveRight = true;
            // Debug.Log("例外");
            // Debug.Log($"例外：{col.gameObject.tag}にぶつかった！！");
        }
    }
    void OnTriggerExit2D( Collider2D col ){
        // isLand = false;
        // if(col.gameObject.tag == "NormalGround"){ //グラフはColliderが細かく、頻繁にExitしてしまうので条件から外す
        //     Debug.Log("地面を離れた2");
        //     // isRunning = false;
        // }
        if(col.gameObject.tag == "JumpGround"){
            canJump = false;
            // Debug.Log("とんだ！");
        }
        // else if(col.gameObject.tag == "DeadArea"){
        //     isLiving = true;
        //     Debug.Log("復活！");
        // }else{
        //     // canJump = true;
        //     // Debug.Log("例外");
        // }
    }

    public void StopMove(){
        moveRight = false;
        canJump = false;
        isRunning = false;
    }


    void Jump(){
        moveRight = true;
        canJump = true;
        isRunning = false;
    }
    void Run(){
        moveRight = true;
        canJump = false;
        isRunning = true;
    }
    // public void DisableGoal(){
    //     isGoal = false;
    // }
}
