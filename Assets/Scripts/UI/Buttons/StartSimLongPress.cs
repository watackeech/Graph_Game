using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class StartSimLongPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public float validTime = 1.0f;      //長押しとして認識する時間（これより長い時間で長押しとして認識する）

    //Local Values
    float requiredTime;                 //長押し認識時刻（この時刻を超えたら長押しとして認識する）
    bool pressing = false;              //押下中フラグ（単一指のみの取得としても利用）

    //長押しイベントコールバック（インスペクタ用）
    public UnityEvent OnLongClick;
    // public StartSimulationButton startSimulationButtonScript;


    // Update is called once per frame
    void Update()
    {
        if (pressing)  //はじめに押した指のみとなる
        {
            if (requiredTime < Time.time)   //一定時間過ぎたら認識
            {
                //コールバックイベント
                if (OnLongClick != null)
                    OnLongClick.Invoke();   //UnityEvent
                    // startSimulationButtonScript.ButtonDisabler();

                pressing = false;           //長押し完了したら無効にする
            }
        }
    }

    //UI領域内で押下
    public void OnPointerDown(PointerEventData data)
    {
        if (!pressing)          //ユニークにするため
        {
            pressing = true;
            requiredTime = Time.time + validTime;
        }
        else
        {
            pressing = false;   //２本以上の指の場合、ピンチの可能性があるため無効にする
        }
    }

    //※スマホだとUIを完全透過にしていると、指を少し動かしただけでも反応してしまうので注意
    public void OnPointerUp(PointerEventData data)
    {
        if (pressing)           //はじめに押した指のみとなる
            pressing = false;
    }

    //UI領域から外れたら無効にする
    public void OnPointerExit(PointerEventData data)
    {
        if (pressing)           //はじめに押した指のみとなる
            pressing = false;   //領域から外れたら無効にする
    }
}
