using System;
using TrainConstructor.Train;
using UnityEngine;

namespace TrainConstructor.Gameplay
{
    public class TrainPartOption : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private SpriteRenderer spriteRenderer;

        public Action<TrainPartOption> OnTrainPartSelected;
        public Action<TrainPartOption> OnTrainPartReleased;

        public TrainPartSO TrainPartSO => trainPartSO;

        private TrainPartSO trainPartSO;
        private Vector3 defaultScale;
        private Vector3 trainScale;

        private void Awake()
        {
            defaultScale = transform.localScale;
        }

        public void Setup(TrainPartSO _trainPartSO, Vector3 _trainScale)
        {
            trainPartSO = _trainPartSO;
            trainScale = _trainScale;

            spriteRenderer.sprite = _trainPartSO.MainTexture;
            boxCollider2D.size = _trainPartSO.MainTexture.bounds.size;
        }

        private void OnMouseDrag()
        {
            Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePosition.z = transform.position.z;
            transform.position = _mousePosition;
        }

        private void OnMouseUp()
        {
            OnTrainPartReleased?.Invoke(this);
            boxCollider2D.enabled = true;
            ReturnToDefaultPosition();
        }

        private void OnMouseDown()
        {
            ScaleToTrainSize();
            boxCollider2D.enabled = false;
            OnTrainPartSelected?.Invoke(this);
        }

        private void ScaleToTrainSize()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, trainScale, 0.5f).setEase(LeanTweenType.easeOutBack);
        }

        private void ReturnToDefaultPosition()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, defaultScale, 0.5f).setEase(LeanTweenType.easeOutBack);
            LeanTween.moveLocal(gameObject, new Vector3(0, 0, transform.localPosition.z), 0.5f)
                .setEase(LeanTweenType.easeOutBack)
                .setOnComplete(() => gameObject.SetActive(true));
        }
    }
}