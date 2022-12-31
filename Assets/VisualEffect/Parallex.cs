using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallex : MonoBehaviour
{

    [SerializeField] private Vector3 ParallexFactor;
    Transform cameraTransform;

    
    Vector3 lastCameraPosition;

    Vector3 DeltaTravel ;

    void Awake(){
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }
    // Start is called before the first frame update
    void Start()
    {
        lastCameraPosition = cameraTransform.position; 
    }

    // Update is called once per frame
    void LateUpdate()
    {
        DeltaTravel =  cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(DeltaTravel.x * ParallexFactor.x , DeltaTravel.y * ParallexFactor.y ,DeltaTravel.z * ParallexFactor.z ) ;
        lastCameraPosition = cameraTransform.position;
    }
}
