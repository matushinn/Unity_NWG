using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
[RequireComponent(typeof(RectTransform))]
public class TextFadeInOutAnimation : MonoBehaviour
{
    private Text text;
    public float fadeTime = 0.5f;
    public float moveUpSpeed = 250f;
    private Color originColor;
    private RectTransform rectTransform;
    private float originYPos;
    private bool isPlaying = false;

    private void Awake()
    {
        text = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();

        originColor = text.color;
        originYPos = rectTransform.position.y - (moveUpSpeed * fadeTime);

        SetAlpha(text, 0f);
    }

    public void Play(string str)
    {
        if (!isPlaying)
        {
            isPlaying = true;

            text.text = str;
            StartCoroutine(SlideUpFade());
        }
    }

    private IEnumerator SlideUpFade()
    {
        float alpha = 0f;
        SetAlpha(text, alpha);

        float yPos = originYPos;
        SetPos(rectTransform, yPos);

        while (alpha < 1f)
        {
            alpha += Time.deltaTime / fadeTime;
            SetAlpha(text, alpha);

            yPos += Time.deltaTime * moveUpSpeed;
            SetPos(rectTransform, yPos);

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (alpha >= 0f)
        {
            alpha -= Time.deltaTime / fadeTime;
            SetAlpha(text, alpha);

            yPos += Time.deltaTime * moveUpSpeed;
            SetPos(rectTransform, yPos);

            yield return null;
        }

        SetPos(rectTransform, originYPos);
        isPlaying = false;
    }

    private void SetPos(RectTransform rectTransform, float yPos)
    {
        rectTransform.position = new Vector2(rectTransform.position.x, yPos);
    }

    private void SetAlpha(Text text, float alpha)
    {
        text.color = new Color(originColor.r, originColor.g, originColor.b, alpha);
    }
}