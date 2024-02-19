using UnityEngine;

namespace TrainConstructor.Train
{
    public class Train : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private bool isLockedWithAnAd;

        public string Id => id;
        public bool IsLockedWithAnAd => isLockedWithAnAd;

        public void Setup(string _id, bool _isLockedWithAnAd)
        {
            id = _id;
            isLockedWithAnAd = _isLockedWithAnAd;
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