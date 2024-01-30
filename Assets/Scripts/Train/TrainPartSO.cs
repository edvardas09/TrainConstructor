using UnityEngine;

namespace TrainConstructor.Train
{
    public class TrainPartSO : ScriptableObject
    {
        public string Id;
        public TrainPartType Type;
        public TrainPartSubtype SubType;
        public Sprite MainTexture;
        public Sprite OutlineTexture;
    }
}