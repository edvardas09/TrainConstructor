using UnityEngine;

namespace TrainConstructor.ReusableComponents
{
    public class FullScreenWorldSpaceCanvas : MonoBehaviour
    {
        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        private void Update()
        {
            Camera _camera = Camera.main;
            float _screenAspect = (float)Screen.width / (float)Screen.height;
            float _cameraHeight = _camera.orthographicSize * 2;
            float _newWidth = _cameraHeight * _screenAspect;

            rectTransform.sizeDelta = new Vector2(_newWidth, _cameraHeight);
        }
    }
}