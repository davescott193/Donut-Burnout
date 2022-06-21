using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{

    // m_ for member variables
    public float m_Spin;
    public float m_Tilt;

    public float m_Sensitivity = 1.0f;
    public Vector2 m_TiltExtents = new Vector2(30.0f, 40.0f);
    public Vector2 h_TiltExtents = new Vector2(-20.0f, 20.0f);
    public bool m_bCursorLocked = true;

    void LockCursor()
    {
        if (m_bCursorLocked)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    void Start()
    {
        LockCursor();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            m_bCursorLocked = !m_bCursorLocked;
            LockCursor();
        }

        // Preventing reading the code if not needed
        if (!m_bCursorLocked)
        {
            return;
        }

        // Obtaining how much the mouse has moved in a frame
        float x = Input.GetAxisRaw("Mouse X");
        float y = Input.GetAxisRaw("Mouse Y");

        m_Spin += x * m_Sensitivity;
        m_Tilt -= y * m_Sensitivity;

        m_Tilt = Mathf.Clamp(m_Tilt, m_TiltExtents.x, m_TiltExtents.y);
        m_Spin = Mathf.Clamp(m_Spin, h_TiltExtents.x, h_TiltExtents.y);

        transform.localEulerAngles = new Vector3(m_Tilt, m_Spin, 0.0f);
    }
}
