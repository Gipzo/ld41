using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UppercaseInput : MonoBehaviour
{
    private InputField _input;

    void Awake()
    {
        _input = GetComponent<InputField>();
    }

    public void Uppercase(string value)
    {
        _input.text = _input.text.ToUpper();
    }
}
