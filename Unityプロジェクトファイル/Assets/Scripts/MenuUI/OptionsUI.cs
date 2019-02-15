using UnityEngine.UI;
using TSM;
using UnityEngine;

public class OptionsUI : UIBase
{
    public Slider volumeSlider;

    public override void Awake()
    {
        base.Awake();

        volumeSlider.onValueChanged.AddListener(VolumeChange);
    }

    public override void Show()
    {
        base.Show();

        volumeSlider.value = SoundManager.Instance.MasterVolumeInt;
    }

    public void VolumeChange(float value)
    {
        SoundManager.Instance.MasterVolumeInt = (int)value;

        SoundManager.Instance.PlayBGM("Dysipe_1_loop");
    }


}
