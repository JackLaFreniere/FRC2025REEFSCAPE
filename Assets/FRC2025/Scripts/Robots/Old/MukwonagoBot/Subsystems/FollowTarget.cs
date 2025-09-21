using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector3 localOffsetPos;
    private Quaternion localOffsetRot;

    void Start()
    {
        localOffsetPos = transform.localPosition - target.localPosition;
        localOffsetRot = Quaternion.Inverse(target.localRotation) * transform.localRotation;
    }

    void LateUpdate()
    {
        transform.SetLocalPositionAndRotation(target.localPosition + target.localRotation * localOffsetPos, target.localRotation * localOffsetRot);
    }
}
