using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour {
    public GameObject Target; // Target to follow
    private float TargetDistance; // Distance between camera and target

    //Calculate Disstance between camera and target on X axis
    void Start () {
        TargetDistance = transform.position.x - Target.transform.position.x;
    }
	
	// Keep chaning x axis to the target
	void Update () {
        Vector3 NewCameraPos = transform.position;
        NewCameraPos.x = Target.transform.position.x + TargetDistance;
        transform.position = NewCameraPos;
    }
}
