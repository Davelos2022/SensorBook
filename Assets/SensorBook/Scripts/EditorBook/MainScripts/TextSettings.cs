using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextSettings : MonoBehaviour, IPointerClickHandler
{
    [Header ("Link to text")]
    [SerializeField] private TextMeshProUGUI _currentText;
    [SerializeField] private RectResizer rect;

    [Header("Text Settings")]
    [SerializeField] private TMP_FontAsset[] _fonts;
    [SerializeField] private TMP_Dropdown _fontSize;
    [SerializeField] private TMP_Dropdown _fontsDropDown;

    [Header("UI Setting textPanel")]
    [SerializeField] private Button _boldText;
    [SerializeField] private Button _italicText;
    [SerializeField] private Button _strikeThrouhgText;
    [SerializeField] private Button _centerText;
    [SerializeField] private Button _leftText;
    [SerializeField] private Button _rightText;

    [Header("FontSettings")]
    [SerializeField] private GameObject _panelSettings;
    [SerializeField] private Vector2 _maxPosition;
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private float _minDistance;

    private List<string> optionsDropdown = new List<string>();

    private void OnEnable()
    {
        _boldText.onClick.AddListener(SetBoldText);
        _italicText.onClick.AddListener(SetItalicText);
        _strikeThrouhgText.onClick.AddListener(SetCursivText);

        _centerText.onClick.AddListener(CenterPositonText);
        _leftText.onClick.AddListener(LeftPositionText);
        _rightText.onClick.AddListener(RightPostionText);

        for (int x = 0; x < _fonts.Length; x++)
        {
            string option = _fonts[x].name.Substring(0, 7) + "....";
            optionsDropdown.Add(option);
        }

        _fontsDropDown.AddOptions(optionsDropdown);
    }


    private void OnDisable()
    {
        _boldText.onClick.RemoveListener(SetBoldText);
        _italicText.onClick.RemoveListener(SetItalicText);
        _strikeThrouhgText.onClick.RemoveListener(SetCursivText);

        _centerText.onClick.RemoveListener(CenterPositonText);
        _leftText.onClick.RemoveListener(LeftPositionText);
        _rightText.onClick.RemoveListener(RightPostionText);

        optionsDropdown.Clear();
        _fontsDropDown .ClearOptions();
    }

    public void OnSizeValueChanged(int value)
    {
        _currentText.fontSize = float.Parse(_fontSize.options[value].text);
        _fontSize.value = value;
    }

    public void OnFontValueChanged(int value)
    {
        _currentText.font = _fonts[value];
    }

    private void SetBoldText()
    {
        if (_currentText.fontStyle != FontStyles.Bold)
            _currentText.fontStyle = FontStyles.Bold;
        else
            _currentText.fontStyle = FontStyles.Normal;
    }

    private void SetItalicText()
    {
        if (_currentText.fontStyle != FontStyles.Italic)
            _currentText.fontStyle = FontStyles.Italic;
        else
            _currentText.fontStyle = FontStyles.Normal;
    }

    private void SetCursivText()
    {
        if (_currentText.fontStyle != FontStyles.Underline)
            _currentText.fontStyle = FontStyles.Underline;
        else
            _currentText.fontStyle = FontStyles.Normal;
    }

    private void LeftPositionText()
    {
        _currentText.alignment = TextAlignmentOptions.MidlineLeft;
    }

    private void RightPostionText()
    {
        _currentText.alignment = TextAlignmentOptions.MidlineRight;
    }

    private void CenterPositonText()
    {
        _currentText.alignment = TextAlignmentOptions.Center;
    }

    public void SetColor(Color color)
    {
        _currentText.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenSettings();    
    }

    public void OpenSettings()
    {
        if (transform.GetComponent<RectTransform>().anchoredPosition.y >= _maxPosition.y - _minDistance)
        {

        }
        else
        {

        }
    }
}
