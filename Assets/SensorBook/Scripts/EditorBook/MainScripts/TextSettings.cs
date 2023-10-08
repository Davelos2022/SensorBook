using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TextSettings : MonoBehaviour, IPointerClickHandler
{
    [Header ("Link to text")]
    [SerializeField] private TextMeshProUGUI _currentTextUP;
    [SerializeField] private RectResizer rect;


    [Header("Text Settings")]
    [SerializeField] private TMP_FontAsset[] _fonts;
    [SerializeField] private TMP_Dropdown _fontSizeUP;
    [SerializeField] private TMP_Dropdown _fontSizeDonw;
    [SerializeField] private TMP_Dropdown _fontsChouseUP;
    [SerializeField] private TMP_Dropdown _fontsChouseDown;

    [Header("UI Setting textPanel UP_panel")]
    [SerializeField] private Button _boldText;
    [SerializeField] private Button _italicText;
    [SerializeField] private Button _strikeThrouhgText;
    [SerializeField] private Button _centerText;
    [SerializeField] private Button _leftText;
    [SerializeField] private Button _rightText;

    [Header("UI Setting textPanel Down_panel")]
    [SerializeField] private Button _boldTextDown;
    [SerializeField] private Button _italicTextDonw;
    [SerializeField] private Button _strikeThrouhgTextDonw;
    [SerializeField] private Button _centerTextDonw;
    [SerializeField] private Button _leftTextDown;
    [SerializeField] private Button _rightTextDonw;

    [Header("FontSettings")]
    [SerializeField] private GameObject _upPanel;
    [SerializeField] private GameObject _downPanel;
    [SerializeField] private Vector2 _maxPosition;
    [SerializeField] private Vector2 _minPosition;
    [SerializeField] private float _minDistance;

    private List<string> optionsDropdownUP = new List<string>();
    private List<string> optionsDropdownDown = new List<string>();

    private void OnEnable()
    {
        _boldText.onClick.AddListener(SetBoldText);
        _italicText.onClick.AddListener(SetItalicText);
        _strikeThrouhgText.onClick.AddListener(SetCursivText);
        _fontSizeUP.onValueChanged.AddListener(OnSizeValueChanged);

        _centerText.onClick.AddListener(CenterPositonText);
        _leftText.onClick.AddListener(LeftPositionText);
        _rightText.onClick.AddListener(RightPostionText);

        _boldTextDown.onClick.AddListener(SetBoldText);
        _italicTextDonw.onClick.AddListener(SetItalicText);
        _strikeThrouhgTextDonw.onClick.AddListener(SetCursivText);
        _fontSizeDonw.onValueChanged.AddListener(OnSizeVelueDown);

        _centerTextDonw.onClick.AddListener(CenterPositonText);
        _leftTextDown.onClick.AddListener(LeftPositionText);
        _rightTextDonw.onClick.AddListener(RightPostionText);

        for (int x = 0; x < _fonts.Length; x++)
        {
            string option = _fonts[x].name.Substring(0, 7) + "....";
            optionsDropdownUP.Add(option);
            optionsDropdownDown.Add(option);
        }

        _fontsChouseUP.AddOptions(optionsDropdownUP);
        _fontsChouseDown.AddOptions(optionsDropdownDown);
    }


    private void OnDisable()
    {
        _boldText.onClick.RemoveListener(SetBoldText);
        _italicText.onClick.RemoveListener(SetItalicText);
        _strikeThrouhgText.onClick.RemoveListener(SetCursivText);
        _fontSizeUP.onValueChanged.RemoveListener(OnSizeValueChanged);
        _fontSizeDonw.onValueChanged.RemoveListener(OnSizeVelueDown);

        _centerText.onClick.RemoveListener(CenterPositonText);
        _leftText.onClick.RemoveListener(LeftPositionText);
        _rightText.onClick.RemoveListener(RightPostionText);

        optionsDropdownUP.Clear();
        _fontsChouseUP.ClearOptions();
    }

    public void OnSizeValueChanged(int value)
    {
        _currentTextUP.fontSize = float.Parse(_fontSizeUP.options[value].text);
        _fontSizeDonw.value = value;
    }

    public void OnSizeVelueDown(int value)
    {
        _currentTextUP.fontSize = float.Parse(_fontSizeDonw.options[value].text);
        _fontSizeUP.value = value;
    }

    public void OnFontValueChanged(int value)
    {
        _currentTextUP.font = _fonts[value];
    }

    private void SetBoldText()
    {
        if (_currentTextUP.fontStyle != FontStyles.Bold)
            _currentTextUP.fontStyle = FontStyles.Bold;
        else
            _currentTextUP.fontStyle = FontStyles.Normal;
    }

    private void SetItalicText()
    {
        if (_currentTextUP.fontStyle != FontStyles.Italic)
            _currentTextUP.fontStyle = FontStyles.Italic;
        else
            _currentTextUP.fontStyle = FontStyles.Normal;
    }

    private void SetCursivText()
    {
        if (_currentTextUP.fontStyle != FontStyles.Underline)
            _currentTextUP.fontStyle = FontStyles.Underline;
        else
            _currentTextUP.fontStyle = FontStyles.Normal;
    }

    private void LeftPositionText()
    {
        _currentTextUP.alignment = TextAlignmentOptions.MidlineLeft;
    }

    private void RightPostionText()
    {
        _currentTextUP.alignment = TextAlignmentOptions.MidlineRight;
    }

    private void CenterPositonText()
    {
        _currentTextUP.alignment = TextAlignmentOptions.Center;
    }

    public void SetColor(Color color)
    {
        _currentTextUP.color = color;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OpenSettings();    
    }

    public void OpenSettings()
    {
        if (transform.GetComponent<RectTransform>().anchoredPosition.y >= _maxPosition.y - _minDistance)
        {
            _upPanel.SetActive(false);
            _downPanel.SetActive(true);
        }
        else
        {
            _downPanel.SetActive(false);
            _upPanel.SetActive(true);
        }
    }
}
