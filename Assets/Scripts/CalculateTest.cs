using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;

public class CalculateTest : MonoBehaviour
{

    public Text funcText;
    private float x = 8;
    private float f;
    private string currentSign = "null";
    private float inputN = (float)0;
    private int digits = 0;
    private string currentFuncString = "";
    private string calcFuncString = "";


    public void calculate(){

        // x = 1;
        // Debug.Log(inputN);
        // fourBasicCalc(inputN);
        // Debug.Log(f);
        // funcText.text = currentFuncString;

        // object fuff = 2*3 + Mathf.Pow(x, 2);
        // Debug.Log(fuff);
        // string math = "100 * x + x";
        calcFuncString = currentFuncString.Replace("x", x.ToString()); //計算用の文字列に代入
        Debug.Log($"{currentFuncString} が関数だよ");
        float result = (float)new DataTable().Compute(calcFuncString, null); //計算用の文字列を計算
        Debug.Log($"{result} が結果だよ！！");

    }

    // public void fourBasicCalc(float a){
    //     if(currentSign == "+"){
    //         doPlus(a);
    //     }else if(currentSign == "-"){
    //         doMinus(a);
    //     }
    // }
    // public void doPlus(float a){
    //     f = f + a;
    // }
    // public void doMinus(float a){
    //     f = f - a;
    // }

    public void addSign(string sign){
        if(currentSign != sign){
            digits = 0;
            Debug.Log($"処理前のサインは {currentSign}");
            if(currentSign == "number" || currentSign == "null" || currentSign == "x"){
                currentSign = sign;
                currentFuncString += $" {sign} ";
                funcText.text = currentFuncString;
            }else if(sign == "number"){
                Debug.Log($"{currentSign}を編集中");
                currentSign = sign;
                currentFuncString = currentFuncString.Remove(currentFuncString.Length - 3, 3);
                currentFuncString += $" {sign} ";
                Debug.Log($"{currentFuncString} が現在の数式");
                funcText.text = currentFuncString;
            }
        }
    }

    public void addX(){
        if(currentSign == "x" || currentSign == "number" ){
            currentFuncString += "*x";
            funcText.text = currentFuncString;
        }else{
            currentFuncString += "x";
            funcText.text = currentFuncString;
        }
        currentSign = "x";
    }


    public void addNumber(float n){
        if(currentSign == "x"){
            currentSign = "number";
            inputN = n;
            digits += 1;
            currentFuncString += $"* {n}";
            funcText.text = currentFuncString;
        }else if(digits == 0){ //桁数が0なら、普通にinputNに入力されたnを代入
            currentSign = "number";
            inputN = n;
            digits += 1;
            currentFuncString += n.ToString();
            funcText.text = currentFuncString;
        }else{  //桁数が1より大きいなら、元のinputNを10倍して、新しく入力されたnを1の位に入れる
            currentSign = "number";
            inputN = 10*inputN + n;
            digits += 1;
            currentFuncString += n.ToString();
            funcText.text = currentFuncString;
        }
    }

    public void delete(){
        currentFuncString = currentFuncString.Remove(currentFuncString.Length-1, 1);
        funcText.text = currentFuncString;
        // currentSign = "null";
    }



    //addSignにリファクター済み
    // public void addPlus(){
    //     if(currentSign != "+"){
    //         Debug.Log($"処理前のサインは {currentSign}");
    //         if(currentSign == "number" || currentSign == "null"){
    //             currentSign = "+";
    //             digits = 0;
    //             currentFuncString += " + ";
    //             funcText.text = currentFuncString;
    //         }else{
    //             Debug.Log($"{currentSign}を編集中");
    //             currentSign = "+";
    //             digits = 0;
    //             currentFuncString = currentFuncString.Remove(currentFuncString.Length - 3, 3);
    //             currentFuncString += " + ";
    //             Debug.Log($"{currentFuncString} が現在の数式");
    //             funcText.text = currentFuncString;
    //         }
    //     }
    // }

    // public void addMinus(){
    //     if(currentSign != "-"){
    //         Debug.Log($"処理前のサインは {currentSign}");
    //         if(currentSign == "number" || currentSign == "null"){
    //             currentSign = "-";
    //             digits = 0;
    //             currentFuncString += " - ";
    //             funcText.text = currentFuncString;
    //         }else{
    //             Debug.Log($"{currentSign}を編集中");
    //             currentSign = "-";
    //             digits = 0;
    //             currentFuncString =  currentFuncString.Remove(currentFuncString.Length - 3, 3);
    //             currentFuncString += " - ";
    //             Debug.Log($"{currentFuncString} が現在の数式");
    //             funcText.text = currentFuncString;
    //         }
    //     }
    // }

}
