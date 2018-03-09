using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    Camera m_sceneCamera;
    Transform m_cameraTransform;

    Vector3 m_previousCursosPosition;
    Vector3 m_currentCursosPosition;

    float m_movementSpeed = 10.0f;

	// Use this for initialization
	void Start () {
        m_sceneCamera = GetComponent<Camera>();
        m_cameraTransform = GetComponent<Transform>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            m_cameraTransform.Translate(new Vector3(0.0f, 0.0f, m_movementSpeed * Time.deltaTime), Space.Self);
        }

        if (Input.GetKey(KeyCode.S))
        {
            m_cameraTransform.Translate(new Vector3(0.0f, 0.0f, -m_movementSpeed * Time.deltaTime), Space.Self);
        }

        if (Input.GetKey(KeyCode.A))
        {
            m_cameraTransform.Translate(new Vector3(-m_movementSpeed * Time.deltaTime, 0.0f, 0.0f), Space.Self);
        }

        if (Input.GetKey(KeyCode.D))
        {
            m_cameraTransform.Translate(new Vector3(m_movementSpeed * Time.deltaTime, 0.0f, 0.0f), Space.Self);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            m_cameraTransform.Translate(new Vector3(0.0f, m_movementSpeed * Time.deltaTime, 0.0f), Space.Self);
        }

        if (Input.GetKey(KeyCode.X))
        {
            m_cameraTransform.Translate(new Vector3(0.0f, -m_movementSpeed * Time.deltaTime, 0.0f), Space.Self);
        }

        if (Input.GetMouseButton(1))
        {
            float h = 10.0f * Input.GetAxis("Mouse X");
            float v = 10.0f * Input.GetAxis("Mouse Y");

            m_cameraTransform.Rotate(-v, h, 0);
            m_cameraTransform.Rotate(0, 0, -transform.eulerAngles.z);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_movementSpeed += 10.0f;
        }

       else  if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            if (m_movementSpeed > 10.0f) m_movementSpeed -= 10.0f;
        }
    }
}
