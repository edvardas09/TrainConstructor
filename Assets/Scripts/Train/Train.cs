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
    }
}