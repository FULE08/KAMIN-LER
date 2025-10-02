using System;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    Ray FindLength;
    [SerializeField] private int Speed = 5;
    [SerializeField] private float MaxCloseDistance = 4;
    [SerializeField] private float MaxFarDistance = 10;
    // Start is called before the first frame update
    void Start()
    {
        //transform.localEulerAngles = new Vector3(90,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        FindLength = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        float WS = Input.GetAxisRaw("Horizontal");
        float AD = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(WS, AD, 0).normalized;
        if (movement != Vector3.zero)
        {
            transform.Translate(movement * Speed * Time.deltaTime);
        }

        if (Physics.Raycast(FindLength, out hit))
        {
            float Distance = MathF.Round(hit.distance);
            //Debug.Log(Distance);
            if (Distance >= MaxCloseDistance && Distance <= MaxFarDistance)
            {
                if (Input.GetKey(KeyCode.E))
                {
                    transform.Translate(Vector3.forward * Speed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.Q))
                {
                    transform.Translate(Vector3.back * Speed * Time.deltaTime);
                }
            }
            else if (Distance <= MaxCloseDistance)
            {
                transform.Translate(Vector3.back * Speed * Time.deltaTime);
            }
            else if (Distance >= MaxFarDistance)
            {
                transform.Translate(Vector3.forward * Speed * Time.deltaTime);
            }
        }
    }
}