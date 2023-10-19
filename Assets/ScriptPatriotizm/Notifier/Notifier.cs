using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using VolumeBox.Toolbox;

public class Notifier : MonoBehaviour
{
    [SerializeField] private CanvasGroup fader;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text text;
    [SerializeField] private TMP_Text additionalText;
    [SerializeField] private Image image;
    [SerializeField] private List<NotifyTemplate> templates;

    private CancellationTokenSource _tokenSource;

    private static Notifier instance;
    private static object lockObject = new object();
    private static bool destroyed = false;

    public static bool HasInstance => instance != null;

    public static Notifier Instance
    {
        get
        {
            if (destroyed) return null;

            lock (lockObject)
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<Notifier>();

                    if (instance == null)
                    {
                        var singleton = new GameObject("[SINGLETON] " + typeof(Notifier));
                        instance = singleton.AddComponent<Notifier>();
                    }
                }
                return instance;
            }
        }
    }

    public static void DontDestroy()
    {
        DontDestroyOnLoad(Instance.gameObject);
    }

    private void OnDestroy()
    {
        destroyed = true;
    }

    public UnityEvent OnNotifyOpened;

    public void Notify(NotifyType type, string message, string additionalMessage = null, bool playSound = true)
    {
        _tokenSource?.Cancel();
        _tokenSource?.Dispose();
        _tokenSource = new CancellationTokenSource();

        fader.DOFade(1, 0.8f).WithCancellation(_tokenSource.Token);

        var t = templates.FirstOrDefault(x => x.type == type);

        additionalText.text = additionalMessage;
        text.text = message;

        image.sprite = t.background;
        icon.sprite = t.icon;

        OnNotifyOpened?.Invoke();

        if(playSound)
        {
            if(type == NotifyType.Error)
            {
                AudioPlayer.Instance.PlaySound("error");
            }
        }

        fader.DOFade(0, 2f).SetDelay(2).WithCancellation(_tokenSource.Token);
    }
}

[Serializable]
public class NotifyTemplate
{
    public NotifyType type;
    public Sprite background;
    public Sprite icon;
}

public enum NotifyType
{
    Error,
    Success,
    Default,
}
