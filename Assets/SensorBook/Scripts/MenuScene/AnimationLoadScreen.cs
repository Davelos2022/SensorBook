using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationLoadScreen : MonoBehaviour
{
    public Sprite[] frames;
    public float frameRate = 10f;
    public Image ImageBox;

    private int currentFrameIndex;
    private bool isPlayingForward = false;
    private bool isPlayingBacklward = false;
    private float timer = 0f;


    private void OnEnable()
    {
        currentFrameIndex = 0;
        isPlayingForward = true;

    }

    void Update()
    {
        if (isPlayingForward && !isPlayingBacklward)
        {
            PlayAnimationForward();
        }


        if (isPlayingBacklward && !isPlayingForward)
        {
            PlayAnimationBackward();
        }
    }

    void PlayAnimationForward()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrameIndex++;

            if (currentFrameIndex >= frames.Length)
            {
                currentFrameIndex = frames.Length - 1;
                isPlayingForward = false;
            }

            ImageBox.sprite = frames[currentFrameIndex];
        }
    }

    void PlayAnimationBackward()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;

            currentFrameIndex--;

            if (currentFrameIndex < 0)
            {
                currentFrameIndex = 0;
                isPlayingForward = false;
                isPlayingBacklward = false;
            }

            ImageBox.sprite = frames[currentFrameIndex];
        }
    }
}
