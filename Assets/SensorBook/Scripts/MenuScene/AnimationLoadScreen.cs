using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationLoadScreen : MonoBehaviour
{
    [SerializeField] private Sprite[] _frames;
    [SerializeField] private float _frameRate = 10f;
    [SerializeField] private Image _ImageBox;

    private int _currentFrameIndex;
    private bool _isPlayingForward;
    private float _timer = 0f;


    private void OnEnable()
    {
        _currentFrameIndex = 0;
        _isPlayingForward = true;
    }

    void Update()
    {
        if (_isPlayingForward)
        {
            PlayAnimationForward();
        }
    }

    void PlayAnimationForward()
    {
        _timer += Time.deltaTime;

        if (_timer >= 1f / _frameRate)
        {
            _timer = 0f;
            _currentFrameIndex++;

            if (_currentFrameIndex >= _frames.Length)
            {
                _currentFrameIndex = _frames.Length - 1;
                _currentFrameIndex = 0;
            }

            _ImageBox.sprite = _frames[_currentFrameIndex];
        }
    }
}
