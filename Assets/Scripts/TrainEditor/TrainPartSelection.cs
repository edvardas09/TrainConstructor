using System;
using TrainConstructor.Train;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TrainConstructor.TrainEditor
{
    public class TrainPartSelection : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private Image image;

        public Action<TrainPartSO> TrainPartSelected;

        private TrainPartSO trainPartSO;

        public void Setup(TrainPartSO _trainPartSO)
        {
            trainPartSO = _trainPartSO;
            image.sprite = _trainPartSO.MainTexture;
        }

        public void OnPointerDown(PointerEventData _eventData)
        {
            TrainPartSelected?.Invoke(trainPartSO);
        }
    }
}