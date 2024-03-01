using System.Collections.Generic;
using UnityEngine;

namespace TrainConstructor.TrainData
{
    public class Train : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private bool isLockedWithAnAd;
        [SerializeField] private Sprite snapshot;
        [SerializeField] private List<TrainPart> partList = new List<TrainPart>();

        public string Id => id;
        public bool IsLockedWithAnAd => isLockedWithAnAd;
        public Sprite Snapshot => snapshot;
        public List<TrainPart> PartList => partList;

        public void Setup(string _id, bool _isLockedWithAnAd, Sprite _snapshot)
        {
            id = _id;
            isLockedWithAnAd = _isLockedWithAnAd;
            snapshot = _snapshot;
        }

        public void AddPart(TrainPart _trainPart)
        {
            partList.Add(_trainPart);
        }

        public void RemovePart(TrainPart _trainPart)
        {
            partList.Remove(_trainPart);
        }

        public Bounds GetBounds()
        {
            Renderer[] _renderers = GetComponentsInChildren<Renderer>();
            if (_renderers.Length == 0)
            {
                Debug.LogError("No renderers found");
                return new Bounds();
            }

            Bounds _bounds = _renderers[0].bounds;
            for (int i = 1; i < _renderers.Length; i++)
            {
                _bounds.Encapsulate(_renderers[i].bounds);
            }

            return _bounds;
        }
    }
}