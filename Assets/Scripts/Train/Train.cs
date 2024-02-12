using UnityEngine;

namespace TrainConstructor.Train
{
    public class Train : MonoBehaviour
    {
        [SerializeField] private string id;
        [SerializeField] private bool isLockedWithAnAd;

        public string Id => id;
        public bool IsLockedWithAnAd => isLockedWithAnAd;

        public void Setup(string id, bool isLockedWithAnAd)
        {
            this.id = id;
            this.isLockedWithAnAd = isLockedWithAnAd;
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