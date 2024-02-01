using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class TrainPartsSelectionView : MonoBehaviour
    {
        [SerializeField] private float paddingFromSides = 0.5f;

        private void Awake()
        {
            Camera _camera = Camera.main;
            float _screenAspect = (float)Screen.width / (float)Screen.height;
            float _cameraHeight = _camera.orthographicSize * 2;
            float _newWidth = _cameraHeight * _screenAspect - paddingFromSides * 2;

            RectTransform _rectTransform = (RectTransform)transform;
            _rectTransform.sizeDelta = new Vector2(_newWidth, _rectTransform.sizeDelta.y);
        }
    }
}