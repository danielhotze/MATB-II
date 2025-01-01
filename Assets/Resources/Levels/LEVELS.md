## How to create level files:
Please note that I did not define the structure for the level-description json myself but follow the structure that was used in the reference project.
I would recommend to rework the json structure but this task would be out of scope for the seminar.

### Why create levels?
This project already contains 3 levels of difficulty, each with 5 minutes of tasks.
When you want a different timespan for your experiment or more control over the tasks that you want to execute, you should create a new level.

### How to use a level?
Either you directly specify which level to use in the Loading.cs Script, or you can send a SetLevel-Command from the browser to the unity instance with the help of the WebBridge.cs Script.

### JSON Structure for a level desciption
The level description consists of one big JSON-Object with lists of events for each of the four MATB-Tasks "System Monitoring", "Tracking", "Communication", and "Resource Management".
Additionally, one can specify "Tank Capacity", "Flow Rates", and "Tank Consumption" for the Resource Management Task.

#### 1. System Monitoring
```json
{
  "System_Monitoring_Tasks": [
    {"list": [element, eventTime, eventTimeout]},
    ...
  ],
}
```
The System Monitoring Task is described by a list of objects that contain a list called "list" (for whatever reason?!). <br>
<b>Element</b>: number from 1 to 6 representing which sysmon element is targeted by the event
1. 1-Scale
2. 2-Scale
3. 3-Scale
4. 4-Scale
5. 5-Button
6. 6-Button
<br>The key mapping is different to the "original" MATB-Task as there are conflicts of the F1-F10 key with default browser behavior such as page-refresh that are difficult to block from being executed.

<b>Event Time</b>: The time (in seconds) when the event is triggered. <br>
<b>Event Timeout</b>: The time (in seconds) for how long the user can complete the task until it is marked as "failed" and the normal system state is automatically recovered. <br>

#### 2. Tracking
```json
{
  , "Tracking_Tasks": [
    {"list": [eventTime, eventTimeout]},
    ...
  ],
}
```
<b>Event Time</b>: The time (in seconds) when the event is triggered and the tracking enters "manual" mode. <br>
<b>Event Timeout</b>: The time (in seconds) for how long the user can complete the task until it is marked as "failed" and the normal system state is automatically recovered. <br>

#### 3. Communication
```json
{
  "Communication_Tasks": [
    {"list": [channel, frequency, eventTime, eventTimeout]},
    ...
  ],
}
```
<b>Channel</b>: This number represents the Communication-Channel that the user has to select before submitting the signal. <br>
0. NAV1
1. NAV2
2. COM1
3. COM2
4. distractor - displays a "fake" channel name that the user must ignore to "succeed" the task

<b>Frequency</b>: A 6 figure number representing the frequency that the user must write into the input field of the communications task. <br>
<b>Event Time</b>: The time (in seconds) when the event is triggered and the tracking enters "manual" mode. <br>
<b>Event Timeout</b>: The time (in seconds) for how long the user can complete the task until it is marked as "failed" and the normal system state is automatically recovered. <br>

#### 4. Resource Management
For the resource management task, the user must balance the fuel between different tanks so that the two main tanks (A and B) always stay around a specified fuel level. <br>
This task can be configured using 3 different ways:

Firstly, one must define a list of pump-failure tasks that lead to a pump "breaking" and not transmitting any more fuel. <br>
The user then must click on the (now red) pump to "fix" it.
```json
{
  "Resource_Management_Tasks": [
    {"list": [pump, eventTime, eventTimeout]},
    ...
  ],
}
```
<b>Pump</b>: A number representing the pumps 1 to 8 (for whatever reason his representation was chosen - I am honestly starting to loose my mind because of how the reference project was written). <br> However, the pump with that references this number will then "break" and turn red and the user must click on that pump to fix it.
<b>Event Time</b>: The time (in seconds) when the event is triggered and the tracking enters "manual" mode. <br>
<b>Event Timeout</b>: The time (in seconds) for how long the user can complete the task until it is marked as "failed" and the normal system state is automatically recovered. <br>

Next, one must specify the tank capacity for the four not-unlimited tanks A, B, C and D. <br>
There is no need to specify tank capacity for the unlimited tanks E and F.
```json
{
  "Resource_Management_Tank_Capacity": [
    {"list": [maxCapacity, initialCapacity]}, // Tank A
    {"list": [maxCapacity, initialCapacity]}, // Tank B
    {"list": [maxCapacity, initialCapacity]}, // Tank C
    {"list": [maxCapacity, initialCapacity]}  // Tank D
  ],
}
```
<b>MaxCapacity</b> and <b>InitialCapacity</b> do what the names say. They define how much fuel can maximally fit into each tank and with which capacity they start at the beginning of the game.

Next, one must specify the Flow-Rate for the 8 pumps with how many units per minute they can transfer from one tank to another.
```json
{
  "Resource_Management_Flow_Rate": [
    800, // Pump 1
    600, // Pump 2
    800, // Pump 3
    600, // Pump 4
    600, // Pump 5
    600, // Pump 6
    400, // Pump 7
    400  // Pump 8
  ],
}
```

Lastly, one must specify the fuel consumption for the main tanks A and B in units per minute.
```json
{
  "Resource_Management_Tank_Consumption": [
    800, // Tank A
    800  // Tank B
  ],
}
```

### Template:
```json
{
  "System_Monitoring_Tasks": [
    {"list": [element, eventTime, eventTimeout]},
  ],
  "Tracking_Tasks": [
    {"list": [eventTime, eventTimeout]},
  ],
  "Communication_Tasks": [
    {"list": [channel, frequency, eventTime, eventTimeout]},
  ],
  "Resource_Management_Tasks": [
    {"list": [pump, eventTime, eventTimeout]},
  ],
  "Resource_Management_Tank_Capacity": [
    {"list": [maxCapacity, initialCapacity]},
    {"list": [maxCapacity, initialCapacity]},
    {"list": [maxCapacity, initialCapacity]},
    {"list": [maxCapacity, initialCapacity]}
  ],
  "Resource_Management_Flow_Rate": [
    800,
    600,
    800,
    600,
    600,
    600,
    400,
    400
  ],
  "Resource_Management_Tank_Consumption": [
    800,
    800
  ]
}
```