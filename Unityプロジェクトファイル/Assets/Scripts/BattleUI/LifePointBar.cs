using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    public class LifePointBar : MonoBehaviour
    {
        public Text lifePointText;
        public Image lifePointBar;
        private float defaultLifeBarWidth;

        public Color green;
        public Color yellow;
        public Color red;

        public void Start()
        {
            defaultLifeBarWidth = lifePointBar.rectTransform.sizeDelta.x;
        }

        public void SetLifeBar(int lifePoint)
        {
            lifePointText.text = lifePoint.ToString();

            lifePointBar.rectTransform.localPosition = new Vector3(
                Mathf.InverseLerp(100, 0, lifePoint) * defaultLifeBarWidth,
                lifePointBar.rectTransform.localPosition.y,
                lifePointBar.rectTransform.localPosition.z);

            if (lifePoint > 70)
            {
                lifePointBar.color = green;
            }
            else if (lifePoint > 40)
            {
                lifePointBar.color = yellow;
            }
            else
            {
                lifePointBar.color = red;
            }
        }
    }
}