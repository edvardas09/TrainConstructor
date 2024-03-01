using System.Collections.Generic;
using TrainConstructor.TrainData;
using UnityEngine;

namespace TrainConstructor.TrainSelection
{
    public class TrainSelectionManager : MonoBehaviour
    {
        [SerializeField] private CreatedTrainsSO createdTrainsSO;

        [SerializeField] private TrainSelection trainSelectionPrefab;
        [SerializeField] private Transform trainSelectionParent;

        private List<Train> trains = new List<Train>();

        private void Awake()
        {
            TrainDataManager.Instance.SetCreatedTrainsSO(createdTrainsSO);
            trains = TrainDataManager.Instance.CreatedTrains;

            SpawnTrainSelections();
        }

        private void SpawnTrainSelections()
        {
            foreach (Train _train in trains)
            {
                TrainSelection _trainSelection = Instantiate(trainSelectionPrefab, trainSelectionParent);
                _trainSelection.transform.SetAsFirstSibling();
                _trainSelection.Setup(_train);
            }
        }
    }
}