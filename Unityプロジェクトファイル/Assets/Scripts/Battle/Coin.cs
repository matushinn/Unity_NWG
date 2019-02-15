using System.Collections.Generic;
using TSM;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using System.Collections;

namespace Battle
{
    [RequireComponent(typeof(Collider))]
    public class Coin : MonoBehaviour, IPauseable
    {
        [SerializeField, Range(1, 10)]
        public int value = 1;

        private float rotateYSpeed = 100f;

        public Transform childrenTransform;
        private SendOnTriggerEventToParent triggerEvent;

        private Rigidbody thisRigidbody;
        private Collider thisCollider;

        public List<MeshRenderer> bodyRendererList;

        public bool IsPaused { get; private set; }

        private void Awake()
        {
            thisRigidbody = GetComponentInParent<Rigidbody>();
            thisCollider = GetComponent<Collider>();

            triggerEvent = childrenTransform.gameObject.GetComponent<SendOnTriggerEventToParent>();
            triggerEvent.OnTriggerEnterEvent = this.OnTriggerEnter;

            bodyRendererList = GetComponentsInChildren<MeshRenderer>().ToList();

            BattleSceneController.Instance.AddPausable(this);

            IsPaused = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                SoundManager.Instance.PlaySE("GetCoin", 0.5f);

                BattleSceneController.Instance.AddCoin(value);

                this.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (IsPaused == false)
            {
                childrenTransform.Rotate(Vector3.up * rotateYSpeed * Time.deltaTime);
            }

            if(this.transform.position.y < -10f)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void OnPause()
        {
            IsPaused = true;
            thisRigidbody.isKinematic = true;
        }

        public void OnResume()
        {
            IsPaused = false;
            thisRigidbody.isKinematic = false;
        }

        public void Spawn(Vector3 position, int value)
        {
            gameObject.SetActive(true);

            transform.position = position + Vector3.up * 2f;
            this.value = value;

            thisCollider.enabled = true;
            thisRigidbody.isKinematic = false;

            thisRigidbody.velocity = Vector3.zero;
            thisRigidbody.angularVelocity = Vector3.zero;

            SoundManager.Instance.PlaySE("SpawnCoin", 0.5f);
            thisRigidbody.AddForce(Vector3.up * 1500f + Vector3.right * Random.Range(-1000f, 1000f));

            bodyRendererList.ForEach(b => b.enabled = true);

            StartCoroutine(PlayerGetEnableDelay());
        }

        IEnumerator PlayerGetEnableDelay()
        {
            float timeCount = 0f;

            while (timeCount < 1f)
            {
                if (!IsPaused)
                {
                    timeCount += Time.deltaTime;
                }

                yield return null;
            }

            triggerEvent.enabled = true;
        }

        public virtual void OnDisable()
        {
            thisCollider.enabled = false;
            thisRigidbody.isKinematic = true;
            triggerEvent.enabled = false;

            bodyRendererList.ForEach(b => b.enabled = false);

            thisRigidbody.velocity = Vector3.zero;
            thisRigidbody.angularVelocity = Vector3.zero;
        }
    }
}