using UnityEngine;

namespace TrainConstructor.TrainEditor
{
    public class SnapshotButton : MonoBehaviour
    {
        private void OnMouseDown()
        {
            TrainEditor.Instance.TakeSnapshot();
        }
    }
}