using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextDotsAnimation : MonoBehaviour
{
    public string message;
    [SerializeField] private bool textMeshPro;
    [Range(0.05f, 1)] [SerializeField] private float time = 0.1f;
    [SerializeField] private int numberOfDots = 3;
    private Text _text;
    private TextMeshProUGUI _textTMP;

    private float _timeElapsed;
    private int _dotsCount;
    private bool _flip = false;
    string str = "";

    private void Awake()
    {
        if (textMeshPro)
        {
            _textTMP = GetComponent<TextMeshProUGUI>();
        }
        else
        {
            _text = GetComponent<Text>();
        }

        // _flip = false;
    }

    private void OnEnable()
    {
        if (textMeshPro)
        {
            _textTMP.text = message;
        }
        else
        {
            _text.text = message;
        }
    }

    private void LateUpdate()
    {
        _timeElapsed += Time.deltaTime;

        if (textMeshPro)
        {
            // string str = _textTMP.text;

            if (_timeElapsed > time)
            {
                _timeElapsed = 0;
                str += ".";
                _dotsCount++;
                
                if (_dotsCount > numberOfDots)
                {
                    str = "";
                    _dotsCount = 0;
                }
                
                _textTMP.text = message + str;
            }
        }
        else
        {
            // string str = "";
            
            if (_timeElapsed > time)
            {
                _timeElapsed = 0;
                str += ".";
                _dotsCount++;
                
                if (_dotsCount > numberOfDots)
                {
                    str = "";
                    _dotsCount = 0;
                }
                
                _text.text = message + str;
            }
        }
    }
}
