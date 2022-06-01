using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MechanicsManager : MonoBehaviour
{
    public Transform EntryPositionsTransform;
    public Transform CounterPositionsTransform;
    public Transform TablePositionsTransform;

    public GameObject CustomerPrefab;

    List<CustomerData> CustomerList = new List<CustomerData>();
    public float CustomerTimerFloat;
    public float CustomerThresholdFloat;

    public class CustomerData
    {
        public Transform CustomerTransform;
        public int CustomerStatusInt;
        public float WaitTimerFloat;
        public float WaitThresholdFloat;

    }

    private void Awake()
    {
        if (!GameManager.instance)
        {
            SceneManager.LoadScene(0);
        }
    }

    Transform ReturnRandomChild(Transform positionsTransform)
    {
        return positionsTransform.GetChild(Random.Range(0, positionsTransform.childCount));

    }
    void CreateCustomerVoid()
    {
        CustomerData customerData = new CustomerData();

        customerData.CustomerTransform = Instantiate(CustomerPrefab, ReturnRandomChild(EntryPositionsTransform).position, Quaternion.identity).transform;

        CustomerList.Add(customerData);
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.instance.ChangeScene(0);
        }

        CustomerTimerFloat += Time.deltaTime;

        if (CustomerTimerFloat >= CustomerThresholdFloat)
        {
            if (CustomerThresholdFloat > 0)
            {
                CustomerTimerFloat = 0;
                CreateCustomerVoid();
            }

            CustomerThresholdFloat = Random.Range(2, 6);

        }

        for (int i = 0; i < CustomerList.Count; i++)
        {
            if (CustomerList[i].CustomerStatusInt == 0 || Vector3.Distance(CustomerList[i].CustomerTransform.position, CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination) <= 1f)
            {
                CustomerList[i].WaitTimerFloat += Time.deltaTime;

                if (CustomerList[i].WaitTimerFloat >= CustomerList[i].WaitThresholdFloat)
                {
                    CustomerList[i].CustomerStatusInt++;
                    CustomerList[i].WaitThresholdFloat = Random.Range(0.5f, 3);
                    CustomerList[i].WaitTimerFloat = 0;

                    if (CustomerList[i].CustomerStatusInt == 1)
                    {
                        CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination = ReturnRandomChild(CounterPositionsTransform).position;
                    }

                    if (CustomerList[i].CustomerStatusInt == 2)
                    {
                        CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination = ReturnRandomChild(TablePositionsTransform).position;
                    }

                    if (CustomerList[i].CustomerStatusInt == 3)
                    {
                        CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination = ReturnRandomChild(EntryPositionsTransform).position;
                    }

                    if (CustomerList[i].CustomerStatusInt == 4)
                    {
                        Destroy(CustomerList[i].CustomerTransform.gameObject);
                        CustomerList.RemoveAt(i);
                    }
                }
            }
        }
    }
}
