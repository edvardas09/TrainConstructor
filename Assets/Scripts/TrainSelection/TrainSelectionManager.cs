using System.Collections.Generic;
using TrainConstructor.Train;
using UnityEngine;

namespace TrainConstructor.TrainSelection
{
    public class TrainSelectionManager : MonoBehaviour
    {
        [SerializeField] private TrainSelection trainSelectionPrefab;
        [SerializeField] private Transform trainSelectionParent;

        private List<Texture2D> snapshots = new List<Texture2D>();
        private List<Train.Train> trains = new List<Train.Train>();

        private void Awake()
        {
            snapshots = TrainDataManager.Instance.LoadSnapshots();
            trains = TrainDataManager.Instance.LoadCreatedTrains();

            SpawnTrainSelections();
        }

        private void SpawnTrainSelections()
        {
            foreach (Train.Train _train in trains)
            {
                TrainSelection _trainSelection = Instantiate(trainSelectionPrefab, trainSelectionParent);
                _trainSelection.Setup(_train, TrainDataManager.Instance.GetTrainSnapshot(_train.Id));
            }
        }
    }
}