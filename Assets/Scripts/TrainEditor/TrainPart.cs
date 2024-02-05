using System;
using TrainConstructor.Train;
using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class TrainPart : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider2D;

        private const float OFFSET = 0.0001f;

        public Action<TrainPart> PartPutDown;

        private TrainPartSO trainPartSO;
        private int order;

        private bool isDragging;
        private bool isFirstDrag;
        private Vector3 offsetFromCenter;

        private void Update()
        {
            if (isDragging)
            {
                Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _mousePosition += offsetFromCenter;
                _mousePosition.z = 0;
                transform.position = _mousePosition;
            }

            if (!isFirstDrag)
            {
                return;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isFirstDrag = false;
                OnMouseUp();
            }
        }

        public void Setup(TrainPartSO _trainPartSO, int _order)
        {
            trainPartSO = _trainPartSO;
            order = _order;

            spriteRenderer.sprite = _trainPartSO.MainTexture;
            boxCollider2D.size = _trainPartSO.MainTexture.bounds.size;

            isDragging = true;
            isFirstDrag = true;
            boxCollider2D.enabled = false;
        }

        public void OffsetPart(int _offsetMultiplier)
        {
            Vector3 _position = transform.position;
            _position.z -= _offsetMultiplier * OFFSET;
            transform.position = _position;
        }

        private void OnMouseUp()
        {
            isDragging = false;
            PlaceObjectInOrder();
            PartPutDown?.Invoke(this);
            boxCollider2D.enabled = true;
        }

        private void PlaceObjectInOrder()
        {
            Vector3 _position = transform.position;
            _position.z = order;
            transform.position = _position;
        }

        private void OnMouseDown()
        {
            isDragging = true;
            offsetFromCenter = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            boxCollider2D.enabled = false;
        }
    }
}