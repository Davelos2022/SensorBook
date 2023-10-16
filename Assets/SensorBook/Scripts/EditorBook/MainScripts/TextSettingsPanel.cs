using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextSettingsPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _currentText;
    [SerializeField] private RectTransform _rectTransformText;
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
    [SerializeField] private Button _italicText;
    [SerializeField] private GameObject _selectedItalic;
    [SerializeField] private Button _underLineText;
    [SerializeField] private GameObject _selectedUnderLine;
    [SerializeField] private Button _colorButton;
    [Space]
    [SerializeField] private Button _centerTextPositon;
    [SerializeField] private GameObject _selectedCenter;
    [SerializeField] private Button _leftTextPosition;
    [SerializeField] private GameObject _selectedLeftText;
    [SerializeField] private Button _rightTextPosition;
    [SerializeField] private GameObject _selectedRightText;

    [Header("FontSettingsPanel")]
    [SerializeField] private RectTransform _panelSettings;
    [SerializeField] private GameObject _downPanel;
    [SerializeField] private GameObject _upPanel;
    [SerializeField] private Transform _upPosition;
    [SerializeField] private Transform _downPosition;

    private Vector3 screenPosition;
    private float _offset = 150f;
    private bool _isMoveUp;
    private bool _isMoveDown;

    private List<string> _fontsOptionsDropdown = new List<string>();
    private List<string> _fontsSizeOptionsDropdown = new List<string>();

    private bool _colorMode;
    private bool _isBold;
    private bool _isItalic;
    private bool _isUnderLine;

    private void OnEnable()
    {
        _boldText.onClick.AddListener(SetBoldText);
        _italicText.onClick.AddListener(SetItalicText);
        _underLineText.onClick.AddListener(SetUnderLineText);

        _centerTextPositon.onClick.AddListener(CenterPositonText);
        _leftTextPosition.onClick.AddListener(LeftPositionText);
        _rightTextPosition.onClick.AddListener(RightPostionText);
        _colorButton.onClick.AddListener(OpenColorSettings);

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

        screenPosition = Camera.main.WorldToScreenPoint(Vector3.zero);
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

        _fontsOptionsDropdown.Clear();
        _fontsDropDown.ClearOptions(); 
    }

    private void Update()
    {
        if (_rectTransformText.anchoredPosition.y > (screenPosition.y - _offset) && !_isMoveUp)
        {
            MovePanelToPosition(_downPosition);
            _isMoveDown = false;
            _isMoveUp = true;

            _downPanel.SetActive(false);
            _upPanel.SetActive(true);
        }

        if (_rectTransformText.anchoredPosition.y < screenPosition.y - (Screen.height - _offset) && !_isMoveDown && _isMoveUp)
        {
            MovePanelToPosition(_upPosition);
            _isMoveUp = false;
            _isMoveDown = true;

            _downPanel.SetActive(true);
            _upPanel.SetActive(false);
        }
    }

    private void MovePanelToPosition(Transform target)
    {
        _panelSettings.DOMove(target.position, 0.2f);
    }
    public void OnSizeValueChanged(int value)
    {
        _currentText.fontSize = float.Parse(_fontSizeDropDown.options[value].text);
        _fontSizeDropDown.value = value;
    }

    public void OnFontValueChanged(int value)
    {
        _currentText.font = _fonts[value];
        _placeholderText.font = _fonts[value];
    }

    private void SetBoldText()
    {
        _isBold = !_isBold;
        _selectedBold.SetActive(_isBold);

        if (_isBold)
            _currentText.fontStyle = _currentText.fontStyle | FontStyles.Bold;
        else
            _currentText.fontStyle = _currentText.fontStyle -= FontStyles.Bold;

        _placeholderText.fontStyle = _currentText.fontStyle;
    }

    private void SetItalicText()
    {
        _isItalic = !_isItalic;
        _selectedItalic.SetActive(_isItalic);

        if (_isItalic)
            _currentText.fontStyle = _currentText.fontStyle | FontStyles.Italic;
        else
            _currentText.fontStyle = _currentText.fontStyle -= FontStyles.Italic;

        _placeholderText.fontStyle = _currentText.fontStyle;
    }

    private void SetUnderLineText()
    {
        _isUnderLine = !_isUnderLine;
        _selectedUnderLine.SetActive(_isUnderLine);

        if (_isUnderLine)
            _currentText.fontStyle = _currentText.fontStyle | FontStyles.Underline;
        else
            _currentText.fontStyle = _currentText.fontStyle -= FontStyles.Underline;

        _placeholderText.fontStyle = _currentText.fontStyle;
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

    public void SetColor(Color color)
    {
        _currentText.color = color;
    }

    public void OpenColorSettings()
    {
        _colorMode = !_colorMode;
        _flexibleColorPicker.gameObject.SetActive(_colorMode);
    }

    public void SetSizeInPanel(int size)
    {
        _fontSizeDropDown.captionText.text = size.ToString();
    }
}
