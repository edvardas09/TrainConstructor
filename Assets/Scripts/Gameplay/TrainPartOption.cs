using System;
using System.Collections.Generic;
using System.Linq;
using TrainConstructor.TrainData;
using UnityEngine;

namespace TrainConstructor.Gameplay
{
    public class TrainPartOption : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private const float ANIMATIONS_DURATION = 0.5f;
        private const float PADDING_MULTIPLIER = 0.7f;
        private const float RAYCAST_SIZE_MULTIPLIER = 0.5f;
        private const float DRAGGING_Z = -7f;

        public event Action<TrainPartOption> OnTrainPartReleased;

        public TrainPartSO TrainPartSO => trainPartSO;
        public BoxCollider2D BoxCollider2D => boxCollider2D;
        public TrainPart HoveredTrainPart => hoveredTrainPart;

        private TrainPartSO trainPartSO;
        private Vector3 defaultPosition;
        private Vector3 defaultScale;
        private Vector3 trainScale;
        private TrainPart hoveredTrainPart;

        private bool returnedToDefaultPosition = true;

        public void Setup(TrainPartSO _trainPartSO, Vector3 _trainScale, Camera _mainCamera)
        {
            trainPartSO = _trainPartSO;
            trainScale = _trainScale;
            mainCamera = _mainCamera;

            spriteRenderer.sprite = _trainPartSO.MainTexture;
            float _longerBound = Mathf.Max(_trainPartSO.MainTexture.bounds.size.x, _trainPartSO.MainTexture.bounds.size.y);
            boxCollider2D.size = new Vector2(_longerBound, _longerBound);

            hoveredTrainPart = null;

            ScaleToParentSize();
            defaultScale = transform.localScale;
        }

        public void HidePart()
        {
            spriteRenderer.enabled = false;
        }

        private void OnMouseDrag()
        {
            Vector3 _mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _mousePosition.z = DRAGGING_Z;
            transform.position = _mousePosition;

            TrainPart _newHoveredTrainPart = CheckForCorrectHit();
            if (_newHoveredTrainPart != null && _newHoveredTrainPart != hoveredTrainPart)
            {
                if (hoveredTrainPart != null)
                {
                    hoveredTrainPart.ShowOutlineTexture();
                }

                _newHoveredTrainPart.ShowMainTextureHint(trainPartSO.MainTexture);
                hoveredTrainPart = _newHoveredTrainPart;
            }
            else if (_newHoveredTrainPart == null && hoveredTrainPart != null)
            {
                hoveredTrainPart.ShowOutlineTexture();
                hoveredTrainPart = null;
            }
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

            if (returnedToDefaultPosition)
            {
                defaultPosition = transform.localPosition;
            }

            returnedToDefaultPosition = false;
        }

        private TrainPart CheckForCorrectHit()
        {
            List<RaycastHit2D> _hits = Physics2D.CapsuleCastAll(transform.position, boxCollider2D.size * transform.localScale * RAYCAST_SIZE_MULTIPLIER, CapsuleDirection2D.Horizontal, 0, Vector2.zero).ToList();
            if (_hits.Count == 0)
            {
                return null;
            }

            RaycastHit2D _hit = _hits.FirstOrDefault(x => x.collider.TryGetComponent<TrainPart>(out var _trainPart) && 
                _trainPart.TrainPartSO.Type == trainPartSO.Type && 
                _trainPart.TrainPartSO.SubType == trainPartSO.SubType);

            if (_hit.collider == null)
            {
                return null;
            }

            return _hit.collider.GetComponent<TrainPart>();
        }

        private void ScaleToParentSize()
        {
            transform.localScale = Vector3.one;

            Vector3 _bounds = trainPartSO.MainTexture.bounds.size / transform.localScale.x;
            Vector3 _parentBounds = transform.parent.GetComponent<RectTransform>().rect.size;

            float _biggerSide = Mathf.Max(_bounds.x, _bounds.y);

            float _newScale = _parentBounds.y / _biggerSide;
            _newScale *= PADDING_MULTIPLIER;

            transform.localScale = new Vector3(_newScale, _newScale, _newScale);
        }

        private void ScaleToTrainSize()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, trainScale, ANIMATIONS_DURATION).setEase(LeanTweenType.easeOutBack);
        }

        private void ReturnToDefaultPosition()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, defaultScale, ANIMATIONS_DURATION).setEase(LeanTweenType.easeOutBack);
            LeanTween.moveLocal(gameObject, new Vector3(defaultPosition.x, defaultPosition.y, defaultPosition.z), ANIMATIONS_DURATION)
                .setEase(LeanTweenType.easeOutBack)
                .setOnComplete(() =>
                {
                    spriteRenderer.enabled = true;
                    returnedToDefaultPosition = true;
                });
        }
    }
}