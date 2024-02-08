using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainConstructor.ReusableComponents
{
    public class FullScreenWorldSpaceCanvas : MonoBehaviour
    {
        private Canvas _canvas;
        private RectTransform _rectTransform;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _rectTransform = GetComponent<RectTransform>();
        }

        private void Update()
        {
            Camera _camera = Camera.main;
            float _screenAspect = (float)Screen.width / (float)Screen.height;
            float _cameraHeight = _camera.orthographicSize * 2;
            float _newWidth = _cameraHeight * _screenAspect;

            RectTransform _rectTransform = (RectTransform)transform;
            _rectTransform.sizeDelta = new Vector2(_newWidth, _cameraHeight);
        }
    }
}