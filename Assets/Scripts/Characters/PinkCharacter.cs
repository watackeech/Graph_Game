using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Timers;

public class PinkCharacter : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject GroundCheckerObject;
    public Animator animator;
    public GameObject deathCheckerObject;
    public StartSimulationButton StartSimBtnScript;
    // public bool isWaterStage = false;

    private DeathChecker deathCheckerScript;
    private GraphDrawer graphDrawerScript;
    private CircleDrawer circleDrawerScript;
    // private float speed = 1500f;
    private float jumpingPower = 6000f;
    private HitChecker groundChecker;
    public bool isSimulating;
    // private bool isLivingPink;
    private bool hasGoaled = false;
    private SceneViewCamera CameraScript;
    private Water2D.Water2D_Spawner watarSpawnerScript;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundChecker = GroundCheckerObject.gameObject.GetComponent<HitChecker>();
        deathCheckerScript = deathCheckerObject.gameObject.GetComponent<DeathChecker>();
        StartSimBtnScript = GameObject.Find("StartSimulationButton").GetComponent<StartSimulationButton>();
        CameraScript = GameObject.Find("2-DefaultCamera").GetComponent<SceneViewCamera>();
        graphDrawerScript  = GameObject.Find("GraphDrawer").GetComponent<GraphDrawer>();
        circleDrawerScript = GameObject.Find("CircleDrawer").GetComponent<CircleDrawer>();
        if(GameObject.Find("Water2D_Spawner")){
            watarSpawnerScript = GameObject.Find("Water2D_Spawner").GetComponent<Water2D.Water2D_Spawner>();
        }


        // animator.SetBool("isStand", true);
        // Debug.Log(this.gameObject.transform.position.x);
    }
    void Update(){
        // groundChecker.OnTriggerEnter2D();
        if(deathCheckerScript.isLiving){
            if(isSimulating && !hasGoaled){
                if(groundChecker.moveRight){ //右に動く間
                    if(rb.velocity.x < 3f){ //速度の上限設定
                        //deltaTime(一つ前のフレーム⇒今のフレームまでの時間)を掛けることでfpsによる差をなくす

                        if(groundChecker.isRunning){
                            if(rb.velocity.y < -1f){
                                // Debug.Log("1");
                                LandAnim();
                                // Debug.Log("着地アニメ！");
                            }else if(rb.velocity.y < 2f && rb.velocity.y >= 0.1f && rb.velocity.x < 2){//坂を上っている間(地面に設置している&&上へ移動中)
                                Debug.Log("さかのぼり中");
                                WalkAnim();
                                rb.AddForce(Vector2.up * 450f * Time.deltaTime);
                                rb.AddForce(Vector2.right * 700f * Time.deltaTime);
                                // Debug.Log("2");
                            }else if(rb.velocity.y < 0.1){
                                rb.AddForce(Vector2.right * 800f * Time.deltaTime);
                                rb.AddForce(Vector2.up * 500f * Time.deltaTime);
                                if(rb.velocity.x > 1){
                                    WalkAnim();
                                }
                                // else if(rb.velocity.x < 0.5 && this.gameObject.transform.position.x < -4){ //前に進めていないとき
                                //     WalkAnim();
                                //     rb.AddForce(Vector2.up * 2000f * Time.deltaTime); //ここどうしようか？
                                // }
                                // Debug.Log("3");
                            }else{
                                // Debug.Log("4");
                                rb.AddForce(Vector2.right * 800f * Time.deltaTime);
                                WalkAnim();
                            }
                        }else{
                            Debug.Log("あるいてないぞ");
                            if(groundChecker.canJump){
                                rb.AddForce(Vector2.up * 6000 * Time.deltaTime); //ジャンプ
                                // rb.AddForce(Vector2.right * 1500f * Time.deltaTime);
                            }else if(rb.velocity.y >= 0){
                                JumpAnim();
                                // Debug.Log("ジャンプ中！");
                            }else if(rb.velocity.y < 0){
                                FallAnim();
                                // Debug.Log("落下中！");
                            }
                        }
                    }
                }else if(groundChecker.isGoal){
                    GoalAnim();
                    hasGoaled = true;
                    // isSimulating = false;
                    // Debug.Log("ゴーーーーーール！！");
                }else if(rb.velocity.y < -0.5f){
                    FallAnim();
                }
            }
        }else if(hasGoaled){
            GoalAnim();
        }else{ // !isSimulatingの場合
            //復活するまでの１秒間、Toggleが反応しないようにしておきたい
            StartSimBtnScript.SetIsOnWithoutCallback(false);
            StartSimBtnScript.toggle.enabled = false;
            CameraScript.isMovingEnabler();
            Rebirth();
            Invoke("EnableToggle", 1);
        }
    }

    private void EnableToggle(){
        StartSimBtnScript.toggle.enabled = true;
    }
    public void StartSimulation(){
        if(!isSimulating){
            rb.simulated = true;
            isSimulating = true;
        }else{
            CameraScript.isMovingEnabler();
            isSimulating = false;
            Reset();
        }
    }

    public void Rebirth(){
        rb.simulated = false;
        isSimulating = false;
        groundChecker.isGoal = false;
        DamagedAnim();
        if(GameObject.Find("Water2D_Spawner")){
            watarSpawnerScript.StopWater();
        }
        Invoke("Reset", 1); //Reset()を1秒後に呼び出し
    }

    public void Reset(){
        rb.simulated = false;
        isSimulating = false;
        groundChecker.isGoal = false;
        transform.position = new Vector3(-7f, 2f, 0f); //初期位置に戻す
        rb.velocity = Vector3.zero; //velocityリセット
        rb.AddForce(Vector2.up * jumpingPower * Time.deltaTime); //復活後ちょっとホップ
        groundChecker.StopMove();
        deathCheckerScript.isLiving = true;
        // deathCheckerScript.CharacterRBToggle();
        hasGoaled = false;
        StandAnim();
        graphDrawerScript.DeleteAllGraphs();
        circleDrawerScript.DeleteAllCircles();
    }


    void StandAnim(){
        animator.SetBool("isStand", true);
        animator.SetBool("isWalk", false);
        animator.SetBool("isJump", false);
        animator.SetBool("isFall", false);
        animator.SetBool("isLand", false);
        animator.SetBool("isGoal", false);
        animator.SetBool("isDamaged", false);
    }
    void WalkAnim(){
        animator.SetBool("isStand", false);
        animator.SetBool("isWalk", true);
        animator.SetBool("isJump", false);
        animator.SetBool("isFall", false);
        animator.SetBool("isLand", false);
        animator.SetBool("isGoal", false);
        animator.SetBool("isDamaged", false);
    }
    void JumpAnim(){
        animator.SetBool("isStand", false);
        animator.SetBool("isWalk", false);
        animator.SetBool("isJump", true);
        animator.SetBool("isFall", false);
        animator.SetBool("isLand", false);
        animator.SetBool("isGoal", false);
        animator.SetBool("isDamaged", false);
    }
    void FallAnim(){
        animator.SetBool("isStand", false);
        animator.SetBool("isWalk", false);
        animator.SetBool("isJump", false);
        animator.SetBool("isFall", true);
        animator.SetBool("isLand", false);
        animator.SetBool("isGoal", false);
        animator.SetBool("isDamaged", false);
    }
    void LandAnim(){
        animator.SetBool("isStand", false);
        animator.SetBool("isWalk", false);
        animator.SetBool("isJump", false);
        animator.SetBool("isFall", false);
        animator.SetBool("isLand", true);
        animator.SetBool("isGoal", false);
        animator.SetBool("isDamaged", false);
    }
    void GoalAnim(){
        animator.SetBool("isStand", false);
        animator.SetBool("isWalk", false);
        animator.SetBool("isJump", false);
        animator.SetBool("isFall", false);
        animator.SetBool("isLand", false);
        animator.SetBool("isGoal", true);
        animator.SetBool("isDamaged", false);
    }
    void DamagedAnim(){
        animator.SetBool("isStand", false);
        animator.SetBool("isWalk", false);
        animator.SetBool("isJump", false);
        animator.SetBool("isFall", false);
        animator.SetBool("isLand", false);
        animator.SetBool("isGoal", false);
        animator.SetBool("isDamaged", true);
    }
}
