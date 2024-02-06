using TMPro;
using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class CreatedTrainButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI idText;

        private string id;

        public void Setup(string id)
        {
            this.id = id;
            idText.text = id;
        }
    }
}