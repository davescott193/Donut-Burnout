using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechanicsManager : MonoBehaviour
{
    public Transform EntryPositionsTransform;
    public Transform CounterPositionsTransform;
    public Transform TablePositionsTransform;

    public GameObject CustomerPrefab;

    List<CustomerData> CustomerList = new List<CustomerData>();
    public float NewCustomerTimerFloat;
    public float ThresholdFloat;

    public class CustomerData
    {
        public Transform CustomerTransform;
        public int CustomerStatusInt;
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
        NewCustomerTimerFloat += Time.deltaTime;

        if (NewCustomerTimerFloat >= ThresholdFloat)
        {
            if (ThresholdFloat > 0)
            {
                NewCustomerTimerFloat = 0;
                CreateCustomerVoid();
            }

            ThresholdFloat = Random.Range(1, 5);

        }

        for (int i = 0; i < CustomerList.Count; i++)
        {
            if (CustomerList[i].CustomerStatusInt == 0 || Vector3.Distance(CustomerList[i].CustomerTransform.position, CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination) <= 1f)
            {
                CustomerList[i].CustomerStatusInt++;

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
