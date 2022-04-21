using UnityEngine;

namespace Runtime
{
    public class FollowObject : MonoBehaviour
    {
        [SerializeField] private Transform target;
        private void LateUpdate()
        {
            if(target == null) { return; }
            
            Vector3 pos = target.position;
            pos.y = transform.position.y;
            transform.position = pos;
        }
    }
}