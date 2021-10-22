using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSimulationButton : MonoBehaviour
{
    public Toggle toggle;
    // private bool isButtonEnabled = true;

    void Start(){
        toggle = this.gameObject.GetComponent<Toggle>();
    }

    public void SetIsOnWithoutCallback( bool isOn )
    {
        // if(isButtonEnabled){
            Toggle self = this.gameObject.GetComponent<Toggle>();
            var onValueChanged = self.onValueChanged;
            self.onValueChanged = new Toggle.ToggleEvent();
            self.isOn = isOn;
            self.onValueChanged = onValueChanged;
        // }
    }
    // public void ButtonDisabler(){
    //     isButtonEnabled = false;
    //     // self.isOn =
    //     Debug.Log(isButtonEnabled);
    //     Invoke("ButtonEnabler", 1f);
    // }
    // private void ButtonEnabler(){
    //     isButtonEnabled = true;
    //     Debug.Log(isButtonEnabled);
    // }

    // public void OnLongPressed(){
    //     Debug.Log( "長押しされた" );
    // }

}
