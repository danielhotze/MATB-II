using System.Collections;
using System.Collections.Generic;
using System.Linq; //For Toggle Group https://www.youtube.com/watch?v=0b6KmdPcDQU
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommunicationsTask : MonoBehaviour
{
    // Define Unity Game Objects
    ToggleGroup toggleGroupInstance;
    [SerializeField] Button submitButton;
    [SerializeField] TextMeshProUGUI changedText;
    [SerializeField] List<TextMeshProUGUI> comLabels;
    [SerializeField] GameObject CallSignContainer;
    [SerializeField] List<TextMeshProUGUI> CallSign;
    [SerializeField] List<Color32> Colors;

    // tasks is a list of integer lists containing values for 0:Event, 1:Frequency, 2:Start Time, 3:Timeout, 4:Status
    public static List<List<int>> tasks = new List<List<int>>();
    public Toggle currentSelection
    {
        get { return toggleGroupInstance.ActiveToggles().FirstOrDefault(); } //Returns the Active toggle 
    }

    // Task management helper variables
    private bool taskRunning = false;
    private int taskIndex = -1;
    private string taskChannel = "";
    private string taskFrequency = "";
    private string frequencyInput = "";

    // public int count = -1;

    // For connection to outside scripts (Serializer, Loading, Results)
    public Serializer serializer;
    public Loading loading;
    public static List<int> score = new List<int> { 0, 0 };

    public void OnChangedInputField(string input)
    {
        this.frequencyInput = input;
    }

    // Start is called before the first frame update
    void Start()
    {
        // count = tasks.Count;
        toggleGroupInstance = GetComponent<ToggleGroup>();
        // what channel ( 0, 1, 2, 3, 4{distractor})// what frequency*100 // at time (seconds) // timeout //

        //Start Coroutines 
        int i = 0;
        foreach (List<int> task in tasks)
        {
            tasks[i].Add('0');
            StartCoroutine(runCommunicationTask(task[0], task[1], task[2], task[3], i));
            i++;
        }
    }

    public void submitButtonClick()
    {
        if(!taskRunning)
        {
            return;
        }
        // Read channel and frequency inputs from GameObjects
        string selectedChannelStr = currentSelection.name;
        int selectedChannelNum;

        if(string.IsNullOrEmpty(frequencyInput)) { Debug.Log("ERROR: Input Frequency is null or empty"); }

        bool isParsable = int.TryParse(selectedChannelStr, out selectedChannelNum);

        setChannelFrequency(selectedChannelNum, frequencyInput, isParsable);

        if(!isParsable)
        {
            Debug.Log("Communication Task failed: Parsing Channel Number Error");
            completeTaskFailed();
            return;
        }

        if((taskChannel == "NAV 1" & selectedChannelNum == 0) | (taskChannel == "NAV 2" & selectedChannelNum == 1)
            | (taskChannel == "COM 1" & selectedChannelNum == 2) | (taskChannel == "COM 2" & selectedChannelNum == 3))
        {
            checkFrequency(taskFrequency, frequencyInput);
        } else if(taskChannel == "RES 1")
        {
            Debug.Log("Communication Task failed: Submitted while distractor");
            completeTaskFailed();
        } else
        {
            Debug.Log("Communication Task failed: Wrong Channel Input");
            completeTaskFailed();
        }
    }

    private void setChannelFrequency(int channelNum, string freq, bool isParsable)
    {
        if (isParsable) //false if error
        {
            comLabels[channelNum].text = freq;
        }
        else
        {
            Debug.Log("ERROR: make sure the name of toggles are an integer value.");
        }
    }

    private void checkFrequency(string taskFreq, string inputFreq) {
        Debug.Log("Submitted answer: " + inputFreq);
        Debug.Log("Correct solution: " + taskFreq);

        if ((taskFreq == inputFreq))
        {
            //correct answer
            completeTaskSuccess();
        }
        else
        {
            //wrong answer
            CallSignContainer.GetComponent<Image>().color = Colors[0];
            completeTaskFailed();
        }
    }

    IEnumerator runCommunicationTask(int channelNum, int freq, int startTime, int timeout, int id)
    {
        // Wait for task start time
        yield return new WaitForSeconds(startTime);

        // Check if there already is another task running -> ignore task in that case
        if(taskRunning == false)
        {
            taskRunning = true;
            taskIndex = id;
            // Set up task properties to display wanted channel and frequency
            setupTaskInstructions(channelNum, freq);

            // Wait for timeout and check if task is still running -> User has not fixed task
            yield return new WaitForSeconds(timeout);
            if(taskRunning == true & taskIndex == id)
            {
                completeTaskTimeout();
            }
        } else
        {
            yield break;
        }
    }

    private void setupTaskInstructions(int channelNum, int freq)
    {
        string frequency = freq.ToString();
        taskFrequency = frequency.Insert(3, ".");
        switch (channelNum) //Switch > IF ELSE (for this case), tabs > Spaces (always)
        {
            case 0:
                taskChannel = "NAV 1";
                break;
            case 1:
                taskChannel = "NAV 2";
                break;
            case 2:
                taskChannel = "COM 1";
                break;
            case 3:
                taskChannel = "COM 2";
                break;
            case 4:
                taskChannel = "RES 1";
                break;
            default:
                taskChannel = "RES 4";
                Debug.Log("ERROR: Wrong instruction.");
                break;
        }
        CallSign[0].text = taskChannel;
        CallSign[1].text = taskFrequency;
        CallSignContainer.GetComponent<Image>().color = Colors[0];
        Debug.Log("Communication Task added: " + taskChannel + ": " + taskFrequency);
    }

    private void completeTaskTimeout()
    {
        // check if Event was a Distractor event that requires no user interaction or if user was too slow
        if(tasks[taskIndex][0] == 4)
        {
            Debug.Log("Communication Task Success (Ignored): [" + taskChannel + "]: " + taskFrequency);

            // set task status to 2 -> "event correctly ignored"
            tasks[taskIndex][4] = 2;
            serializer.CommunicationsTaskAddRecord(tasks[taskIndex]);
            addTaskScore(true);

            // allow other tasks to fire
            CallSignContainer.GetComponent<Image>().color = Colors[1];
            taskRunning = false;
        } else
        {
            // set task status to 0 -> "event failed" (timeout)
            Debug.Log("Communication Task Fail (Timeout): [" + taskChannel + "]: " + taskFrequency);
            tasks[taskIndex][4] = 0;
            serializer.CommunicationsTaskAddRecord(tasks[taskIndex]);
            addTaskScore(false);

            // allow other tasks to fire
            CallSignContainer.GetComponent<Image>().color = Colors[0];
            taskRunning = false;
        }
    }

    private void completeTaskSuccess()
    {
        Debug.Log("Communication Task Success (User fix): [" + taskChannel + "]: " + taskFrequency);
        tasks[taskIndex][4] = 1;
        serializer.CommunicationsTaskAddRecord(tasks[taskIndex]);
        addTaskScore(true);

        // allow other tasks to fire
        CallSignContainer.GetComponent<Image>().color = Colors[1];
        taskRunning = false;
    }

    private void completeTaskFailed()
    {
        Debug.Log("Communication Task Fail (Wrong Input): [" + taskChannel + "]: " + taskFrequency);
        tasks[taskIndex][4] = 0;
        serializer.CommunicationsTaskAddRecord(tasks[taskIndex]);
        addTaskScore(false);

        // allow other tasks to fire
        CallSignContainer.GetComponent<Image>().color = Colors[0];
        taskRunning = false;
    }

    private void addTaskScore(bool success)
    {
        if(success)
        {
            score[0]++;
            score[1]++;
        } else
        {
            score[1]++;
        }
    }

}
