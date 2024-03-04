using System;
using UnityEngine;

namespace TrainConstructor.TrainData
{
    public class DraggablePart : MonoBehaviour
    {
        [SerializeField] private TrainPart trainPart;

        private const float OFFSET = 0.0001f;

        public event Action<TrainPart> PartPutDown;

        private int order;
        private Camera mainCamera;

        private bool isDragging;
        private bool isFirstDrag;
        private bool isMouseOver;
        private Vector3 offsetFromCenter;

        private void Update()
        {
            if (isMouseOver && Input.mouseScrollDelta.y != 0)
            {
                if (Input.mouseScrollDelta.y > 0)
                {
                    order--;
                }
                else if (Input.mouseScrollDelta.y < 0)
                {
                    order++;
                }

                PlaceObjectInOrder();
                PartPutDown?.Invoke(trainPart);
            }

            if (isDragging)
            {
                Vector3 _mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
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

        private void OnMouseUp()
        {
            if (!isActiveAndEnabled)
                return;

            isDragging = false;
            PlaceObjectInOrder();
            PartPutDown?.Invoke(trainPart);
            trainPart.BoxCollider2D.enabled = true;
        }

        private void OnMouseDown()
        {
            if (!isActiveAndEnabled)
                return;

            isDragging = true;
            offsetFromCenter = transform.position - mainCamera.ScreenToWorldPoint(Input.mousePosition);
            trainPart.BoxCollider2D.enabled = false;
        }

        private void OnMouseEnter()
        {
            if (!isActiveAndEnabled)
                return;

            isMouseOver = true;
        }

        private void OnMouseExit()
        {
            if (!isActiveAndEnabled)
                return;

            isMouseOver = false;
        }

        public void Setup(TrainPart _trainPart, int _order, Camera _camera, bool _newPart)
        {
            trainPart = _trainPart;
            order = _order;
            mainCamera = _camera;

            if (!_newPart)
            {
                return;
            }

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

        private void PlaceObjectInOrder()
        {
            Vector3 _position = transform.position;
            _position.z = order;
            transform.position = _position;
        }
    }
}