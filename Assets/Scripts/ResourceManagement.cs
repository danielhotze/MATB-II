using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManagement : MonoBehaviour
{
    public List<GameObject> pumpsFlows;
    public List<GameObject> tanks;
    public List<Button> buttons;
    public List<GameObject> tankLabel;
    public List<Color32> colors = new List<Color32>();

    public int count = -1;

    public Serializer serializer;
    public Loading loading;

    private float timer = 0f;

    public static List<List<int>> pumps = new List<List<int>>
    {   // From // To // active // flowrate
        new List<int> { 2, 0 , 0 , 800}, 
        new List<int> { 42, 0 , 0 , 600},
        new List<int> { 3, 1 , 0 , 800},
        new List<int> { 42, 1 , 0 , 600},
        new List<int> { 42, 2 , 0 , 600},
        new List<int> { 42, 3 , 0 , 600},
        new List<int> { 0, 1 , 0 , 400},
        new List<int> { 1, 0 , 0 , 400},
        new List<int> { 0, 42 , 1 , 800},
        new List<int> { 1, 42 , 1 , 800},
    };

    public static List<int> flowRates = new List<int>();
    public static List<List<int>> tankCapacity = new List<List<int>>(); // max capacity // current capacity // Consumption // 
    public static List<float> initialCapacity = new List<float> { 0f, 0f };

    // what channel pump // at time (seconds) // timeout //
    public static List<List<int>> tasks = new List<List<int>>();    

    // Start is called before the first frame update
    void Start()
    {
        count = tasks.Count;

        for (int z = 0; z < pumps.Count; z++)
        {
            pumps[z][3] = flowRates[z]; 
        }

        int i = 0;
        foreach (List<int> task in tasks)
        {
            tasks[i].Add('0');
            StartCoroutine(disablePumpSchedule(task[0], task[1], task[2], i));
            i++;
        }

        initialCapacity[0] = tankCapacity[0][1];
        initialCapacity[1] = tankCapacity[1][1];
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f / 2f)
        {
            changeTankValues();
            timer = 0;
        }
    }

    private void changeTankValues()
    {
        for(int i = 0; i < 10; i++)
        {
            if (pumps[i][2] == 1)
            {
                int temp = pumps[i][3] / 120;
                if(pumps[i][1] == 42)
                {
                    if (tankCapacity[pumps[i][0]][1] > temp) //checks if other tank has enough units
                    {
                        addAndSubtractfromTank(pumps[i][1], pumps[i][0], temp);
                    }
                    else
                    {
                        //Tank Empty
                    }
                }
                else
                {
                    if ((tankCapacity[pumps[i][1]][1] + temp) < tankCapacity[pumps[i][1]][0]) //checks max 
                    {
                        if (pumps[i][0] == 42) //infinite pump
                        {
                            addAndSubtractfromTank(pumps[i][1], pumps[i][0], temp);
                        }
                        else
                        {
                            if (tankCapacity[pumps[i][0]][1] > temp) //checks if other tank has enough units
                            {
                                addAndSubtractfromTank(pumps[i][1], pumps[i][0], temp);
                            }
                            else
                            {
                                closePump(i);
                            }
                        }
                    }
                    else
                    {
                        closePump(i);
                    }
                }
            }
        }
        serializer.ResourceManagementAddRecord();
    }

    void closePump(int number)
    {
        //Debug.Log("CLOSE PUMP CALLED"); 
        pumps[number][2] = 0;

        //change color & Flow label below
        buttons[number].GetComponent<Image>().color = colors[2];
        pumpsFlows[number].GetComponent<Text>().text = "0";
    }

    void addAndSubtractfromTank(int toTank, int fromTank, int value)
    {
        //if 42 -> infinite tank i.e. just add
        if(fromTank != 42)
        {
            tankCapacity[fromTank][1] -= value;
            
            tanks[fromTank].GetComponent<Slider>().value = (float)tankCapacity[fromTank][1] / (float)tankCapacity[fromTank][0];
            tankLabel[fromTank].GetComponent<Text>().text = tankCapacity[fromTank][1].ToString();
            //Debug.Log(tankCapacity[fromTank][1]);
        }
        if(toTank != 42)
        {
            tankCapacity[toTank][1] += value;

            tanks[toTank].GetComponent<Slider>().value = (float)tankCapacity[toTank][1] / (float)tankCapacity[toTank][0];
            tankLabel[toTank].GetComponent<Text>().text = tankCapacity[toTank][1].ToString();
            //Debug.Log(tankCapacity[toTank][1]);
        }
    }

    public void pumpTrigger(int number)
    {
        //change active status, change color, change flow label

        if(pumps[number][2] == 0)
        {
            pumps[number][2] = 1;
            buttons[number].GetComponent<Image>().color = colors[1];
            pumpsFlows[number].GetComponent<Text>().text = pumps[number][3].ToString();
        }
        else if (pumps[number][2] == 1)
        {
            pumps[number][2] = 0;
            buttons[number].GetComponent<Image>().color = colors[2];
            pumpsFlows[number].GetComponent<Text>().text = "0";
        }
    }

    IEnumerator disablePumpSchedule(int number, int startTime, int timeout, int id)
    {
        yield return new WaitForSeconds(startTime);
        pumps[number][2] = 2;
        buttons[number].GetComponent<Image>().color = colors[0];
        pumpsFlows[number].GetComponent<Text>().text = "0";

        yield return new WaitForSeconds(timeout);
        pumps[number][2] = 0;
        buttons[number].GetComponent<Image>().color = colors[2];
        pumpsFlows[number].GetComponent<Text>().text = "0";

        count--;
        if (count == 0)
        {
            loading.toReport();
        }
    }
}
