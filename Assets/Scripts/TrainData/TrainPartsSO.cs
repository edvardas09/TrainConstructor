using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TrainConstructor.TrainData
{
    [CreateAssetMenu(fileName = "TrainPartsSO", menuName = "Train Constructor/TrainPartsSO", order = 0)]
    public class TrainPartsSO : ScriptableObject
    {
        public List<TrainPartSO> TrainParts = new List<TrainPartSO>();

        public void AddPart(TrainPartSO _trainPartSO)
        {
            TrainParts.Add(_trainPartSO);

#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
#endif
        }
    }
}