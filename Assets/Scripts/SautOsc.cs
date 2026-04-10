using UnityEngine;
using extOSC; 

public class SautOsc : MonoBehaviour
{
    public Rigidbody rb; 
    public float forceSaut = 5f;

    void Start()
    {
        OSCReceiver receiver = GetComponent<OSCReceiver>();

        receiver.Bind("/saut", FaireSauter);
    }

    void FaireSauter(OSCMessage message)
    {
        Debug.Log("Message OSC reçu : Saut !");

        rb.linearVelocity = Vector3.up * forceSaut;
    }
}