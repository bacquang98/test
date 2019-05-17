using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnderlineController : MonoBehaviour, IPointerClickHandler
{
    private TMP_InputField _inputField;
    private TMP_Text _text;
    private RectTransform _textRectTransform;
    private TextGenerator _textGenerator;

    private GameObject _lineUpperGameObject;
    private GameObject _lineLowerGameObject;
    private Image _lineImageUpper;
    private Image _lineImageLower;
    private RectTransform _lineUpperRectTransform;
    private RectTransform _lineLowerRectTransform;
    private Canvas _canvas;
    private int _underlineStart = 0;
    private int _underlineEnd = 0;
    private UnderlineModel _underlineModel;

    void Start()
    {   
        _canvas = gameObject.GetComponentInParent<Canvas>();

        _inputField = gameObject.GetComponent<TMP_InputField>();

        _text = _inputField.textComponent;
        
        _textRectTransform = gameObject.GetComponent<RectTransform>();
        
        _lineUpperGameObject = new GameObject("UnderlineUpper");
        _lineImageUpper = _lineUpperGameObject.AddComponent<Image>();
        _lineImageUpper.color = _text.color;
        _lineUpperRectTransform = _lineUpperGameObject.GetComponent<RectTransform>();
        _lineUpperRectTransform.SetParent(_text.gameObject.transform, false);
        
        _lineLowerGameObject = new GameObject("UnderlineLower");
        _lineImageLower = _lineLowerGameObject.AddComponent<Image>();
        _lineImageLower.color = _text.color;
        _lineLowerRectTransform = _lineLowerGameObject.GetComponent<RectTransform>();
        _lineLowerRectTransform.SetParent(_text.gameObject.transform, false);
        
        var pivot = _textRectTransform.pivot;
        _lineUpperRectTransform.anchorMin = pivot;
        _lineUpperRectTransform.anchorMax = pivot;

        _lineUpperGameObject.SetActive(false);
        _lineLowerGameObject.SetActive(false);
        
        _underlineModel = new UnderlineModel();
    }

    void Update()
    {
    }

    public void Underline()
    {
        if (_lineLowerGameObject.activeSelf)
        {
            _lineLowerGameObject.SetActive(false);
        }
        var linesInfo = _text.textInfo.lineInfo;
        if (linesInfo.Length < 1)
            return;
        var height = linesInfo[0].lineHeight;
        var factor = 1.0f / _canvas.scaleFactor;
        var position = _text.textInfo.characterInfo[_underlineStart].bottomLeft.y;
        var characterStart = _text.textInfo.characterInfo[_underlineStart].bottomLeft.x;
        var characterEnd = _text.textInfo.characterInfo[_underlineEnd].bottomLeft.x;

        _lineUpperRectTransform.anchoredPosition =
            new Vector2(factor * (characterStart + characterEnd) / 2.0f, position);
        _lineUpperRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,  Mathf.Abs(characterStart - characterEnd));
        _lineUpperRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height / 12.0f);
        
        _lineUpperGameObject.SetActive(true);
        Debug.Log("Underline");
    }
    
    public void UnderlineTwice()
    {
        var linesInfo = _text.textInfo.lineInfo;
        if (linesInfo.Length < 1)
            return;
        
        var height = linesInfo[0].lineHeight;
        var factor = 1.0f / _canvas.scaleFactor;
        var position = _text.textInfo.characterInfo[_underlineStart].bottomLeft.y;
        var characterStart = _text.textInfo.characterInfo[_underlineStart].bottomLeft.x;
        var characterEnd = _text.textInfo.characterInfo[_underlineEnd].bottomLeft.x;

        _lineUpperRectTransform.anchoredPosition =
            new Vector2(factor * (characterStart + characterEnd) / 2.0f, position);
        _lineUpperRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,  Mathf.Abs(characterStart - characterEnd));
        _lineUpperRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height / 12.0f);
        _lineUpperGameObject.SetActive(true);
        
        _lineLowerRectTransform.anchoredPosition =
            new Vector2(factor * (characterStart + characterEnd) / 2.0f, position);
        _lineLowerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,  Mathf.Abs(characterStart - characterEnd));
        _lineLowerRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height / 12.0f);
        _lineLowerGameObject.GetComponent<RectTransform>().localPosition -= new Vector3(0, 10, 0);
        
        _lineLowerGameObject.SetActive(true);
        
        Debug.Log("Underline twice");
    }

    public void RemoveUnderline()
    {
        _underlineStart = 0;
        _underlineEnd = 0;
        _lineUpperGameObject.SetActive(false);
        _lineLowerGameObject.SetActive(false);
    }

    public void Backup()
    {
        _underlineModel.text = _inputField.text;
        _underlineModel.underlineStart = _underlineStart;
        _underlineModel.underlineEnd = _underlineEnd;
        _underlineModel.isUnderlining = _lineUpperGameObject.activeSelf;
        _underlineModel.isUnderliningTwice = _lineLowerGameObject.activeSelf;
        Debug.Log("Backed up");
    }

    public void Restore()
    {
        _inputField.text = _underlineModel.text;
        _underlineStart = _underlineModel.underlineStart;
        _underlineEnd = _underlineModel.underlineEnd;
        _text.ForceMeshUpdate();
        
        if (!_underlineModel.isUnderlining && !_underlineModel.isUnderliningTwice)
            return;
        
        if (_underlineModel.isUnderliningTwice)
        {
            UnderlineTwice();
        }
        
        if (_underlineModel.isUnderlining)
        {
            Underline();
        }

        Debug.Log("Restored");
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_inputField.selectionAnchorPosition == _inputField.selectionFocusPosition)
            return;
        if (_inputField.selectionAnchorPosition == 0 && _inputField.selectionFocusPosition == 0)
            return;
        
        var a = _inputField.selectionAnchorPosition;
        var b = _inputField.selectionFocusPosition;
        
        if (a > b)
        {
            _underlineStart = b;
            _underlineEnd = a;
        }
        if (a < b)
        {
            _underlineStart = a;
            _underlineEnd = b;
        }
    }
}

[System.Serializable]
public class UnderlineModel
{
    public string text;
    public int underlineStart;
    public int underlineEnd;
    public bool isUnderlining;
    public bool isUnderliningTwice;

}
