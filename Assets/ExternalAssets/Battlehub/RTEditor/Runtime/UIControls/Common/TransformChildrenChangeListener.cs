using UnityEngine;

namespace Battlehub.UIControls
{
    public delegate void TransformChildrenChanged();

    public class TransformChildrenChangeListener : MonoBehaviour
    {
        public event TransformChildrenChanged TransformChildrenChanged;

        private void OnTransformChildrenChanged()
        {
            if(TransformChildrenChanged != null)
            {
                TransformChildrenChanged();
            }
        }
    }

}

