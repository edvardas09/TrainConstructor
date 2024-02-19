using UnityEngine;

namespace TrainConstructor.ReusableComponents
{
    public class DragAndZoom : MonoBehaviour
    {
        [SerializeField] private float minScale = 0.5f;
        [SerializeField] private float maxScale = 2f;
        [SerializeField] private float scaleSpeed = 0.1f;

        private Vector3 offsetFromCenter;

        private void OnMouseDown()
        {
            offsetFromCenter = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void OnMouseDrag()
        {
            Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePosition += offsetFromCenter;
            _mousePosition.z = transform.position.z;
            transform.position = _mousePosition;
        }

        private void OnMouseOver()
        {
            if (Input.mouseScrollDelta.y != 0)
            {
                float _scale = transform.localScale.x + Input.mouseScrollDelta.y * scaleSpeed;
                _scale = Mathf.Clamp(_scale, minScale, maxScale);
                transform.localScale = new Vector3(_scale, _scale, 1);
            }
        }
    }
}