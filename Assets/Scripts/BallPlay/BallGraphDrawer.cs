using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Data;
using System;
using System.Linq;

// namespace Calc
// {
public class BallGraphDrawer : MonoBehaviour
{
    // public GameObject GraphPrefabs; //プレハブにするグラフのLineRendererを指定
    public FuncCalculator Calc;
    public GameObject PointPrefab;
    public GameObject BasketballPrefab;
    public GameObject PinkShooterPrefab;
    // public float LineWidth;
    // public Gradient PreviewLineColor;
    // public Gradient StaticLineColor;
    // public Gradient DynamicLineColor;
    // public Text funcText; //関数の式表示用のテキスト
    public Text ErrorText; //xの最小値>最大値になっているときに表示するテキスト
    // public GameObject InputFieldObject;
    public GameObject MinFieldObject;
    public GameObject MaxFieldObject;

    public bool isFixedOn;
    public float fixedMinX;
    public float fixedMaxX;
    public Text fixedMinXText;
    public Text fixedMaxXText;

    public InputScript InputScript;
    // public GameObject GraphListContainer;

    // InputField inputField; //InputFieldからの入力を管理
    InputField minField;
    InputField maxField;
    BallGraph ballGraphScript;
    // Graph currentLineScript; //Graphスクリプトを指定
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
    private float minX = -3f;
    private float maxX = 3f;
    // private bool isPhysicsOn = false;
    private float powerOf;
    private float toPower;
    private bool isGraphTabOn = true;
    private GraphList graphListScript;
    private GameObject currentBasketball;
    private GameObject currentPinkShooter;
    private Vector3 currentPos;
    private bool isReleased = false;
    private float range;
    private float calcX;
    private float centerX = 0;
    private float centerY = 0;
    private GameObject GridControllerObj;


    void Start()
    {
        GridControllerObj = GameObject.Find("GridController");
        centerX = GridControllerObj.GetComponent<GridPlotter>().centerX;
        centerY = GridControllerObj.GetComponent<GridPlotter>().centerY;
        ErrorText.text = "";
        // inputField = InputFieldObject.gameObject.GetComponent<InputField>();
        if(isFixedOn){
            fixedMinXText.text = fixedMinX.ToString();
            fixedMaxXText.text = fixedMaxX.ToString();
        }else{
            minField = MinFieldObject.gameObject.GetComponent<InputField>();
            maxField = MaxFieldObject.gameObject.GetComponent<InputField>();
            minField.text = fixedMinX.ToString();
            maxField.text = fixedMaxX.ToString();
        }
        minX = fixedMinX;
        maxX = fixedMaxX;
        // graphListScript = this.gameObject.GetComponent<GraphList>();
        //ボール用追記
        ballGraphScript = PointPrefab.GetComponent<BallGraph>();
    }


    public void Draw()
    {
        // DeletePreview();
        DeleteAllPoints();
        CancelInvoke();
        isReleased = false;

        if(currentFuncString.Length > 0 && minX < maxX && isGraphTabOn){
            string judgeString = currentFuncString.Substring(currentFuncString.Length-1, 1);
            if(judgeString == " " || judgeString == "+" || judgeString == "-" || judgeString == "*" || judgeString == "/"){ //もし文字式の最後がこの条件なら、Drawせずに戻る
                return;
            }
            DrawGraph(new Vector3(0, 0, 0));
            // currentLine.tag = "Preview"; //Previewタグを追加（すでにonSimulateされたものと区別するため。）
        }
    }

    private void DrawGraph(Vector3 startPoint)
    {
        if(isFixedOn){
            maxX = fixedMaxX;
            minX = fixedMinX;
        }
        range = maxX - minX;
        currentFuncString = Calc.AddMultiple(currentFuncString, "x");
        currentFuncString = Calc.AddMultiple(currentFuncString, "(");

        if(0 < range){ //ちゃんとｘの最小値<最大値になっているなら
            // InstantiateGraph();
            x = (float)minX;
            // calcX = (float)minX - centerX;
            // Debug.Log(centerX);
            // Debug.Log(calcX);
            // Debug.Log(minX);

            //ボールを最初のポイントに描画
            try{
                //ボール描画
                string startPointString = Calc.PowerCalculate(currentFuncString, x);
                string calcStartPointString = startPointString.Replace("x", x.ToString());
                float startY = System.Convert.ToSingle(new DataTable().Compute(calcStartPointString, null));
                if(startY > -25 && startY < 100){
                    currentBasketball = Instantiate(BasketballPrefab, new Vector3(x + centerX, startY + centerY, 0), Quaternion.identity);
                    currentBasketball.GetComponent<Rigidbody2D>().simulated = false;
                    currentBasketball.transform.SetParent(this.gameObject.transform);
                    //PinkShooter描画
                    currentPinkShooter = Instantiate(PinkShooterPrefab, new Vector3(x + centerX - 1.19f, startY + centerY - 0.18f, 0), Quaternion.identity);
                    currentPinkShooter.transform.SetParent(this.gameObject.transform);

                    ErrorText.text = "";
                }else if(startY <= -25){
                    ErrorText.text = "開始位置が低すぎるよ！";
                }else{
                    ErrorText.text = "開始位置が高すぎるよ！";
                }
            }catch{
                ErrorText.text = "式を見直してみよう！";
            }

            for (int i = 0; i < range*2+1; i++){ //ここで頂点の数を調整して、処理の重さを変える
                string poweredString = Calc.PowerCalculate(currentFuncString, x);

                calcFuncString = poweredString.Replace("x", x.ToString()); //計算用の文字列に代入
                try{
                    y = System.Convert.ToSingle(new DataTable().Compute(calcFuncString, null)); //計算用の文字列を計算⇒float型に変換
                    if(-50 < y && y < 50){
                        Instantiate(PointPrefab, new Vector3(x + centerX, y + centerY, 0), Quaternion.identity).transform.SetParent(this.gameObject.transform);
                        // currentLineScript.AddPoint(new Vector3(x, y, 0) + startPoint); //開始点に、計算したx,yのベクトルを足すと、新しい点がプロットできる
                    }
                    x += 0.5f; //xを次のポイントへ
                    // calcX += 0.5f; //calcXも次のポイントへ
                }catch{
                    // Debug.Log("例外発生中！");
                    ErrorText.text = "式を見直してみよう！";
                }
            }
        }else{
            ErrorText.text = "xの範囲を確認してみよう！";
        }
    }

    public void StartSimulation(){
        if(currentBasketball){
            if(InputScript.funcString.Length > 0){
                currentFuncString = InputScript.funcString;
                // if(currentBasketball){
                    Rigidbody2D currentBasketballRB = currentBasketball.GetComponent<Rigidbody2D>();
                    currentBasketballRB.velocity = Vector3.zero;
                    currentBasketballRB.angularVelocity = 0f;
                // }
                // Debug.Log("StartSim");
                // currentBasketball.GetComponent<Rigidbody2D>().simulated = true;
                MoveBall();

                // graphListScript.AddGraphPanel(inputField.text);
            }else{
                ErrorText.text = "式を入力しよう！";
            }
        }
    }

    private void MoveBall(){
        x = (float)minX;
        // calcX = minX - centerX;
        float range = maxX - minX;
        currentFuncString = Calc.AddMultiple(currentFuncString, "x");
        currentFuncString = Calc.AddMultiple(currentFuncString, "(");

        if(0 < range){
            // Debug.Log("MoveBall開始");
            // for (int i = 0; i < range*2+1; i++){ //ここで頂点の数を調整して、処理の重さを変える
            if(ErrorText.text == ""){
                CancelInvoke();
                Rigidbody2D currentBasketballRB = currentBasketball.GetComponent<Rigidbody2D>();
                currentBasketballRB.simulated = true;
                currentBasketballRB.gravityScale = 0f;
                InvokeRepeating("BallMoveCalculation", 0f, 0.03f);
            }
            // }
        }else{
            ErrorText.text = "xの範囲を確認してみよう！";
        }
    }
    private void BallMoveCalculation(){
        // Debug.Log("Calculation開始");
        // x = (float)minX;
        if(x <= (float)maxX){
            string poweredString = Calc.PowerCalculate(currentFuncString, x);

            calcFuncString = poweredString.Replace("x", x.ToString()); //計算用の文字列に代入
            // try{
                y = System.Convert.ToSingle(new DataTable().Compute(calcFuncString, null)); //計算用の文字列を計算⇒float型に変換
                if(-50 < y && y < 100){
                    currentPos = currentBasketball.transform.position;
                    currentBasketball.transform.Rotate(0f, 0f, 15f);
                    // currentPos.x = Mathf.MoveTowards(currentPos.x, y, Time.deltaTime * 100f);
                    // currentPos.y = Mathf.MoveTowards(currentPos.y, y, Time.deltaTime * 100f);
                    // currentPos.x = Mathf.SmoothStep(currentPos.x, 10f, 1);
                    // currentPos.y = Mathf.SmoothStep(currentPos.y, 10f, 1);
                    currentPos.x = x + centerX;
                    currentPos.y = y + centerY;
                    currentBasketball.transform.position = currentPos;
                    // Instantiate(PointPrefab, new Vector3(x, y, 0), Quaternion.identity).transform.SetParent(this.gameObject.transform);
                    // currentLineScript.AddPoint(new Vector3(x, y, 0) + startPoint); //開始点に、計算したx,yのベクトルを足すと、新しい点がプロットできる
                }
                x += 0.16f; //xを次のポイントへ
                // calcX += 0.16f;
            // }catch{
            //     // Debug.Log("例外発生中！");
            //     ErrorText.text = "式を見直してみよう！";
            // }
            // Debug.Log(x);
        }else{
            // ReleaseBall();
            lastThrow();
        }
    }

    public void ReleaseBall(){
        // float lastX = x - 0.5f;
        // Debug.Log(lastX);
        // string poweredString = Calc.PowerCalculate(currentFuncString, lastX);
        // calcFuncString = poweredString.Replace("x", lastX.ToString());
        // float lastY = System.Convert.ToSingle(new DataTable().Compute(calcFuncString, null));
        // Vector3 previousPos = new Vector3(lastX, lastY, 0);

        // Vector3 lastBallMove = previousPos - currentPos;
        if(!isReleased){
            Debug.Log("ボールが強制リリース！");
            Vector3 lastBallMove = CalcLastMove(x - 0.1f, x);
            Rigidbody2D currentBasketballRB = currentBasketball.GetComponent<Rigidbody2D>();
            // currentBasketballRB.simulated = true;
            currentBasketballRB.gravityScale = 1f;
            currentBasketballRB.AddTorque(100f);
            //  = x.up * Mathf.PI * 1000;
            currentBasketballRB.AddForce(lastBallMove * 1200f);
            isReleased = true;
            CancelInvoke();
        }
    }

    private void lastThrow(){
        float lastX = x - 0.1f;
        // float lastCalcX = lastX - centerX;
        // Debug.Log(lastX);
        string poweredString = Calc.PowerCalculate(currentFuncString, lastX);
        calcFuncString = poweredString.Replace("x", lastX.ToString());
        float lastY = System.Convert.ToSingle(new DataTable().Compute(calcFuncString, null));

        Vector3 previousPos = new Vector3(lastX + centerX, lastY + centerY, 0);
        Vector3 lastBallMove = previousPos - currentBasketball.transform.position;
        Rigidbody2D currentBasketballRB = currentBasketball.GetComponent<Rigidbody2D>();
        // currentBasketballRB.simulated = true;
        currentBasketballRB.gravityScale = 1f;
        currentBasketballRB.AddTorque(100f);
        //  = Vector3.up * Mathf.PI * 1000;
        currentBasketballRB.AddForce(lastBallMove * 5000f);
        CancelInvoke();
        isReleased = true;
    }

    private Vector3 CalcLastMove(float lastX, float currentX){
        // float lastX = x - 0.5f;
        // Debug.Log(lastX);
        // float lastCalcX = lastX - centerX;
        string poweredString = Calc.PowerCalculate(currentFuncString, lastX);
        calcFuncString = poweredString.Replace("x", lastX.ToString());
        float lastY = System.Convert.ToSingle(new DataTable().Compute(calcFuncString, null));
        Vector3 previousPos = new Vector3(lastX + centerX, lastY + centerY, 0);

        // float currentX = x;
        // Debug.Log(lastX);
        // float currentCalcX = currentX - centerX;
        // poweredString = Calc.PowerCalculate(currentFuncString, x);
        // calcFuncString = poweredString.Replace("x", x.ToString());
        // float currentY = System.Convert.ToSingle(new DataTable().Compute(calcFuncString, null));
        // Vector3 currentPos = new Vector3(currentX + centerX, currentY + centerY, 0);
        Vector3 lastMoveVec = currentBasketball.transform.position - previousPos;

        return lastMoveVec;
    }

    // public void UpdateFuncString(){
    //     currentFuncString = inputField.text;
    //     // Debug.Log(currentFuncString);
    //     if(currentFuncString.Length > 0){
    //         Draw();
    //     }
    // }

    public void setMinX(){
        //inputMinXをint型にParseし、成功すればそれをminXに代入しtrueを返す、失敗すればfalseが返る
        if(float.TryParse(minField.text, out minX) && !isFixedOn){
            if(minX < maxX){
                // Debug.Log($"xの最小値は{minX}");
                minX += centerX;
                ErrorText.text = "";
                Draw();
            }else{
                ErrorText.text = "xの範囲を確認してみよう！";
            }
        }
    }
    public void setMaxX(){
        if(float.TryParse(maxField.text, out maxX) && !isFixedOn){
            if(minX < maxX){
                // Debug.Log($"xの最大値は{maxX}");
                minX += centerX;
                ErrorText.text = "";
                Draw();
            }else{
                ErrorText.text = "xの範囲を確認してみよう！";
            }
        }
    }

    public void DeletePreview(){
        GameObject[] previews = GameObject.FindGameObjectsWithTag("Preview");
        foreach (GameObject preview in previews)
        {
            Destroy(preview);
        }
    }
    public void GraphTabOn(){
        isGraphTabOn = !isGraphTabOn;
    }
    public void DeleteAllPoints(){
        foreach(Transform n in gameObject.transform){ //子要素すべて削除
            GameObject.Destroy(n.gameObject);
        }
    }

}