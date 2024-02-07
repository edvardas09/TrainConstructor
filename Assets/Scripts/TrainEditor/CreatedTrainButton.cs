using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TrainConstructor.TrainEditor
{
    public class CreatedTrainButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI idText;
        [SerializeField] private Image snapshotImage;

        public Train.Train Train => train;

        private Train.Train train;

        public void Setup(Train.Train _train, Texture2D _snapshot)
        {
            train = _train;
            idText.text = _train.Id;

            if (_snapshot == null)
            {
                Debug.LogError($"Snapshot for train {_train.Id} does not exists");
                return;
            }

            snapshotImage.sprite = Sprite.Create(_snapshot, new Rect(0, 0, _snapshot.width, _snapshot.height), new Vector2(0.5f, 0.5f));
        }
    }
}