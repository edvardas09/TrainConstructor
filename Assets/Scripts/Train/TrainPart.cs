using System.Collections.Generic;
using System.Linq;
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

        public void Randomize()
        {
            if (trainPartSO == null)
            {
                Debug.LogError("No train part SO assigned");
                return;
            }

            List<TrainPartSO> _availableParts = TrainDataManager.Instance.TrainParts.Where(x => x.Type == trainPartSO.Type).ToList();
            if (_availableParts.Count == 0)
            {
                Debug.LogError("No available parts found");
                return;
            }

            TrainPartSO _randomPart = _availableParts[Random.Range(0, _availableParts.Count)];
            Setup(_randomPart);
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