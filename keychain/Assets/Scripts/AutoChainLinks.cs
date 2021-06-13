using UnityEngine;

public class AutoChainLinks : MonoBehaviour
{
    [SerializeField] private HingeJoint2D root;
    [SerializeField] private Rigidbody2D target;

    private void Awake()
    {
        var joints = GetComponentsInChildren<HingeJoint2D>();
        var rigidbodies = GetComponentsInChildren<Rigidbody2D>();

        // connect root
        root.connectedBody = rigidbodies[0];

        // link together child with next until last child
        for (var i = 0; i < joints.Length - 1; i += 1) joints[i].connectedBody = rigidbodies[i + 1];

        // connect target
        joints[joints.Length - 1].connectedBody = target;
    }
}