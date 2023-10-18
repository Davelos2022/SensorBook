using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VolumeBox.Toolbox;


[RequireComponent(typeof(CanvasGroup))]
public class DragObject : MonoBehaviour, IBeginDragHandler, IEventSystemHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [SerializeField] private Button _deletedObject;
    [SerializeField] private GameObject _selectedPanel;

    private Canvas _canvas;

    private RectTransform _rectTransform;
    private CanvasGroup _canvasGroup;
    private Vector3 _originalPosition;
    

    private void Awake()
    {
        Messager.Instance.Subscribe<SetActiveSelectionFrameMessage>(m =>
        {
            DisableSelection(m.exception != null && m.exception == this);

        }, gameObject.scene.name);


        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();

        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        _canvas = Object.FindObjectOfType<Canvas>();
    }

    private void OnEnable()
    {
        _deletedObject.onClick.AddListener(DeletedObject);
    }

    private void OnDisable()
    {
        DisableSelection(false);
        _deletedObject.onClick.RemoveListener(DeletedObject);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform.SetAsLastSibling();

        _canvasGroup.alpha = 0.7f;
        _canvasGroup.blocksRaycasts = false;

        _originalPosition = _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _rectTransform.anchoredPosition += eventData.delta / _canvas.scaleFactor;
    }

    private bool GetInsideScene()
    {
        var localCorners = new Vector3[4];
        _rectTransform.GetWorldCorners(localCorners);
        //var parentWorldRect = _parentTransform.GetWorldRect();
        var parentWorldRect = GetWorldRect(_rectTransform.parent.GetComponent<RectTransform>());
        return localCorners.All(corner => parentWorldRect.Contains(corner));
    }

    private Rect GetWorldRect(RectTransform rectTransform)
    {
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        var min = corners[0];
        var max = corners[2];
        var size = max - min;
        return new Rect(min, size);
    }

    public void MoveAround(PointerEventData eventData)
    {
        var anchoredPosition = _rectTransform.anchoredPosition;
        var oldPosition = anchoredPosition;
        var newPosition = anchoredPosition + eventData.delta / _canvas.scaleFactor;
        anchoredPosition = newPosition;

        _rectTransform.anchoredPosition = anchoredPosition;

        if (!GetInsideScene())
        {
            _rectTransform.anchoredPosition = oldPosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.alpha = 1f;

        SaveUndoStack();

        EditorBook.Instance.TakeScreenShotCurrentPage();
    }

    private void SaveUndoStack()
    {
        UndoRedoSystem.Instance.AddAction(new MoveObjectAction
            (_rectTransform, _originalPosition, _rectTransform.anchoredPosition));
    }

    private void DeletedObject()
    {
        EditorBook.Instance.DeletedObject(this.gameObject);
    }

    public void DisableSelection(bool active)
    {
        if (_selectedPanel == null)
            return;

        _selectedPanel.SetActive(active);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DisableSelection(true);
        Messager.Instance.Send(new SetActiveSelectionFrameMessage { exception = this });
    }
}

public class SetActiveSelectionFrameMessage : Message
{
    public DragObject exception;
}
