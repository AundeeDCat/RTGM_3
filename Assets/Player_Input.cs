using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.VFX;

public class Player_Input : MonoBehaviour
{
    public float detectionRange = 10f;
    public LayerMask layerMask;
    public KeyCode KeyCode_Throw = KeyCode.Space, KeyCode_Grab = KeyCode.E;
    public bool handsFull = false;

    public GameObject Space_Released, Space_Pressed, Crosshair_Target, Crosshair_Untarget, Hand_Position;

    // Start is called before the first frame update
    float mainSpeed = 5f; //regular speed
    float shiftAdd = 10.0f; //multiplied by how long shift is held.  Basically running
    float maxShift = 30.0f; //Maximum speed when holdin gshift
    float camSens = 0.25f; //How sensitive it with mouse
    private Vector3 lastMouse = new Vector3(255, 255, 255); //kind of in the middle of the screen, rather than at the top (play)
    private float totalRun = 1.0f;

    // Update is called once per frame
    void FixedUpdate()
    {
        Player_Movement();
        Pick_Up();

        //UI_Status();
    }

    private Vector3 GetBaseInput()
    { //returns the basic values, if it's 0 than it's not active.
        Vector3 p_Velocity = new Vector3();
        if (Input.GetKey(KeyCode.W))
        {
            p_Velocity += new Vector3(0, 0, 1);
        }
        if (Input.GetKey(KeyCode.S))
        {
            p_Velocity += new Vector3(0, 0, -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            p_Velocity += new Vector3(-1, 0, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            p_Velocity += new Vector3(1, 0, 0);
        }
        return p_Velocity;
    }

    private void Player_Movement()
    {
        lastMouse = Input.mousePosition - lastMouse;
        lastMouse = new Vector3(-lastMouse.y * camSens, lastMouse.x * camSens, 0);
        lastMouse = new Vector3(transform.eulerAngles.x + lastMouse.x, transform.eulerAngles.y + lastMouse.y, 0);
        transform.eulerAngles = lastMouse;



        lastMouse = Input.mousePosition;
        //Mouse camera angle done.  

        //Keyboard commands
        float f = 0.0f;
        Vector3 p = GetBaseInput();
        if (p.sqrMagnitude > 0)
        { // only move while a direction key is pressed
            if (Input.GetKey(KeyCode.LeftShift))
            {
                totalRun += Time.deltaTime;
                p = p * totalRun * shiftAdd;
                p.x = Mathf.Clamp(p.x, -maxShift, maxShift);
                p.y = Mathf.Clamp(p.y, -maxShift, maxShift);
                p.z = Mathf.Clamp(p.z, -maxShift, maxShift);
            }
            else
            {
                totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
                p = p * mainSpeed;
            }

            p = p * Time.deltaTime;
            Vector3 newPosition = transform.position;

            this.gameObject.transform.Translate(p);
            newPosition.x = this.gameObject.transform.position.x;
            newPosition.z = this.gameObject.transform.position.z;
            newPosition.y = 1.5f; // Constant, to prevent flying
            this.gameObject.transform.position = newPosition;

        }
    }

    private void Pick_Up()
    {
        RaycastHit hit;
        Ray ray = new Ray(this.gameObject.transform.position, this.gameObject.transform.forward);

        //detectionRange = 10f;

        //Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.forward * detectionRange, Color.red);

        //Vector3 Hand_Pos = Hand_Position.transform.position;

        if (Physics.Raycast(ray, out hit, detectionRange, layerMask))
        {
            //Debug.Log("ItemDetected" + hit.collider.gameObject.name);
            Debug.Log(handsFull);
             
            Crosshair_Target.SetActive(true);
            Crosshair_Untarget.SetActive(false);

            if (Input.GetKeyDown(KeyCode_Throw) && handsFull == true)
            {
                handsFull = false;
                detectionRange = 50f;

                hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                hit.collider.gameObject.transform.parent = null;

                hit.collider.gameObject.GetComponent<Rigidbody>().AddForce(this.gameObject.transform.forward * shiftAdd, ForceMode.Impulse);
            }

            else if (Input.GetKeyDown(KeyCode_Grab) && handsFull == false)
            {
                handsFull = true;
                detectionRange = 5f;

                hit.collider.gameObject.tag = "Item";
                hit.collider.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                hit.collider.gameObject.transform.parent = Hand_Position.gameObject.transform;

                hit.collider.gameObject.transform.DOLocalMove(new Vector3(0f, 0f, 0f), 0.25f);
            }
            //UI_Status();
        }

        else
        {
            Crosshair_Target.SetActive(false);
            Crosshair_Untarget.SetActive(true);

            //handsFull = false;
        }
    }

    private void UI_Status()
    {
        if (Input.GetKeyDown(KeyCode_Throw))
        {
            Space_Released.SetActive(false);
            Space_Pressed.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode_Throw))
        {
            Space_Released.SetActive(true);
            Space_Pressed.SetActive(false);
        }
    }
}
