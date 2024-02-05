using UnityEngine;

namespace TrainConstructor.ReusableComponents
{
    public class BoxCollider2DSizeMatcher : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransformToMatchSize;
        [SerializeField] private BoxCollider2D _collider;

        private void Start()
        {
            if (_rectTransformToMatchSize == null)
            {
                Debug.LogError("RectTransformToMatchSize is not set in " + gameObject.name);
                return;
            }

            if (_collider == null)
            {
                Debug.LogError("Collider is not set in " + gameObject.name);
                return;
            }

        }

        private void Update()
        {
            _collider.size = _rectTransformToMatchSize.rect.size;
        }
    }
}