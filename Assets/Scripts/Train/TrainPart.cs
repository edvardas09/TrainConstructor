using UnityEngine;

namespace TrainConstructor.Train
{
    public class TrainPart : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private BoxCollider2D boxCollider2D;
        [SerializeField, HideInInspector] private TrainPartSO trainPartSO;

        public TrainPartSO TrainPartSO => trainPartSO;
        public BoxCollider2D BoxCollider2D => boxCollider2D;

        public void Setup(TrainPartSO _trainPartSO)
        {
            trainPartSO = _trainPartSO;

            spriteRenderer.sprite = _trainPartSO.MainTexture;
            boxCollider2D.size = _trainPartSO.MainTexture.bounds.size;

            boxCollider2D.enabled = false;
        }

        public void ShowOutlineTexture()
        {
            spriteRenderer.sprite = trainPartSO.OutlineTexture;
        }

        public void ShowMainTexture()
        {
            spriteRenderer.sprite = trainPartSO.MainTexture;
        }
    }
}