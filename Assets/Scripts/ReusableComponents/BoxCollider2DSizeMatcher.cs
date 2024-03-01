using UnityEngine;

namespace TrainConstructor.ReusableComponents
{
    public class BoxCollider2DSizeMatcher : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransformToMatchSize;
        [SerializeField] private BoxCollider2D boxCollider;

        private void Start()
        {
            if (rectTransformToMatchSize == null)
            {
                Debug.LogError("RectTransformToMatchSize is not set in " + gameObject.name);
                return;
            }

            if (boxCollider == null)
            {
                Debug.LogError("Collider is not set in " + gameObject.name);
                return;
            }

        }

        private void OnEnable()
        {
            boxCollider.size = rectTransformToMatchSize.rect.size;
        }
    }
}