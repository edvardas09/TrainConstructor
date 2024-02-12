using System.Collections;
using System.Collections.Generic;
using TrainConstructor.Train;
using UnityEngine;

namespace TrainConstructor.Gameplay
{
    public class TrainPartOption : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private TrainPartSO trainPartSO;
        private Vector3 defaultScale;
        private Vector3 trainScale;

        public void Setup(TrainPartSO _trainPartSO, Vector3 _trainScale)
        {
            trainPartSO = _trainPartSO;
            trainScale = _trainScale;
            defaultScale = transform.localScale;

            spriteRenderer.sprite = _trainPartSO.MainTexture;
            boxCollider2D.size = _trainPartSO.MainTexture.bounds.size;
        }

        private void OnMouseDrag()
        {
            Vector3 _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _mousePosition.z = transform.position.z;
            transform.position = _mousePosition;
        }

        private void OnMouseUp()
        {
            //check if the part is dropped on the train
            RaycastHit2D _hit = Physics2D.Raycast(transform.position, Vector2.zero);
            if (_hit.collider != null)
            {
                TrainPart _trainPart = _hit.collider.GetComponent<TrainPart>();
                if (_trainPart != null)
                {
                    Debug.Log("Train part selected: " + _trainPart.TrainPartSO.name);
                    //_trainPart.Setup(trainPartSO);
                    //Destroy(gameObject);
                }
            }

            ReturnToDefaultPosition();
        }

        private void OnMouseDown()
        {
            ScaleToTrainSize();
        }

        private void ScaleToTrainSize()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, trainScale, 0.5f).setEase(LeanTweenType.easeOutBack);
        }

        private void ReturnToDefaultPosition()
        {
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, defaultScale, 0.5f).setEase(LeanTweenType.easeOutBack);
            LeanTween.moveLocal(gameObject, new Vector3(0, 0, transform.localPosition.z), 0.5f).setEase(LeanTweenType.easeOutBack);
        }
    }
}