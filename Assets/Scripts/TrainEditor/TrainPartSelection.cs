using System;
using TrainConstructor.TrainData;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrainConstructor.TrainEditor
{
    public class TrainPartSelection : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private Image image;

        private const float DISTANCE_TO_SELECT = 1.3f;

        public event Action<TrainPartSO> TrainPartSelected;

        private TrainPartSO trainPartSO;
        private bool spawned;
        private ScrollRect scrollRect;

        public void Setup(TrainPartSO _trainPartSO)
        {
            trainPartSO = _trainPartSO;
            image.sprite = _trainPartSO.MainTexture;

            scrollRect = GetComponentInParent<ScrollRect>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnDrag(PointerEventData _eventData)
        {
            if (spawned)
            {
                return;
            }

            Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float _distance = _mousePosition.y - transform.position.y;
            if (_distance > DISTANCE_TO_SELECT)
            {
                spawned = true;
                TrainPartSelected?.Invoke(trainPartSO);
            }
            else
            {
                ExecuteEvents.Execute(scrollRect.gameObject, _eventData, ExecuteEvents.dragHandler);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ExecuteEvents.Execute(scrollRect.gameObject, eventData, ExecuteEvents.endDragHandler);
            if (spawned)
            {
                spawned = false;
            }
        }
    }
}