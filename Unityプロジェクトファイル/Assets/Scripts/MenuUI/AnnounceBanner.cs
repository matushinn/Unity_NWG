using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class AnnounceBanner : MonoBehaviour
{
    public Text title;
    public Text date;
    public Button button;

    public void SetBanner(AnnouncementInfo info, UnityAction buttonAction)
    {
        title.text = info.title;
        date.text = info.date.ToString();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(buttonAction);
    }
}
