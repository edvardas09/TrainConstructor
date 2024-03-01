using UnityEngine;

namespace TrainConstructor.ReusableComponents
{
    public class CameraViewWorldSpaceCanvas : MonoBehaviour
    {
        [SerializeField] private Camera targetCamera;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)transform;
        }

        private void Update()
        {
            float _cameraHeight = targetCamera.orthographicSize * 2;
            float _cameraWidth = targetCamera.aspect * _cameraHeight;

            rectTransform.sizeDelta = new Vector2(_cameraWidth, _cameraHeight);
        }
    }
}