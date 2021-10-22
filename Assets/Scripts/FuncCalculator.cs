using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System;
using System.Linq;

public class FuncCalculator : MonoBehaviour
{
    public Text ErrorText;
    private float toPower;
    private float powerOf;

    public string AddMultiple(string f, string x){ //計算できる形に変換するための関数
        string removedString = f.Replace(x, "");
        int countX = f.Length - removedString.Length; //文字列に含まれるxの個数を割り出す
        if(countX > 0){
            string aStr = f.Replace(x, "a");
            for(int i = 0; i < countX; i++){
                int xLocation = aStr.IndexOf("a");
                // System.Console.WriteLine(xLocation);
                if(xLocation == 0){
                    // System.Console.WriteLine("ｘが一文字目");
                    // aStr = aStr.Remove(0, 1).Insert(xLocation, x);
                    aStr = ReplaceCharacter(aStr, x, 0, 1);
                }else{
                    string previousChar = aStr.Substring(xLocation-1, 1);
                    if(previousChar == " " || previousChar == "+" || previousChar == "-" || previousChar == "*" || previousChar == "/" || previousChar == "^" || previousChar == "("){
                        // System.Console.WriteLine("直前が記号");
                        aStr = ReplaceCharacter(aStr, x, xLocation, 1);
                        // aStr = aStr.Remove(xLocation, 1).Insert(xLocation, x);
                    }else{
                        // System.Console.WriteLine("xを*aに変換！");
                        // aStr = aStr.Remove(xLocation, 1).Insert(xLocation, $"*{x}");
                        aStr = ReplaceCharacter(aStr, $"*{x}", xLocation, 1);
                    }
                }
            }
            f = aStr;
        }
        return f;
    }

    public string PowerCalculate(string f, float x){
        int countPower = countCharacter(f, "^");
        // Debug.Log($"{countPower}が^の個数");
        int powerLocation = f.IndexOf("^");
        if(countPower > 0){
            if(powerLocation != 0 && powerLocation + 1 != f.Length){ //"^"が一つ入っている かつ 一文字目に"^"が来ていない場合
                for(int i = 0; i < countPower; i++){
                    powerLocation = f.IndexOf("^");
                    // Debug.Log($"{powerLocation}が^の位置（{i}）");

                    //##########ここから、 ^ の後に何があるかを判断する
                    if(f.Substring(powerLocation+1, f.Length - powerLocation - 1).Length == 0){
                        return f;
                        ErrorText.text = "式を見直してみよう！";
                    }
                    //"^"の後にくる最初のスペースの位置を特定
                    // "^"が0番目の文字列を作って、「その文字列で」何番目がBreakPointかを表している
                    // x^-23+2の場合、4と表示される
                    int BehindBreakPoint = BehindBreakPointChecker(f.Substring(powerLocation+1, f.Length - powerLocation - 1));
                    // Debug.Log(BehindBreakPoint);
                    // Debug.Log(BehindBreakPoint);

                    try{
                        if(BehindBreakPoint > 0){ //途中に"^"がある場合
                        // System.Console.WriteLine(BehindBreakPoint);
                        // Debug.Log(f.Substring(powerLocation+2, 1));


                            if(f.Substring(powerLocation+1, 1) == "("){ //^( )の形になっているとき
                                // Debug.Log(f.Substring(powerLocation+2, f.Length - powerLocation - 2).IndexOf(")"));
                                int rightLocation = f.Substring(powerLocation+2, f.Length - powerLocation - 2).IndexOf(")");
                                if(rightLocation > 0){
                                    BehindBreakPoint = rightLocation + 2;
                                    Debug.Log(f.Substring(BehindBreakPoint+powerLocation, 1));
                                    Debug.Log(BehindBreakPoint);
                                    if(f.Substring(powerLocation+2, 1) == "-" || f.Substring(powerLocation+2, 1) == "+"){ // ^(-2)などのとき
                                        Debug.Log(f.Substring(powerLocation+2, BehindBreakPoint-2));
                                        if(float.TryParse(f.Substring(powerLocation+2, BehindBreakPoint-2), out powerOf)){
                                            if(f.Length == powerLocation+BehindBreakPoint+1){
                                                Debug.Log("これは最後");
                                                f = ReplaceCharacter(f, " ", powerLocation+BehindBreakPoint, 1);
                                                Debug.Log(f);
                                            }else{
                                                BehindBreakPoint += 1;
                                                Debug.Log(powerOf);
                                            }
                                            //成功（^の後ろがParseできる形）
                                        }else{
                                            string xFunc = f.Substring(powerLocation+1, BehindBreakPoint).Replace("x", x.ToString());
                                            powerOf = System.Convert.ToSingle(new DataTable().Compute(xFunc, null));
                                        }
                                    }else{
                                        if(float.TryParse(f.Substring(powerLocation+2, BehindBreakPoint-2), out powerOf)){
                                            Debug.Log(powerOf);
                                            f = ReplaceCharacter(f, " ", powerLocation+BehindBreakPoint, 1);
                                            // BehindBreakPoint += 1;
                                        }else{
                                            string xFunc = f.Substring(powerLocation+1, BehindBreakPoint).Replace("x", x.ToString());
                                            powerOf = System.Convert.ToSingle(new DataTable().Compute(xFunc, null));
                                            f = ReplaceCharacter(f, " ", powerLocation+BehindBreakPoint, 1);
                                            Debug.Log(powerOf);
                                        }
                                    }
                                }else{
                                    ErrorText.text = "式を見直してみよう！";
                                    // currentLineScript.deleteThisGraph();
                                    return f;
                                }
                            }else{
                                if(float.TryParse(f.Substring(powerLocation+1, BehindBreakPoint), out powerOf)){
                                    //成功（^の後ろがParseできる形）
                                }else{
                                    string xFunc = f.Substring(powerLocation+1, BehindBreakPoint).Replace("x", x.ToString());
                                    powerOf = System.Convert.ToSingle(new DataTable().Compute(xFunc, null));
                                    BehindBreakPoint += 1;
                                    Debug.Log($"{powerOf}がパワー");
                                }
                            }
                            // System.Console.WriteLine(powerOf);
                        }else if(powerLocation < f.Length - 1){ //最後に"^2"があり、後ろに空白がない場合
                            if(float.TryParse(f.Substring(powerLocation+1, f.Length - powerLocation - 1), out powerOf)){

                                //成功（^の後ろがParseできる形）
                            }else{
                                string xFunc = f.Substring(powerLocation+1, f.Length - powerLocation - 1).Replace("x", x.ToString());
                                powerOf = System.Convert.ToSingle(new DataTable().Compute(xFunc, null));
                            }
                            // System.Console.WriteLine(powerOf);
                        }
                    }catch{
                        ErrorText.text = "式を見直してみよう！";
                        // currentLineScript.deleteThisGraph();
                        return f;
                    }

                    //#####################ここからは ^ の前に何があるかを判断する
                    string beforePower = f.Substring(powerLocation-1, 1);
                    // System.Console.WriteLine(beforePower);
                    int FrontBreakPoint = FrontBreakPointChecker(f.Substring(0, powerLocation));

                    if(beforePower == "x"){
                        toPower = (float)x;
                        float resultForX = (float)Mathf.Pow(toPower, powerOf);
                        // Debug.Log($"{resultForX}が最終的な変形");
                        if(Single.IsNaN(resultForX)){ //resultForXがNan（定義不能）かどうかチェック！
                            ErrorText.text = "xの範囲を確認してみよう！";
                            return "IsNan";
                        }else{
                            if(BehindBreakPoint == -1){ //この部分が文字列の一番後ろにある
                                // f = f.Remove(powerLocation - 1, f.Length - powerLocation + 1).Insert(powerLocation - 1, resultForX.ToString());
                                f = ReplaceCharacter(f, resultForX.ToString(), powerLocation - 1, f.Length - powerLocation + 1);
                            }else{
                                // 絶対的なBehindBreakPointは、powerLocation + BehindBreakPoint + 1
                                // よって、求めたい文字列の長さは、"絶対的なBehindBreakPoint - powerLocation + 1"になる
                                f = ReplaceCharacter(f, resultForX.ToString(), powerLocation - 1, BehindBreakPoint + 2);
                                // f = f.Remove(powerLocation - 1, BehindBreakPoint + 2).Insert(powerLocation - 1, resultForX.ToString());
                            }
                        }
                        return PowerCalculate(f, x);
                        // System.Console.WriteLine(result);
                    }else if(beforePower == ")"){
                        // return = BracketsChecker(string f, int powerLocation, int BehindBreakPoint, float x, float powerOf);
                        return PowerCalculate(BracketsChecker(f, powerLocation, BehindBreakPoint, x, powerOf), x);
                    }else{
                        // System.Console.WriteLine(FrontBreakPoint);
                        // "2^-2 + 3^3 - x^2"
                        if(FrontBreakPoint > -1){
                            // Debug.Log(FrontBreakPoint);
                            float.TryParse(f.Substring(FrontBreakPoint+1, powerLocation - FrontBreakPoint - 1), out toPower);

                        }else{
                            // Debug.Log("あああああああああああ");
                            float.TryParse(f.Substring(0, powerLocation - FrontBreakPoint - 1), out toPower);
                        }
                    }

                    float result = (float)Mathf.Pow(toPower, powerOf);
                    // Debug.Log(f.Substring(FrontBreakPoint+1, powerLocation - FrontBreakPoint - 1));

                    if(BehindBreakPoint != -1 && FrontBreakPoint != -1){
                        // f = f.Remove(FrontBreakPoint + 1, powerLocation + BehindBreakPoint - FrontBreakPoint).Insert(FrontBreakPoint + 1, result.ToString());
                        f = ReplaceCharacter(f, result.ToString(), FrontBreakPoint + 1, powerLocation + BehindBreakPoint - FrontBreakPoint);
                    }else if(BehindBreakPoint == -1 && FrontBreakPoint == -1){
                        // f = f.Remove(0, f.Length).Insert(0, result.ToString());
                        f = ReplaceCharacter(f, result.ToString(), 0, f.Length);
                    }else if(BehindBreakPoint == -1){
                        // f = f.Remove(FrontBreakPoint + 1, f.Length - FrontBreakPoint - 1).Insert(FrontBreakPoint + 1, result.ToString());
                        f = ReplaceCharacter(f, result.ToString(), FrontBreakPoint + 1, f.Length - FrontBreakPoint - 1);
                    }else if(FrontBreakPoint == -1){
                        // f = f.Remove(0, powerLocation + BehindBreakPoint).Insert(0, result.ToString());
                        f = ReplaceCharacter(f, result.ToString(), 0, powerLocation + BehindBreakPoint);
                    }
                    powerLocation = f.IndexOf("^");
                }

            }else{
                ErrorText.text = "式を見直してみよう！";
                // currentLineScript.deleteThisGraph();
            }
        }else{
            // Debug.Log("べき乗なし");
        }
        // Debug.Log($"{f}が最終的な式");
        return f;
    }

    string BracketsChecker(string f, int powerLocation, int BehindBreakPoint, float x, float powerOf){
        string cut = f.Substring(0, powerLocation);
        int countLeft = cut.Length - cut.Replace("(", "").Length;
        int countRight = cut.Length - cut.Replace(")", "").Length;
        int DefiniteBBP = BehindBreakPoint + powerLocation;
        if(countLeft == countRight){
            int leftBracketLocation = cut.IndexOf("(");
            if(countLeft == 1){
                // xを代入
                cut = cut.Substring(leftBracketLocation + 1, powerLocation - leftBracketLocation - 2).Replace("x", x.ToString());
                // Debug.Log($"{str}がx代入後");
                try{
                    float toPower = System.Convert.ToSingle(new DataTable().Compute(cut, null));
                    float result = (float)Mathf.Pow(toPower, powerOf);
                    if(BehindBreakPoint <= -1){
                        // Debug.Log(ReplaceCharacter(f, result.ToString(), leftBracketLocation, f.Length - leftBracketLocation));
                        return ReplaceCharacter(f, result.ToString(), leftBracketLocation, f.Length - leftBracketLocation);
                    }else{
                        // Debug.Log(ReplaceCharacter(f, result.ToString(), 0, DefiniteBBP + 1));
                        // Debug.Log(ReplaceCharacter(f, result.ToString(), leftBracketLocation, DefiniteBBP - leftBracketLocation + 1));
                        return ReplaceCharacter(f, result.ToString(), leftBracketLocation, DefiniteBBP - leftBracketLocation + 1);
                    }
                }catch{
                    ErrorText.text = "式を見直してみよう！";
                    return "BracketError";
                }
            }else{
                // Debug.Log($"（）は{countLeft}組あるよ");
                string tempocut = cut.Substring(0, cut.Length - 1);
                // Debug.Log(tempocut);
                int leftPosition = tempocut.LastIndexOf("(");
                int rightPosition = tempocut.LastIndexOf(")");
                if(rightPosition < leftPosition){ //left" ( "のほうが近い（後ろの）とき  (x+2)+(x+1)^2のような状況
                    Debug.Log("よおおおおおおおおおおおおおおおおお");
                    cut = cut.Substring(leftPosition + 1, powerLocation - leftPosition - 2).Replace("x", x.ToString());
                    Debug.Log(cut);
                    try{
                        float toPower = System.Convert.ToSingle(new DataTable().Compute(cut, null));
                        float result = (float)Mathf.Pow(toPower, powerOf);
                        // int leftPosition = tempocut.Length - leftDistance - 1;
                        Debug.Log($"{cut}が計算式");
                        return ReplaceCharacter(f, result.ToString(), leftPosition, DefiniteBBP-leftPosition+1);
                    }catch{
                        ErrorText.text = "式を見直してみよう！";
                        return "BracketError";
                    }
                }else{ //right" ) "のほうが近い（後ろの）とき  ((x+2)+(x+1))^2や、(x+2)+(1+(x+1))^2のような状況
                    string smallerBrackets = cut.Substring(leftPosition + 1, rightPosition - leftPosition - 1); //一番内側かつ右の括弧内を取得
                    Debug.Log($"{smallerBrackets}が括弧の内側");
                    int anotherRightPosition = smallerBrackets.Substring(0, smallerBrackets.Length).LastIndexOf(")");
                    Debug.Log($"{anotherRightPosition}が右括弧の位置！");
                    if(countCharacter(smallerBrackets, ")") <= 0){
                        string calcX = smallerBrackets.Replace("x", x.ToString());
                        try{
                            float resultInBrackets = System.Convert.ToSingle(new DataTable().Compute(calcX, null));
                            string modifiedCut = ReplaceCharacter(f, resultInBrackets.ToString(), leftPosition, rightPosition - leftPosition + 1);
                            Debug.Log(modifiedCut);
                            int deduction = f.Length - modifiedCut.Length;
                            powerLocation -= deduction;
                            BehindBreakPoint -= deduction;
                            modifiedCut = BracketsChecker(modifiedCut, powerLocation, BehindBreakPoint, x, powerOf); //()がなくなるまでループさせる
                            return modifiedCut;
                        }catch{
                            ErrorText.text = "式を見直してみよう！";
                            return "BracketError";
                        }
                    }else{ //smallerBrackets内に右括弧 ）がある場合
                        string anotherSmallerBrackets = smallerBrackets.Substring(0, anotherRightPosition);
                        string calcX = anotherSmallerBrackets.Replace("x", x.ToString());
                        Debug.Log($"{calcX}がx代入後の式！");
                        try{
                            float resultInBrackets = System.Convert.ToSingle(new DataTable().Compute(calcX, null));
                            string modifiedCut = ReplaceCharacter(f, resultInBrackets.ToString(), leftPosition, anotherRightPosition + 2);
                            Debug.Log($"{modifiedCut}が新たな式！");
                            int deduction = f.Length - modifiedCut.Length;
                            powerLocation -= deduction;
                            BehindBreakPoint -= deduction;
                            modifiedCut = BracketsChecker(modifiedCut, powerLocation, BehindBreakPoint, x, powerOf); //()がなくなるまでループさせる
                            return modifiedCut;
                        }catch{
                            ErrorText.text = "式を見直してみよう！";
                            return "BracketError";
                        }
                    }
                }
                // for(int i = 0; i < countLeft; i++){
                //     cut = PowerCalculate(cut, x);
                //     Debug.Log(cut);
                // }
            }
        }else{
            ErrorText.text = "式を見直してみよう！";
            // currentLineScript.deleteThisGraph();
            return "BracketError";
        }
    }

    int BehindBreakPointChecker(string str){
        //  ^ の直後の文字は存在するという条件下でのみ呼び出し
        // こうすることで、x^-2などを計算できるようになる
        str = str.Substring(1, str.Length - 1); //スタート地点を^の次の次からにする
        int sp = str.IndexOf(" ");
        int pp = str.IndexOf("+");
        int mp = str.IndexOf("-");
        int mlp = str.IndexOf("*");
        int dp = str.IndexOf("/");
        int lb = str.IndexOf("(");
        int rb = str.IndexOf(")");
        int pwp = str.IndexOf("^");
        List<int> pointList = new List<int>{sp,pp,mp,mlp,dp,lb,rb,pwp};
        bool isExist = pointList.Any(x => x > -1);
        if(isExist){
            return pointList.Where(x => x > -1).Min() + 1; //stackoverflowerror
        }else{
            return -1;
        }
    }
    int FrontBreakPointChecker(string str){
        int sp = str.LastIndexOf(" ");
        int pp = str.LastIndexOf("+");
        int mp = str.LastIndexOf("-");
        int mlp = str.LastIndexOf("*");
        int dp = str.LastIndexOf("/");
        int lb = str.LastIndexOf("(");
        int rb = str.LastIndexOf(")");
        List<int> pointList = new List<int>{sp,pp,mp,mlp,dp,lb,rb};
        bool isExist = pointList.Any(x => x > -1);
        if(isExist){
            return pointList.Where(x => x > -1).Max();
        }else{
            return -1;
        }
    }
    string ReplaceCharacter(string str, string replacement, int startPoint, int count){
        str = str.Remove(startPoint, count).Insert(startPoint, replacement);
        return str;
    }
    int countCharacter(string f, string character){
        string removedString = f.Replace(character, "");
        return f.Length - removedString.Length;
    }
}
