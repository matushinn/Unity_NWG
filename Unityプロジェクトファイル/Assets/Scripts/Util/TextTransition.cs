using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TextTransition : MonoBehaviour
{
    private Text thisText;
    public string[] stringArray;
    public float spanTime = 0.5f;

    private void Awake()
    {
        thisText = GetComponent<Text>();
        if (stringArray.Length == 0) Debug.LogError("no text");
    }

    private void OnEnable()
    {
        thisText.enabled = true;
        StartCoroutine("SwitchTextString");
    }

    private IEnumerator SwitchTextString()
    {
        int i = 0;

        while (true)
        {
            thisText.text = stringArray[i];
            yield return new WaitForSeconds(spanTime);

            i++;

            if (i > stringArray.Length - 1)
            {
                i = 0;
            }
        }
    }

    private void OnDisable()
    {
        StopCoroutine("SwitchTextString");
        thisText.enabled = false;
    }
}