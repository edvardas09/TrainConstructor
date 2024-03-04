using System.Collections.Generic;
using TrainConstructor.TrainData;
using UnityEngine;

namespace TrainConstructor.TrainSelection
{
    public class TrainSelectionManager : MonoBehaviour
    {
        [SerializeField] private CreatedTrainsSO createdTrainsSO;
        [SerializeField] private TrainPartsSO trainPartsSO;
        [SerializeField] private TrainSelection randomTrain;

        [SerializeField] private TrainSelection trainSelectionPrefab;
        [SerializeField] private Transform trainSelectionParent;

        private List<Train> trains = new List<Train>();

        private void Awake()
        {
            TrainDataManager.Instance.SetCreatedTrainsSO(createdTrainsSO, trainPartsSO);
            trains = TrainDataManager.Instance.CreatedTrains;

            SpawnTrainSelections();
        }

        private void SpawnTrainSelections()
        {
            foreach (Train _train in trains)
            {
                TrainSelection _trainSelection = Instantiate(trainSelectionPrefab, trainSelectionParent);
                _trainSelection.Setup(_train);
            }

            randomTrain.transform.SetAsLastSibling();
        }
    }
}