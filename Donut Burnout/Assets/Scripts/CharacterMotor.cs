using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    // Headers for the unity editor for visibility
    [Header("Attached Components")]
    public CharacterController m_controller;
    public MouseLook m_look;

    [Header("Motion Values")]
    public float m_movespeed = 8.0f;
    public float m_gravity = 32.0f;
    public float m_jumpspeed = 10.0f;

    [Header("Current State")]
    public Vector3 m_velocity = new Vector3(0.0f, 0.0f, 0.0f);
    public bool m_grounded = false;

    void Update()
    {
        float x = 0.0f;
        if (Input.GetKey(KeyCode.A))
        {
            x -= 1.0f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x += 1.0f;
        }

        float z = 0.0f;
        if (Input.GetKey(KeyCode.S))
        {
            z -= 1.0f;
        }
        if (Input.GetKey(KeyCode.W))
        {
            z += 1.0f;
        }

        if (Input.GetKey(KeyCode.Space) && m_grounded)
        {
            m_velocity.y = m_jumpspeed;
            m_grounded = false;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            z += 1.1f;
        }

        Vector3 inputMove = new Vector3(x, 0.0f, z);
        // Making sure that you go where you're looking, changing global z and x to local z and x
        inputMove = Quaternion.Euler(0.0f, m_look.m_Spin, 0.0f) * inputMove;

        m_velocity.x = inputMove.x * m_movespeed;
        m_velocity.y -= m_gravity * Time.deltaTime;
        m_velocity.z = inputMove.z * m_movespeed;

        m_controller.Move(m_velocity * Time.deltaTime);

        if ((m_controller.collisionFlags & CollisionFlags.Below) != 0)
        {
            m_velocity.y = -1.0f;
            m_grounded = true;
        }
        else
        {
            m_grounded = false;
        }

        if ((m_controller.collisionFlags & CollisionFlags.Above) != 0)
        {
            m_velocity.y = -1.0f;
        }

    }
}