using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LongPressController : MonoBehaviour
{
    public float validTime = 1.0f;      //長押しとして認識する時間（これより長い時間で長押しとして認識する）

    //認識する画面上の領域
    public Rect validArea = new Rect(0, 0, 1, 1);    //長押しとして認識する画面領域（0.0～1.0）[(0,0):画面左下, (1,1):画面右上]

    //Local Values
    Vector2 minPos = Vector2.zero;      //長押し認識ピクセル最小座標
    Vector2 maxPos = Vector2.one;       //長押し認識ピクセル最大座標
    float requiredTime;                 //長押し認識時刻（この時刻を超えたら長押しとして認識する）
    bool pressing;                      //押下中フラグ（単一指のみの取得にするため）

    bool isValid = false;               //フレーム毎判定用

    //長押検出プロパティ（フレーム毎取得用）
    public bool IsLongClick {
        get { return isValid; }
    }

    //長押しイベントコールバック（インスペクタ用）
    public UnityEvent OnLongClick;      //引数なし


    //アクティブになったら、初期化する（アプリの中断などしたときはリセットする）
    // void OnEnable()
    // {
    //     pressing = false;
    // }

    // // Update is called once per frame
    // void Update()
    // {
    //     isValid = false;    //フレーム毎にリセット


    //     //プリプロセス命令
    //     //https://programming.pc-note.net/csharp/preprocess.html
    //     //プラットフォーム依存コンパイル⇒コンパイルするプラットフォームによって、処理を変えられる
    //     //https://docs.unity3d.com/ja/2018.4/Manual/PlatformDependentCompilation.html

    //     #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)   //タッチで取得したいプラットフォームのみ
    //             if (Input.touchCount == 1)  //複数の指は不可とする（※２つ以上の指の場合はピンチの可能性もあるため）
    //     #endif
    //     {
    //         if (!pressing && Input.GetMouseButtonDown(0))   //押したとき（左クリック/タッチが取得できる）
    //         {
    //             Vector2 pos = Input.mousePosition; //マウス位置取得
    //             minPos.Set(validArea.xMin * Screen.width, validArea.yMin * Screen.height); //マウスのスクリーンに対する相対位置セット
    //             maxPos.Set(validArea.xMax * Screen.width, validArea.yMax * Screen.height);
    //             if (minPos.x <= pos.x && pos.x <= maxPos.x && minPos.y <= pos.y && pos.y <= maxPos.y)   //認識エリア内
    //             {
    //                 pressing = true;
    //                 requiredTime = Time.time + validTime; //現在時刻＋有効化までの時間
    //             }
    //         }
    //         if (pressing)      //既に押されている
    //         {
    //             if (Input.GetMouseButton(0))    //押下継続（※この関数は２つ以上タッチの場合、どの指か判別できない）
    //             {
    //                 if (requiredTime < Time.time)   //一定時間過ぎたら認識
    //                 {
    //                     Vector2 pos = Input.mousePosition;
    //                     if (minPos.x <= pos.x && pos.x <= maxPos.x && minPos.y <= pos.y && pos.y <= maxPos.y)   //認識エリア内
    //                     {
    //                         isValid = true;
    //                         Debug.Log("ロングプレス！");

    //                         //コールバックイベント
    //                         if (OnLongClick != null)
    //                             OnLongClick.Invoke();   //UnityEvent
    //                     }

    //                     pressing = false;   //長押し完了したら無効にする
    //                 }
    //             }
    //             else  //MouseButtonUp, MouseButtonDown
    //             {
    //                 pressing = false;
    //             }
    //         }
    //     }
    // #if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX)   //タッチで取得したいプラットフォームのみ
    //         else  //タッチが１つでないときは無効にする（※２つ以上の指の場合はピンチの可能性もあるため）
    //         {
    //             pressing = false;
    //         }
    // #endif
    // }
}
