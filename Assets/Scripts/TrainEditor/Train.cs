using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class Train : MonoBehaviour
    {
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
                float _scale = transform.localScale.x + Input.mouseScrollDelta.y * 0.1f;
                _scale = Mathf.Clamp(_scale, 0.5f, 2);
                transform.localScale = new Vector3(_scale, _scale, 1);
            }
        }
    }
}