using UnityEngine;
using extOSC; 

public class JumpReceiver : MonoBehaviour
{
    public GameObject player; 
    public float jumpForce = 5f;

    void Start()
    {
        var receiver = GetComponent<OSCReceiver>();
        // On dit à Unity : "Si tu reçois le message /jump, lance la fonction Jump"
        receiver.Bind("/saut", OnJumpReceived);
    }

    void OnJumpReceived(OSCMessage message)
    {
        Debug.Log("Saut reçu !");
        player.GetComponent<Rigidbody2D>().AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }
}