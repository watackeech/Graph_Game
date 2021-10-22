using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class PhysicSwitch : MonoBehaviour
{
    private bool isSwitchOn = false;
    public GameObject switchDot;
    public GameObject switchBackground;

    public void OnSwitchDotClicked(){
        switchDot.transform.DOLocalMoveX(-switchDot.transform.localPosition.x,0.2f);
        isSwitchOn = !isSwitchOn;
        Debug.Log(isSwitchOn);
        if(isSwitchOn){
            switchBackground.GetComponent<RawImage>().color = Color.green;
        }else{
            switchBackground.GetComponent<RawImage>().color = Color.grey;
        }
        // switchState = Math.Sign(-switchDot.transform.localPosition.x);
    }
}
