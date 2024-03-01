using UnityEngine;

namespace TrainConstructor.TrainData
{
    public class TrainPartSO : ScriptableObject
    {
        public string Id;
        public TrainPartType Type;
        public TrainPartSubtype SubType;
        public Sprite MainTexture;
        public Sprite OutlineTexture;

        public void Setup(string _id, TrainPartType _type, TrainPartSubtype _subType, Sprite _mainTexture, Sprite _outlineTexture)
        {
            Id = _id;
            Type = _type;
            SubType = _subType;
            MainTexture = _mainTexture;
            OutlineTexture = _outlineTexture;
        }
    }
}