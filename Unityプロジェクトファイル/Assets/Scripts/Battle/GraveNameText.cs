using UnityEngine;
using UnityEngine.UI;

namespace Battle
{
    [RequireComponent(typeof(Text))]
    [RequireComponent(typeof(Button))]
    public class GraveNameText : MonoBehaviour
    {
        private Transform textShowPointTransform;
        public float lifeTime = 3f;
        private float timer = 0f;
        private bool isPaused = false;

        private string deathMessage;
        private GraveObject relatedGraveObject;

        private Button thisButton;
        private Text thisText;

        private BattleSceneController battleSceneController;

        private void Awake()
        {
            thisText = GetComponent<Text>();
            thisText.enabled = false;

            thisButton = GetComponent<Button>();
            thisButton.onClick.RemoveAllListeners();

            battleSceneController = BattleSceneController.Instance;
            thisButton.onClick.AddListener(ShowThisGraveMessage);
        }

        private void Update()
        {
            if (!string.IsNullOrEmpty(thisText.text) && !isPaused)
            {
                UpdateTextPosition();
                timer += Time.deltaTime;
                if (timer > lifeTime)
                {
                    Clear();
                }
            }
        }

        public GraveObject ReratedGraveObject
        {
            get
            {
                if (relatedGraveObject != null)
                {
                    return relatedGraveObject;
                }

                return null;
            }
        }

        private void ShowThisGraveMessage()
        {
            battleSceneController.ShowSelectedGraveObjectsInfo(this);
        }

        public void ShowGraveNickNameAndSetRelation(Transform gravePointTransform, GraveObject grave)
        {
            textShowPointTransform = gravePointTransform;
            relatedGraveObject = grave;

            thisText.enabled = true;
            thisText.text = relatedGraveObject.graveInfo.nickName;

            UpdateTextPosition();

            this.enabled = true;
            thisButton.enabled = true;
        }

        public void Pause()
        {
            isPaused = true;
            thisButton.enabled = false;
        }

        public void Resume()
        {
            isPaused = false;
            thisButton.enabled = false;
        }

        public void Clear()
        {
            timer = 0f;

            relatedGraveObject = null;//nullにしないと参照が残り同じIDのメッセージが表示されなくなる//
            thisText.text = string.Empty;
            thisText.enabled = false; //空でも他のテキストが重なるとEventが取れなくなるので非表示にする//
            thisButton.enabled = false;
            this.enabled = false;
        }

        private void UpdateTextPosition()
        {
            if (textShowPointTransform != null)
            {
                thisText.rectTransform.position = RectTransformUtility.WorldToScreenPoint(Camera.main, textShowPointTransform.position);
            }
        }
    }
}