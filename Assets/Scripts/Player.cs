using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    public float speed = 20;
    public float rotSpeed = 10;
    
    Rigidbody rigidbody;
    private static Vector3 defaultLocation = new Vector3(11.8f, 18.36f,0);
    private Transform eye;
 

	void Start () {
        rigidbody = GetComponent<Rigidbody>();
	}

    
    void FixedUpdate () {

        float rot = Input.GetAxis("Horizontal");
        float move = Input.GetAxis("Vertical");

        float rotY = rigidbody.rotation.eulerAngles.y;
        rotY += rotSpeed * rot;
        rigidbody.MoveRotation(Quaternion.Euler(0, rotY, 0));

        rigidbody.velocity = transform.forward * speed * move;
        //rigidbody.AddForce(transform.forward * speed * move);
        //Vector3 pos = rigidbody.position;
        //pos += transform.forward * move * speed;
        //rigidbody.MovePosition(pos);
	
    }
}
