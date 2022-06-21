using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderManager : MonoBehaviour
{
    public static OrderManager _instance = null;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        StartGenerateOrders();
    }


    private void StartGenerateOrders()
    {
        StartCoroutine(GenerateOrder());
    }

    private IEnumerator GenerateOrder()
    {
        while(true)
        {
            yield return new WaitForSeconds(2);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
