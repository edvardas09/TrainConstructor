using System.Collections.Generic;
using UnityEngine;

namespace TrainConstructor.TrainData
{
    [CreateAssetMenu(fileName = "CreatedTrainsSO", menuName = "Train Constructor/CreatedTrainsSO", order = 0)]
    public class CreatedTrainsSO : ScriptableObject
    {
        public List<Train> CreatedTrains = new List<Train>();
    }
}