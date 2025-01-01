using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SystemMonitoring : MonoBehaviour
{
    public bool automationState;
    public static List<List<int>> tasks = new List<List<int>>();
    public List<GameObject> top = new List<GameObject>();
    public List<GameObject> bars = new List<GameObject>();
    public List<bool> barsRandom = new List<bool>();
    private List<int> dirs = new List<int>() { 1 , 1 , 1 , 1 };

    public int count = -1;

    //public Serializer serializer;
    public Loading loading;

    private float timer = 0f;

    public List<Color32> colors = new List<Color32>();

    public static List<int> score = new List<int> { 0, 0 };

    // Start is called before the first frame update
    void Start()
    {
        count = tasks.Count;
        for (int i = 0; i < tasks.Count; i++) {
            Debug.Log("[Sysmon] <Init Task> Btn/Scale: " + (tasks[i][0] + 1) + " | Start: " + tasks[i][1] + " | Timeout: " + tasks[i][2]);
            tasks[i].Add('0');
            if (tasks[i][0] < 4)
            {
                StartCoroutine(runBarTask(tasks[i][0], tasks[i][1], tasks[i][2], i));
            }
            else
            {
                StartCoroutine(runBtnTask(tasks[i][0], tasks[i][1], tasks[i][2], i));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            resetBar(0);
        }
        if (Input.GetKeyDown("2"))
        {
            resetBar(1);
        }
        if (Input.GetKeyDown("3"))
        {
            resetBar(2);
        }
        if (Input.GetKeyDown("4"))
        {
            resetBar(3);
        }
        if (Input.GetKeyDown("5"))
        {
            Color32 newCol = top[0].GetComponent<Image>().color;
            if (newCol.Equals(colors[4]))
            {
                top[0].GetComponent<Image>().color = colors[0];
            }
            else
            {
                top[0].GetComponent<Image>().color = colors[4];
            }
        }
        if (Input.GetKeyDown("6"))
        {
            Color32 newCol = top[1].GetComponent<Image>().color;
            if (newCol.Equals(colors[1]))
            {
                top[1].GetComponent<Image>().color = colors[4];
            }
            else
            {
                top[1].GetComponent<Image>().color = colors[1];
            }
        }

        timer += Time.deltaTime;

        if(timer >= 1f/10f)
        {
            randomMotion();
            timer = 0;
        }
    }

    public void acceptButtonClick(string btnNum)
    {
        if(btnNum == "5")
        {
            Color32 newCol = top[0].GetComponent<Image>().color;
            if (newCol.Equals(colors[4]))
            {
                top[0].GetComponent<Image>().color = colors[0];
            }
        } else if(btnNum == "6")
        {
            Color32 newCol = top[1].GetComponent<Image>().color;
            if (newCol.Equals(colors[1]))
            {
                top[1].GetComponent<Image>().color = colors[4];
            }
        }
    }

    IEnumerator runBtnTask(int btnIndex, int startTime, int timeout, int id)
    {
        yield return new WaitForSeconds(startTime);
        Debug.Log("[Sysmon] <Start Task> Btn: " + (btnIndex + 1) + " | Start: " + startTime + " | Timeout: " + timeout);
        //Do the changes 
        if (btnIndex == 4)
        {
            top[0].GetComponent<Image>().color = colors[4];
        }

        if (btnIndex == 5)
        {
            top[1].GetComponent<Image>().color = colors[1];
        }

        yield return new WaitForSeconds(timeout);
        //Check if human interacted
        //Change back to normal if not

        if (btnIndex == 4)
        {
            Color32 newCol = top[0].GetComponent<Image>().color;
            if (newCol.Equals(colors[4])) // Didn't fix automate
            {
                top[0].GetComponent<Image>().color = colors[0];
                tasks[id][3] = 0;
                //serializer.SystemMonitoringAddRecord(tasks[id]);
            }
            else
            {
                tasks[id][3] = 1;
                //serializer.SystemMonitoringAddRecord(tasks[id]);
                Debug.Log("User Fixed 5-Button");
            }
        }

        if (btnIndex == 5)
        {
            Color32 newCol = top[1].GetComponent<Image>().color;
            if (newCol.Equals(colors[1])) // Didn't fix automate
            {
                top[1].GetComponent<Image>().color = colors[4];
                tasks[id][3] = 0;
                //serializer.SystemMonitoringAddRecord(tasks[id]);
                score[1]++;
            }
            else
            {
                tasks[id][3] = 1;
                //serializer.SystemMonitoringAddRecord(tasks[id]);
                Debug.Log("User Fixed 6-Button");
                score[0]++;
                score[1]++;
            }
        }

        count--;
        if (count == 0)
        {
            loading.toReport();
        }
    }

    IEnumerator runBarTask(int barIndex, int startTime, int timeout, int id)
    {
        yield return new WaitForSeconds(startTime);
        //Do the changes 
        barsRandom[barIndex] = false;

        yield return new WaitForSeconds(timeout);
        Debug.Log("[Sysmon] <Start Task> Scale: " + (barIndex + 1) + " | Start: " + startTime + " | Timeout: " + timeout);
        //Check if human interacted
        //Change back to normal if not

        if (barsRandom[barIndex] == false)
        {
            barsRandom[barIndex] = true;
            tasks[id][3] = 0;
            //serializer.SystemMonitoringAddRecord(tasks[id]);
            score[1]++;
        }
        else
        {
            Debug.Log("User fixed (bars) " + (barIndex + 1));
            tasks[id][3] = 1;
            //serializer.SystemMonitoringAddRecord(tasks[id]);
            score[0]++;
            score[1]++;
        }
        count--;
        if (count == 0)
        {
            loading.toReport();
        }
    }

    void resetBar(int id)
    {
        barsRandom[id] = true;
    }

    void randomMotion()
    {
        for(int i=0; i<4; i++)
        {
            var val = bars[i].GetComponent<Slider>().value;
            if (barsRandom[i])
            {
                if (val >= 0.620) { dirs[i] = -1; }
                if (val <= 0.380) { dirs[i] = 1; }
            }
            bars[i].GetComponent<Slider>().value = val + (0.025f * dirs[i]);
        }
    }
}
