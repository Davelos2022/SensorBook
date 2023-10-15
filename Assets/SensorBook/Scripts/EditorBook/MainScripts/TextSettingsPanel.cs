using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextSettingsPanel : MonoBehaviour, IPointerClickHandler
{
    [Header ("Link to text")]
    [SerializeField] private TextMeshProUGUI _currentText;
    [SerializeField] private RectResizer rect;

    [Header("Text Settings")]
    [SerializeField] private TMP_FontAsset[] _fonts;
    [SerializeField] private int[] _fontSize;
    [SerializeField] private TMP_Dropdown _fontSizeDropDown;
    [SerializeField] private TMP_Dropdown _fontsDropDown;
    [SerializeField] private ColorPicker colorPicker;

    [Header("UI Setting textPanel")]
    [SerializeField] private Button _boldText;
    [SerializeField] private Button _italicText;
    [SerializeField] private Button _strikeThrouhgText;
    [SerializeField] private Button _centerTextPositon;
    [SerializeField] private Button _leftTextPosition;
    [SerializeField] private Button _rightTextPosition;

    [Header("FontSettingsPanel")]
    [SerializeField] private RectTransform _panelSettings;
    

    private List<string> _fontsOptionsDropdown = new List<string>();
    private List<string> _fontsSizeOptionsDropdown = new List<string>();

    private bool _isMoveUpScreen = false;
    private bool _isMoveDownScreen = false;

    private void OnEnable()
    {
        _boldText.onClick.AddListener(SetBoldText);
        _italicText.onClick.AddListener(SetItalicText);
        _strikeThrouhgText.onClick.AddListener(SetCursivText);

        _centerTextPositon.onClick.AddListener(CenterPositonText);
        _leftTextPosition.onClick.AddListener(LeftPositionText);
        _rightTextPosition.onClick.AddListener(RightPostionText);

        for (int x = 0; x < _fonts.Length; x++)
        {
            string option = _fonts[x].name.Substring(0, 7) + "...";
            _fontsOptionsDropdown.Add(option);
        }

        for (int x = 0; x < _fontSize.Length; x++)
        {
            string option = _fontSize[x].ToString();
            _fontsSizeOptionsDropdown.Add(option);
        }

        _fontSizeDropDown.AddOptions(_fontsSizeOptionsDropdown);
        _fontsDropDown.AddOptions(_fontsOptionsDropdown);
    }


    private void OnDisable()
    {
        _boldText.onClick.RemoveListener(SetBoldText);
        _italicText.onClick.RemoveListener(SetItalicText);
        _strikeThrouhgText.onClick.RemoveListener(SetCursivText);

        _centerTextPositon.onClick.RemoveListener(CenterPositonText);
        _leftTextPosition.onClick.RemoveListener(LeftPositionText);
        _rightTextPosition.onClick.RemoveListener(RightPostionText);

        _fontsOptionsDropdown.Clear();
        _fontsDropDown .ClearOptions();
    }


    private void Update()
    {
        if (_panelSettings.anchoredPosition.x >= Screen.width - 10 && !_isMoveUpScreen)
        {
            _panelSettings.gameObject.SetActive(false);
            _isMoveUpScreen = true;
        }
    }

    public void OnSizeValueChanged(int value)
    {
        _currentText.fontSize = float.Parse(_fontSizeDropDown.options[value].text);
        _fontSizeDropDown.value = value;
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

    }
}
