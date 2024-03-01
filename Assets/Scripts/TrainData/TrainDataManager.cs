using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TrainConstructor.TrainData
{
    public class TrainDataManager : Singleton<TrainDataManager>
    {
        public CreatedTrainsSO CreatedTrainsSO { get; private set; }
        public TrainPartsSO TrainPartsSO { get; private set; }

        public List<TrainPartSO> TrainParts { get; private set; } = new List<TrainPartSO>();
        public List<Train> CreatedTrains { get; private set; } = new List<Train>();

        public Train SelectedTrain { get; private set; }
        public bool IsRandom { get; private set; }

        public void SetCreatedTrainsSO(CreatedTrainsSO _createdTrainsSO, TrainPartsSO _trainPartsSO)
        {
            CreatedTrainsSO = _createdTrainsSO;
            TrainPartsSO = _trainPartsSO;
            CreatedTrains = _createdTrainsSO.CreatedTrains;
            TrainParts = _trainPartsSO.TrainParts;
        }

        public void SetSelectedTrain(Train _train)
        {
            if (_train == null)
            {
                IsRandom = true;
                SelectedTrain = CreatedTrains[Random.Range(0, CreatedTrains.Count)];
                return;
            }

            IsRandom = false;
            SelectedTrain = _train;
        }

        public void SaveCreatedTrains()
        {
#if UNITY_EDITOR
            CreatedTrainsSO.CreatedTrains = CreatedTrains;
            EditorUtility.SetDirty(CreatedTrainsSO);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}