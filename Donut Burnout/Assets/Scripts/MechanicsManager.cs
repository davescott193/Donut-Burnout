using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class MechanicsManager : MonoBehaviour
{
    public static MechanicsManager instance;

    public List<CharacterMotor> characterMotors;
    public Transform EntryPositionsTransform;
    public Transform CounterPositionsTransform;
    public Transform TablePositionsTransform;
    public Transform PictureHolderTransform;
    public GameObject CustomerPrefab;
    public GameObject PlatePrefab;
    public GameObject PromptCanvasPrefab;
    public GameObject PictureMechanicsPrefab;

    public List<CustomerData> CustomerList = new List<CustomerData>();
    public float CustomerTimerFloat;
    public float CustomerThresholdFloat;

    [System.Serializable]
    public class CustomerData
    {
        public Transform CustomerTransform;
        public Transform PositionTransform;

        public Prompt CustomerPrompt;
        public Transform FoodTransform;
        public Transform PlateTransform;

        public int CustomerStatusInt;
        public int QueueInt;
        public int FoodTypeInt;
        public float WaitTimerFloat;
        public float WaitThresholdFloat;

    }

    private void Awake()
    {
        if (!GameManager.instance)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            instance = this;
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
        {
            return vacentTransformList[Random.Range(0, vacentTransformList.Count)];
        }
        else
        {
            return positionsTransform.GetChild(Random.Range(0, positionsTransform.childCount));
        }

    }
    void CreateCustomerVoid()
    {

        CustomerData customerData = new CustomerData();


        customerData.CustomerTransform = Instantiate(CustomerPrefab, ReturnRandomChild(EntryPositionsTransform).position, Quaternion.identity).transform;
        customerData.CustomerTransform.name = "Customer Number " + CustomerList.Count.ToString("00");
        GameManager.instance.SoundPool.PlaySound(GameManager.instance.CustomerEntersSound, 1, true, 0, false, customerData.CustomerTransform);
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
                if (CustomerList[i].PositionTransform)
                {
                    Quaternion rotationQuaternion = Quaternion.LookRotation(CustomerList[i].PositionTransform.forward);
                    CustomerList[i].CustomerTransform.rotation = Quaternion.Lerp(CustomerList[i].CustomerTransform.rotation, rotationQuaternion, 2 * Time.deltaTime);

                    // Debug.Log(Vector3.Dot(CustomerList[i].CustomerTransform.forward, CustomerList[i].PositionTransform.forward));
                    if (Vector3.Dot(CustomerList[i].CustomerTransform.forward, CustomerList[i].PositionTransform.forward) < 0.98f)
                        continue;

                }





                CustomerList[i].WaitTimerFloat += Time.deltaTime;

                if (CustomerList[i].WaitTimerFloat >= CustomerList[i].WaitThresholdFloat)
                {
                    int countInt = 0;

                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {

                        CustomerList[i].PositionTransform = ReturnRandomChild(CounterPositionsTransform);
                        CustomerList[i].WaitThresholdFloat = Random.Range(0.5f, 3);


                        for (int j = 0; j < CustomerList.Count; j++)
                        {
                            if (CustomerList[j].PositionTransform == CustomerList[i].PositionTransform && CustomerList[j].QueueInt > CustomerList[i].QueueInt)
                            {
                                CustomerList[i].PositionTransform = CustomerList[j].CustomerTransform;
                                CustomerList[i].QueueInt = CustomerList[j].QueueInt;
                            }
                        }

                        CustomerList[i].QueueInt++;
                    }

                    countInt++;
                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {
                        CustomerList[i].WaitThresholdFloat = 0;

                        if (CustomerList[i].QueueInt > 1)
                        {
                            UpdateDestination(i);
                            continue;
                        }
                    }

                    countInt++;
                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {
                        CustomerList[i].CustomerPrompt = Instantiate(PromptCanvasPrefab, CustomerList[i].CustomerTransform).GetComponent<Prompt>();
                        CustomerList[i].CustomerPrompt.transform.position = CustomerList[i].CustomerTransform.position + new Vector3(0, 2.4f, 0);
                        CustomerList[i].FoodTypeInt = Random.Range(0, GameManager.instance.FoodList.Count);
                        CustomerList[i].CustomerPrompt.CustomerData = CustomerList[i];
                        CustomerList[i].CustomerPrompt.CreatePicture();
                        CustomerList[i].CustomerPrompt.PromptText.text = GameManager.instance.FoodList[CustomerList[i].FoodTypeInt].name;
                        CustomerList[i].WaitThresholdFloat = 0;

                        GameManager.instance.SoundPool.PlaySound(GameManager.instance.CustomerOrdersSound, 1, true, 0, false, CustomerList[i].CustomerTransform);
                    }

                    countInt++;
                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {

                        bool isNearBool = false;
                        for (int j = 0; j < characterMotors.Count; j++)
                        {
                            if (Vector3.Distance(CustomerList[i].CustomerTransform.position, characterMotors[j].transform.position) <= 2)
                            {
                                isNearBool = true;
                                break;
                            }
                        }

                        if (!isNearBool)
                            continue;
                    }

                    countInt++;
                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {
                        Destroy(CustomerList[i].CustomerPrompt.gameObject);
                        GameManager.instance.SoundPool.PlaySound(GameManager.instance.CustomerReceivesSound, 1, true, 0, false, CustomerList[i].CustomerTransform);
                        for (int j = 0; j < CustomerList.Count; j++)
                        {
                            if (CustomerList[j].PositionTransform == CustomerList[i].CustomerTransform)
                            {
                                CustomerList[j].PositionTransform = CustomerList[i].PositionTransform;
                                CustomerList[j].QueueInt = 1;
                                break;
                            }
                        }

                        CustomerList[i].QueueInt = -1;
                        CustomerList[i].PlateTransform = Instantiate(PlatePrefab, CustomerList[i].CustomerTransform).transform;
                        CustomerList[i].PlateTransform.position += (CustomerList[i].CustomerTransform.forward * 0.5f) - Vector3.up;

                        CustomerList[i].FoodTransform = Instantiate(GameManager.instance.FoodList[CustomerList[i].FoodTypeInt], CustomerList[i].PlateTransform).transform;



                        CustomerList[i].PositionTransform = ReturnRandomChild(TablePositionsTransform);
                        CustomerList[i].WaitThresholdFloat = Random.Range(4f, 8);
                    }

                    countInt++;
                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {
                        CustomerList[i].PlateTransform.position += (CustomerList[i].CustomerTransform.forward * 0.5f);
                        CustomerList[i].PlateTransform.SetParent(null, true);
                        GameManager.instance.SoundPool.PlaySound(GameManager.instance.CustomerPlacesPlateDownSound, 1, true, 0, false, CustomerList[i].CustomerTransform);
                    }

                    countInt++;
                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {
                        Destroy(CustomerList[i].FoodTransform.gameObject);
                        GameManager.instance.SoundPool.PlaySound(GameManager.instance.CustomerEatsSound, 1, true, 0, false, CustomerList[i].CustomerTransform);
                        CustomerList[i].PositionTransform = ReturnRandomChild(EntryPositionsTransform);
                        CustomerList[i].WaitThresholdFloat = 0;
                    }

                    UpdateDestination(i);

                    countInt++;
                    if (CustomerList[i].CustomerStatusInt == countInt)
                    {
                        Destroy(CustomerList[i].CustomerTransform.gameObject);
                        CustomerList.RemoveAt(i);
                    }
                    else
                    {
                        CustomerList[i].CustomerStatusInt++;
                        CustomerList[i].WaitTimerFloat = 0;
                    }

                }
            }
        }
    }

    void UpdateDestination(int i)
    {
        CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination = CustomerList[i].PositionTransform.position;

        if (CustomerList[i].QueueInt > 1)
            CustomerList[i].CustomerTransform.GetComponent<NavMeshAgent>().destination -= CustomerList[i].PositionTransform.forward * 2f;

    }
}
