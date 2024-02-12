using System;
using UnityEngine;

namespace TrainConstructor.Train
{
    public class DraggablePart : MonoBehaviour
    {
        private const float OFFSET = 0.0001f;

        public Action<TrainPart> PartPickedUp;
        public Action<TrainPart> PartPutDown;

        private TrainPart trainPart;
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

        public void Setup(TrainPart _trainPart, int _order)
        {
            trainPart = _trainPart;
            order = _order;

            isDragging = true;
            isFirstDrag = true;
        }

        public void OffsetPart(int _offsetMultiplier)
        {
            Vector3 _position = transform.position;
            _position.z -= _offsetMultiplier * OFFSET;
            transform.position = _position;
        }

        public void SetOrder(int _order)
        {
            order = _order;
        }

        private void OnMouseUp()
        {
            isDragging = false;
            PlaceObjectInOrder();
            PartPutDown?.Invoke(trainPart);
            trainPart.BoxCollider2D.enabled = true;
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
            trainPart.BoxCollider2D.enabled = false;
            PartPickedUp?.Invoke(trainPart);
        }
    }
}