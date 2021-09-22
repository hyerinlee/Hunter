using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public InputField InputTxt;

    public void GetName(Text txt)
    {
        txt.text = InputTxt.text;
    }
}