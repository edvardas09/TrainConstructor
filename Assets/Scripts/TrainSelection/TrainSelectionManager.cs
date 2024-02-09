using System.Collections.Generic;
using TrainConstructor.Train;
using UnityEngine;

namespace TrainConstructor.TrainSelection
{
    public class TrainSelectionManager : MonoBehaviour
    {
        [SerializeField] private TrainSelection trainSelectionPrefab;
        [SerializeField] private Transform trainSelectionParent;
        [SerializeField] private AdRewardCanvas adRewardCanvas;

        private List<Train.Train> trains = new List<Train.Train>();

        private void Awake()
        {
            TrainDataManager.Instance.LoadSnapshots();
            trains = TrainDataManager.Instance.LoadCreatedTrains();

            SpawnTrainSelections();
        }

        private void SpawnTrainSelections()
        {
            foreach (Train.Train _train in trains)
            {
                TrainSelection _trainSelection = Instantiate(trainSelectionPrefab, trainSelectionParent);
                Texture2D _snapshot = TrainDataManager.Instance.GetTrainSnapshot(_train.Id);
                _trainSelection.Setup(_train, _snapshot, adRewardCanvas);
            }
        }
    }
}