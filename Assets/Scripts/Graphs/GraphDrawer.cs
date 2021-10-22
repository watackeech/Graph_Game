using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System;
using System.Linq;

// namespace Calc
// {
public class GraphDrawer : MonoBehaviour
{
    public GameObject GraphPrefabs; //プレハブにするグラフのLineRendererを指定
    public float LineWidth;
    public Gradient PreviewLineColor;
    public Gradient StaticLineColor;
    public Gradient DynamicLineColor;
    // public Text funcText; //関数の式表示用のテキスト
    public Text rangeErrorText; //xの最小値>最大値になっているときに表示するテキスト
    public GameObject MinFieldObject;
    public GameObject MaxFieldObject;
    public GameObject GraphListContainer;
    public float initialMinX;
    public float initialMaxX;
    public GameObject startSimulationButton;
    public InputScript inputScript;

    InputField minField;
    InputField maxField;
    Graph currentLineScript; //Graphスクリプトを指定

    // MathParser mathParser;

    private GameObject currentLine;
    // private float testNum = (float)0.5;
    private float x;
    private float y;
    private string currentSign = "null";
    private float inputN = (float)0;
    private int digits = 0; //数値を入力するときの桁数⇒二桁以上の数値入力に対応するため
    [System.NonSerialized]
    public string currentFuncString = ""; //画面に表示する用の文字式
    private string calcFuncString = ""; //実際に計算する用の文字式。計算の直前に記述
    private float minX;
    private float maxX;
    private bool isPhysicsOn = false;
    private float powerOf;
    private float toPower;
    private bool isGraphTabOn = true;
    private GraphList graphListScript;
    private float centerX = 0;
    private float centerY = 0;
    private float calcX; //式にxを代入する時用。平行移動させたるため
    private GameObject GridControllerObj;


    void Start()
    {
        GridControllerObj = GameObject.Find("GridController");
        centerX = GridControllerObj.GetComponent<GridPlotter>().centerX;
        centerY = GridControllerObj.GetComponent<GridPlotter>().centerY;
        rangeErrorText.text = "";
        // inputField = InputFieldObject.gameObject.GetComponent<InputField>();
        minField = MinFieldObject.gameObject.GetComponent<InputField>();
        maxField = MaxFieldObject.gameObject.GetComponent<InputField>();
        minField.text = initialMinX.ToString();
        maxField.text = initialMaxX.ToString();
        minX = initialMinX;
        maxX = initialMaxX;
        graphListScript = this.gameObject.GetComponent<GraphList>();
    }


    public void Draw()
    {
        DeletePreview();

        if(currentFuncString.Length > 0 && minX < maxX && isGraphTabOn){
            string judgeString = currentFuncString.Substring(currentFuncString.Length-1, 1);
            if(judgeString == " " || judgeString == "+" || judgeString == "-" || judgeString == "*" || judgeString == "/"){ //もし文字式の最後がこの条件なら、Drawせずに戻る
                return;
            }
            DrawGraph(new Vector3(0, 0, 0));
            currentLine.tag = "Preview"; //Previewタグを追加（すでにonSimulateされたものと区別するため。）
        }
    }

    public void StartSimulation(){
        if(currentLine){
            if(inputScript.funcString.Length > 0){
                if(isPhysicsOn){
                    currentLine.tag = "DynamicGraph";
                    currentLineScript.SetLineColor(DynamicLineColor);
                }else{
                    currentLine.tag = "onSimulate";
                    currentLineScript.SetLineColor(StaticLineColor);
                }
                currentLineScript.onCollider();
                currentLine.name = inputScript.funcString;
                graphListScript.AddGraphPanel(inputScript.funcString);
            }else{
                rangeErrorText.text = "式を入力しよう！";
            }
        }
    }


    private void DrawGraph(Vector3 startPoint)
    {

        // x = -2.1f;  //linewidthを0.2に設定しているときは、開始点x座標を-0.1をすると、線の中心が始めたいところに来るようになる
        // int.TryParse(minField.text, out minX);
        // int.TryParse(maxField.text, out maxX);
        float range = maxX - minX;
        currentFuncString = AddMultiple(currentFuncString, "x");
        currentFuncString = AddMultiple(currentFuncString, "(");

        // Debug.Log(currentFuncString);

        // Debug.Log(currentFuncString);

        if(0 < range){ //ちゃんとｘの最小値<最大値になっているなら
            InstantiateGraph();
            x = (float)minX;
            for (int i = 0; i < range*10+1; i++){ //ここで頂点の数を調整して、処理の重さを変える
                string poweredString = PowerCalculate(currentFuncString, (float)x);

                // Debug.Log($"x={x}で、式は{poweredString}");
                calcFuncString = poweredString.Replace("x", x.ToString()); //計算用の文字列に代入
                try{
                    y = System.Convert.ToSingle(new DataTable().Compute(calcFuncString, null)); //計算用の文字列を計算⇒float型に変換
                    // Debug.Log($"{x}でyは{y}");
                    if(-50 < y && y < 50){
                        currentLineScript.AddPoint(new Vector3(x + centerX, y + centerY, 0) + startPoint); //開始点に、計算したx,yのベクトルを足すと、新しい点がプロットできる
                    }
                    x += 0.1f; //xを次のポイントへ
                }catch{
                    // Debug.Log("例外発生中！");
                    rangeErrorText.text = "式を見直してみよう！";
                    currentLineScript.deleteThisGraph();
                }
            }
        }else{
            rangeErrorText.text = "xの範囲を確認してみよう！";
        }
    }

    string AddMultiple(string f, string x){ //計算できる形に変換するための関数
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

    void InstantiateGraph(){
            rangeErrorText.text = "";
            currentLine = Instantiate(GraphPrefabs, this.transform); //インスタンス化
            currentLineScript = currentLine.GetComponent<Graph>(); //GraphスクリプトをGraphプレハブから取得
            currentLineScript.SetLineColor(PreviewLineColor);
            currentLineScript.SetLineWidth(LineWidth);
    }

    string PowerCalculate(string f, float x){
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
                        rangeErrorText.text = "式を見直してみよう！";
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
                                    rangeErrorText.text = "式を見直してみよう！";
                                    currentLineScript.deleteThisGraph();
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
                        rangeErrorText.text = "式を見直してみよう！";
                        currentLineScript.deleteThisGraph();
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
                            rangeErrorText.text = "xの範囲を確認してみよう！";
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
                rangeErrorText.text = "式を見直してみよう！";
                currentLineScript.deleteThisGraph();
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
                        Debug.Log(ReplaceCharacter(f, result.ToString(), leftBracketLocation, f.Length - leftBracketLocation));
                        return ReplaceCharacter(f, result.ToString(), leftBracketLocation, f.Length - leftBracketLocation);
                    }else{
                        // Debug.Log(ReplaceCharacter(f, result.ToString(), 0, DefiniteBBP + 1));
                        Debug.Log(ReplaceCharacter(f, result.ToString(), leftBracketLocation, DefiniteBBP - leftBracketLocation + 1));
                        return ReplaceCharacter(f, result.ToString(), leftBracketLocation, DefiniteBBP - leftBracketLocation + 1);
                    }
                }catch{
                    rangeErrorText.text = "式を見直してみよう！";
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
                        rangeErrorText.text = "式を見直してみよう！";
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
                            rangeErrorText.text = "式を見直してみよう！";
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
                            rangeErrorText.text = "式を見直してみよう！";
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
            rangeErrorText.text = "式を見直してみよう！";
            currentLineScript.deleteThisGraph();
            return "BracketError";
        }
    }

    // float CalculateX(string cut, float x){
    //     cut = cut.Replace("x", x.ToString());
    //     try{
    //         return System.Convert.ToSingle(new DataTable().Compute(cut, null));
    //     }catch{
    //         return
    //     }
    // }

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
            return pointList.Where(x => x > -1).Min() + 1;
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

    // public void calc(){
    //     x= 2.01f;
    //     calcFuncString = currentFuncString.Replace("x", x.ToString()); //計算用の文字列に代入
    //     Debug.Log($"{currentFuncString} が関数だよ");
    //     object rawResult = new DataTable().Compute(calcFuncString, null); //計算用の文字列を計算
    //     float result = System.Convert.ToSingle(rawResult);
    //     Debug.Log($"{result} が結果だよ！！");
    // }



    // public void addNumber(float n){
    //     if(currentSign == "x"){
    //         currentSign = "number";
    //         inputN = n;
    //         digits += 1;
    //         currentFuncString += $"* {n}";
    //         funcText.text = currentFuncString;
    //     }else if(digits == 0){ //桁数が0なら、普通にinputNに入力されたnを代入
    //         currentSign = "number";
    //         inputN = n;
    //         digits += 1;
    //         currentFuncString += n.ToString();
    //         funcText.text = currentFuncString;
    //     }else{  //桁数が1より大きいなら、元のinputNを10倍して、新しく入力されたnを1の位に入れる
    //         currentSign = "number";
    //         inputN = 10*inputN + n;
    //         digits += 1;
    //         currentFuncString += n.ToString();
    //         funcText.text = currentFuncString;
    //     }
    //     Draw();
    // }

    // public void addSign(string sign){
    //     if(currentSign != sign){
    //         digits = 0;
    //         Debug.Log($"処理前のサインは {currentSign}");
    //         if(currentSign == "number" || currentSign == "null" || currentSign == "x"){
    //             currentSign = sign;
    //             currentFuncString += $" {sign} ";
    //             funcText.text = currentFuncString;
    //         }else if(sign == "number"){
    //             Debug.Log($"{currentSign}を編集中");
    //             currentSign = sign;
    //             currentFuncString = currentFuncString.Remove(currentFuncString.Length - 3, 3);
    //             currentFuncString += $" {sign} ";
    //             Debug.Log($"{currentFuncString} が現在の数式");
    //             funcText.text = currentFuncString;
    //         }
    //     }
    //     Draw();
    // }


    // public void addX(){
    //     if(currentSign == "x" || currentSign == "number" ){
    //         currentFuncString += "*x";
    //         funcText.text = currentFuncString;
    //     }else{
    //         currentFuncString += "x";
    //         funcText.text = currentFuncString;
    //     }
    //     currentSign = "x";
    //     Draw();
    // }

    // public void delete(){
    //     if(currentFuncString.Length > 0){
    //         currentFuncString = currentFuncString.Remove(currentFuncString.Length-1, 1);
    //         funcText.text = currentFuncString;
    //         if(currentFuncString.Length > 0){
    //             Draw();
    //         }else{
    //             currentSign = "null";
    //         }
    //     }
    // }

    public void setMinX(){
        //inputMinXをint型にParseし、成功すればそれをminXに代入しtrueを返す、失敗すればfalseが返る
        if(float.TryParse(minField.text, out minX)){
            if(minX < maxX){
                // Debug.Log($"xの最小値は{minX}");
                rangeErrorText.text = "";
                Draw();
            }else{
                rangeErrorText.text = "xの範囲を確認してみよう！";
            }
        }
    }

    public void setMaxX(){
        if(float.TryParse(maxField.text, out maxX)){
            if(minX < maxX){
                // Debug.Log($"xの最大値は{maxX}");
                rangeErrorText.text = "";
                Draw();
            }else{
                rangeErrorText.text = "xの範囲を確認してみよう！";
            }
        }
    }

    public void physicsSwitch(){
        isPhysicsOn = !isPhysicsOn;
    }

    public void DeleteAllGraphs(){
        foreach(Transform n in gameObject.transform){
            GameObject.Destroy(n.gameObject);
        }
        foreach(Transform n in GraphListContainer.gameObject.transform){ //リストのほうも全削除
            GameObject.Destroy(n.gameObject);
        }
        Draw();
    }

    public void StartDynamic(){
        if(startSimulationButton.GetComponent<Toggle>().isOn){
            GameObject[] dynamicGraphs = GameObject.FindGameObjectsWithTag("DynamicGraph");
            foreach(GameObject dynamicG in dynamicGraphs){
                dynamicG.GetComponent<Graph>().onDynamic();
                dynamicG.tag = "onSimulate";
            }
        }else{
            Draw();
        }
    }

    public void GraphTabOn(){
        isGraphTabOn = !isGraphTabOn;
    }

    public void DeletePreview(){
        // if(currentLine && currentLine.tag == "Preview" && minX < maxX) //Previewタグのものなら削除⇒描き直し
        // {
        //     Destroy(currentLine);
        // }
        GameObject[] previews = GameObject.FindGameObjectsWithTag("Preview");
        foreach (GameObject preview in previews)
        {
            Destroy(preview);
        }

    }


    // void Update() //うまくいかなかった
    // {
    //     // DrawGraph(new Vector3(0,2,0));
    //     // Destroy(currentLine);
    // }

}