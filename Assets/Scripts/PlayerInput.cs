using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    string hAxisName = "Horizontal";
    string vAxisName = "Vertical";
    string wAxisName = "Walk";
    string jAxisName = "Jump";
    string fAxisName = "Fire1";
    string f2AxisName = "Fire2";
    string rAxisName = "Reload";
    string iAxisName = "Interaction";
    string swap1 = "Swap1";
    string swap2 = "Swap2";
    string swap3 = "Swap3";

    public float h { get; private set; }
    public float v { get; private set; }
    public bool IsRun { get; private set; }
    public bool IsWalk { get; private set; }
    public bool Jump { get; private set; }
    public bool Fire { get; private set; }
    public bool Fire2 { get; private set; }
    public bool Reload { get; private set; }
    public bool IsInteraction { get; private set; }
    public bool Swap1 { get; private set; }
    public bool Swap2 { get; private set; }
    public bool Swap3 { get; private set; }


    void Update()
    {
        h = Input.GetAxisRaw(hAxisName);
        v = Input.GetAxisRaw(vAxisName);
        Jump = Input.GetButton(jAxisName);
        Fire = Input.GetButton(fAxisName);
        Fire2 = Input.GetButtonDown(f2AxisName);
        Reload = Input.GetButtonDown(rAxisName);
        IsInteraction = Input.GetButtonDown(iAxisName);
        Swap1 = Input.GetButtonDown(swap1);
        Swap2 = Input.GetButtonDown(swap2);
        Swap3 = Input.GetButtonDown(swap3);

        IsRun = h != 0 || v != 0 ? true : false;
        IsWalk = (Input.GetButton(wAxisName) && IsRun);
       
    }

    
}
