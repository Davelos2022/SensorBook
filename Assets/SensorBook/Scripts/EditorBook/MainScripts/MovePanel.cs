using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MovePanel : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private GameObject _panelMove;
    [SerializeField] private GameObject _downPartPanel;
    [SerializeField] private GameObject _upPartPanel;
    [SerializeField] private Transform _upTargetPosition;
    [SerializeField] private Transform _downTargetPositon;

    //Setting
    private Vector3 _screenPosition;
    private float _offset = 150f;
    private bool _isMoveUp;
    private bool _isMoveDown;

    private void OnEnable()
    {
        _screenPosition = Camera.main.WorldToScreenPoint(Vector3.zero);
    }
    private void Update()
    {
        if (_rectTransform.anchoredPosition.y > (_screenPosition.y - _offset) && !_isMoveUp)
        {
            MovePanelToPosition(_downTargetPositon);
            _isMoveDown = false;
            _isMoveUp = true;

            _downPartPanel.SetActive(false);
            _upPartPanel.SetActive(true);
        }

        if (_rectTransform.anchoredPosition.y < _screenPosition.y - (Screen.height - _offset) && !_isMoveDown && _isMoveUp)
        {
            MovePanelToPosition(_upTargetPosition);
            _isMoveUp = false;
            _isMoveDown = true;

            _downPartPanel.SetActive(true);
            _upPartPanel.SetActive(false);
        }
    }

    private void MovePanelToPosition(Transform target)
    {
        _panelMove.transform.DOMove(target.position, 0.2f);
    }

}