using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TrainConstructor.ReusableComponents
{
    public class ObjectResizerToParent : MonoBehaviour
    {
        private void Start()
        {
            Resize();
        }

        private void Resize()
        {
            RectTransform parent = transform.parent.GetComponent<RectTransform>();
            RectTransform rectTransform = GetComponent<RectTransform>();

            float parentHeight = parent.rect.height;

            //Bounds objectBounds = 
            //float objectHeight = rectTransform.rect.height;
            //float objectScale = rectTransform.localScale.x;

            //float newScale = parentHeight * objectScale / objectHeight;

            //rectTransform.localScale = new Vector3(newScale, newScale, newScale);

        }
    }
}