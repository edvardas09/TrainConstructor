using System.Collections.Generic;
using UnityEngine;

namespace TrainConstructor.Train
{
    [CreateAssetMenu(fileName = "TrainPartType", menuName = "Train Constructor/Train Part Type", order = 0)]
    public class TrainPartTypeRelationsSO : ScriptableObject
    {
        public TrainPartType Type;
        public List<TrainPartSubtype> SubTypes;
    }
}