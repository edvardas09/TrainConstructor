using System;
using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class DeletePartObject : MonoBehaviour
    {
        [SerializeField] private float hoverScale = 1.1f;
        [SerializeField] private float hoverScaleTime = 0.2f;

        public Action MouseOverStateChanged;

        private void OnMouseEnter()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, Vector3.one * hoverScale, hoverScaleTime);
            MouseOverStateChanged?.Invoke();
        }

        private void OnMouseExit()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, Vector3.one, hoverScaleTime);
            MouseOverStateChanged?.Invoke();
        }
    }
}