using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class TextSettingsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentText;
    [SerializeField] private TextMeshProUGUI _placeholderText;

    [Header("Text Settings")]
    [SerializeField] private TMP_FontAsset[] _fonts;
    [SerializeField] private int[] _fontSize;
    [SerializeField] private TMP_Dropdown _fontSizeDropDown;
    [SerializeField] private TMP_Dropdown _fontsDropDown;
    [SerializeField] private FlexibleColorPicker _flexibleColorPicker;

    [Header("UI Setting textPanel")]
    [SerializeField] private Button _boldText;
    [SerializeField] private GameObject _selectedBold;
    [Space]
    [SerializeField] private Button _italicText;
    [SerializeField] private GameObject _selectedItalic;
    [Space]
    [SerializeField] private Button _underLineText;
    [SerializeField] private GameObject _selectedUnderLine;
    [Space]
    [SerializeField] private Button _colorButton;
    [Space]
    [SerializeField] private Button _centerTextPositon;
    [SerializeField] private GameObject _selectedCenter;
    [Space]
    [SerializeField] private Button _leftTextPosition;
    [SerializeField] private GameObject _selectedLeftText;
    [Space]
    [SerializeField] private Button _rightTextPosition;
    [SerializeField] private GameObject _selectedRightText;

    private List<string> _fontsOptionsDropdown = new List<string>();
    private List<string> _fontsSizeOptionsDropdown = new List<string>();

    private bool _colorMode;
    private bool _isBold;
    private bool _isItalic;
    private bool _isUnderLine;

    private string _beforeText;

    private void OnEnable()
    {
        _boldText.onClick.AddListener(SetBoldText);
        _italicText.onClick.AddListener(SetItalicText);
        _underLineText.onClick.AddListener(SetUnderLineText);

        _centerTextPositon.onClick.AddListener(CenterPositonText);
        _leftTextPosition.onClick.AddListener(LeftPositionText);
        _rightTextPosition.onClick.AddListener(RightPostionText);
        _colorButton.onClick.AddListener(OpenColorSettings);

        _fontSizeDropDown.onValueChanged.AddListener(SetSizeFontText);
        _fontsDropDown.onValueChanged.AddListener(SetFontText);

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
        _underLineText.onClick.RemoveListener(SetUnderLineText);

        _centerTextPositon.onClick.RemoveListener(CenterPositonText);
        _leftTextPosition.onClick.RemoveListener(LeftPositionText);
        _rightTextPosition.onClick.RemoveListener(RightPostionText);
        _colorButton.onClick.RemoveListener(OpenColorSettings);

        _fontSizeDropDown.onValueChanged.RemoveListener(SetSizeFontText);
        _fontsDropDown.onValueChanged.RemoveListener(SetFontText);

        _fontsOptionsDropdown.Clear();
        _fontsDropDown.ClearOptions();
    }

    private void SetBoldText()
    {
        _isBold = !_isBold;
        _selectedBold.SetActive(_isBold);

        if (_isBold)
        {
            SaveStepFontStyle(FontStyles.Bold, _selectedBold);
            _currentText.fontStyle = _currentText.fontStyle | FontStyles.Bold;
        }
        else
        {
            _currentText.fontStyle = _currentText.fontStyle -= FontStyles.Bold;
        }

        _placeholderText.fontStyle = _currentText.fontStyle;
    }

    private void SetItalicText()
    {
        _isItalic = !_isItalic;
        _selectedItalic.SetActive(_isItalic);

        if (_isItalic)
        {
            SaveStepFontStyle(FontStyles.Italic, _selectedItalic);
            _currentText.fontStyle = _currentText.fontStyle | FontStyles.Italic;
        }
        else
        {
            _currentText.fontStyle = _currentText.fontStyle -= FontStyles.Italic;
        }

        _placeholderText.fontStyle = _currentText.fontStyle;
    }

    private void SetUnderLineText()
    {
        _isUnderLine = !_isUnderLine;
        _selectedUnderLine.SetActive(_selectedUnderLine);

        if (_isUnderLine)
        {
            SaveStepFontStyle(FontStyles.Underline, _selectedUnderLine);
            _currentText.fontStyle = _currentText.fontStyle | FontStyles.Underline;
        }
        else
        {
            _currentText.fontStyle = _currentText.fontStyle -= FontStyles.Underline;
        }

        _placeholderText.fontStyle = _currentText.fontStyle;
    }

    private void SetSizeFontText(int indexSizeFont)
    {
        float currentSize = _currentText.fontSize;

        _currentText.fontSize = _fontSize[indexSizeFont];

        SaveStepValueSize(currentSize, _fontSize[indexSizeFont], _fontSizeDropDown);
    }

    private void SetFontText(int fontsIndex)
    {
        _currentText.font = _fonts[fontsIndex];
    }

    private void LeftPositionText()
    {
        _selectedCenter.SetActive(false);
        _selectedRightText.SetActive(false);

        _selectedLeftText.SetActive(true);
        _currentText.alignment = TextAlignmentOptions.MidlineLeft;
    }

    private void RightPostionText()
    {
        _selectedCenter.SetActive(false);
        _selectedLeftText.SetActive(false);

        _selectedRightText.SetActive(true);
        _currentText.alignment = TextAlignmentOptions.MidlineRight;
    }

    private void CenterPositonText()
    {
        _selectedRightText.SetActive(false);
        _selectedLeftText.SetActive(false);

        _selectedCenter.SetActive(true);
        _currentText.alignment = TextAlignmentOptions.Center;
    }

    public void OpenColorSettings()
    {
        _colorMode = !_colorMode;
        _flexibleColorPicker.gameObject.SetActive(_colorMode);
    }


    public void SetColorText(Color color)
    {
        _currentText.color = color;
    }

    public void SetSizeFontDropDown(int size)
    {
        float currentSize = _currentText.fontSize;

        _fontSizeDropDown.captionText.text = size.ToString();

        SaveStepValueSize(currentSize, size, _fontSizeDropDown);
    }

    private void SaveStepFontStyle(FontStyles styles, GameObject selected)
    {
        UndoRedoSystem.Instance.AddAction(new ChangeTextPropertiesAction
            (_currentText, _currentText.fontStyle, _currentText.fontStyle | styles, selected));
    }

    private void SaveStepValueText()
    {
        UndoRedoSystem.Instance.AddAction(new ChangeTextValueAction(_currentText, _beforeText, _currentText.text));
    }

    public void SaveStepValueSize(float oldSizeFont, float newSizeFont, TMP_Dropdown panelFontSize)
    {
        UndoRedoSystem.Instance.AddAction(new TextSizeFontAction(_currentText, oldSizeFont, newSizeFont, panelFontSize));
    }

    public void SaveStepValueFont(TMP_FontAsset oldFont, TMP_FontAsset newFont, TMP_Dropdown panelFonts)
    {

    }
}
