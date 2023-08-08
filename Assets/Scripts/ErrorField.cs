using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;

public class ErrorField : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    private Guid _guid;
    public Guid Guid { 
        get { return _guid; } 
        set { _guid = value; } 
    }

    public void SetText(string text)
    {
        _text.text = text;
    }
}
