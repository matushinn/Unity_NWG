using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Battle
{

    public class JoyStick : Graphic, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Transform padObjectTransform;
        private float padMovableRadius = 100;
        private static Vector2 axis = Vector2.zero;

        public static float GetAxisHorizontal()
        {
            return axis.x;
        }

        public static float GetAxisVertical()
        {
            return axis.y;
        }

        protected override void Awake()
        {
            base.Awake();

            Image padImage = padObjectTransform.GetComponent<Image>();
            padImage.raycastTarget = false;

            raycastTarget = true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            OnEndDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            //CanvasがScreen Space - Overlayの場合のみ//
            Vector2 tapPosition = transform.InverseTransformPoint(eventData.position);

            //CanvasがScreen Space - Cameraなら、たぶんこう//
            //Vector2 tapPosition = Vector2.zero;
            //RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rectTransform, eventData.position, null, out tapPosition);

            //タップ位置の半径が指定半径より長かったら修正//
            if (Vector3.Distance(Vector3.zero, tapPosition) > padMovableRadius)
            {
                tapPosition = GetRadiusLimitedPosition(tapPosition, padMovableRadius);
            }

            SetPadObjectPosition(tapPosition);
        }

        //指定半径に収めた座標を返す//
        private Vector2 GetRadiusLimitedPosition(Vector2 tapPosition, float radius)
        {
            float radian = Mathf.Atan2(tapPosition.y, tapPosition.x);

            Vector2 limitedPosition = Vector2.zero;
            limitedPosition.x = radius * Mathf.Cos(radian);
            limitedPosition.y = radius * Mathf.Sin(radian);

            return limitedPosition;
        }

        //JoyStickのPad位置を設定//
        public void SetPadObjectPosition(Vector3 position)
        {
            padObjectTransform.localPosition = position;

            axis = new Vector2(
                padObjectTransform.localPosition.x / padMovableRadius,
                padObjectTransform.localPosition.y / padMovableRadius
            );
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            SetPadObjectPosition(Vector3.zero);
        }
    }
}