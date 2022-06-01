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
    public GameObject PlatePrefab;

    List<CustomerData> CustomerList = new List<CustomerData>();
    public float CustomerTimerFloat;
    public float CustomerThresholdFloat;

    public class CustomerData
    {
        public Transform CustomerTransform;
        public Transform PositionTransform;

        public Transform FoodTransform;
        public Transform PlateTransform;

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
        List<Transform> vacentTransformList = new List<Transform>();
        for (int i = 0; i < positionsTransform.childCount; i++)
        {
            bool vacentBool = true;

            for (int j = 0; j < CustomerList.Count; j++)
            {
                if (CustomerList[j].PositionTransform == positionsTransform.GetChild(i))
                {
                    vacentBool = false;
                    break;
                }
            }

            if (vacentBool)
                vacentTransformList.Add(positionsTransform.GetChild(i));
        }

        if (vacentTransformList.Count > 0)
            return vacentTransformList[Random.Range(0, vacentTransformList.Count)];
        else
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
            CustomerTimerFloat = 0;
            CreateCustomerVoid();


            CustomerThresholdFloat = Random.Range(3, 6);

        }

        for (int i = 0; i < CustomerList.Count; i++)
        {
            if (CustomerList[i].CustomerStatusInt == 0 || Vector3.Distance(CustomerList[i].CustomerTransform.position, CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination) <= 2)
            {
                CustomerList[i].WaitTimerFloat += Time.deltaTime;

                if (CustomerList[i].PositionTransform)
                {
                    Quaternion rotationQuaternion = Quaternion.LookRotation(CustomerList[i].PositionTransform.forward);
                    CustomerList[i].CustomerTransform.rotation = Quaternion.Lerp(CustomerList[i].CustomerTransform.rotation, rotationQuaternion, Time.deltaTime);
                }

                if (CustomerList[i].WaitTimerFloat >= CustomerList[i].WaitThresholdFloat)
                {
                    CustomerList[i].CustomerStatusInt++;
                    CustomerList[i].WaitTimerFloat = 0;

                    if (CustomerList[i].CustomerStatusInt == 1)
                    {
                        CustomerList[i].PositionTransform = ReturnRandomChild(CounterPositionsTransform);
                        CustomerList[i].WaitThresholdFloat = Random.Range(0.5f, 3);
                    }

                    if (CustomerList[i].CustomerStatusInt == 2)
                    {
                        CustomerList[i].PlateTransform = Instantiate(PlatePrefab, CustomerList[i].CustomerTransform).transform;
                        CustomerList[i].PlateTransform.position += (CustomerList[i].CustomerTransform.forward * 0.5f) - Vector3.up;

                        CustomerList[i].FoodTransform = Instantiate(GameManager.instance.FoodList[Random.Range(0, GameManager.instance.FoodList.Count)], CustomerList[i].PlateTransform).transform;

                        CustomerList[i].PositionTransform = ReturnRandomChild(TablePositionsTransform);
                        CustomerList[i].WaitThresholdFloat = Random.Range(4f, 8);
                    }

                    if (CustomerList[i].CustomerStatusInt == 3)
                    {
                        Destroy(CustomerList[i].FoodTransform.gameObject);
                        CustomerList[i].PlateTransform.SetParent(null, true);
                        CustomerList[i].PositionTransform = ReturnRandomChild(EntryPositionsTransform);
                        CustomerList[i].WaitThresholdFloat = 0;
                    }

                    CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination = CustomerList[i].PositionTransform.position;

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
