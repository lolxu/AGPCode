using System;
using System.Collections.Generic;
using __OasisBlitz.__Scripts.Player.Environment.Checkpoints;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum MetricAction {
    Death,
    DrillRequest,
    Blast,
    Dash,
    Slide,
    Jump,
    
    // no need to specify checkpoint when trigger
    ActivateCheckpoint,
    GetPlant,
    LevelStart,
    LevelEnd,
    CutsceneSkip,
    DrillIn,
    DrillOut,
    
    // need specify ActionData
    ControllerChange,
}

public class InLevelMetrics : MonoBehaviour {
    private static InLevelMetrics _instance;
    public static InLevelMetrics Instance
    {
        get
        {
            if (_instance == null)
            {
                // Debug.LogError("InLevelMetrics is NULL.");   // john comment (it should be fine if it doesn't exist
                        // the code structure is wrong here because the game should not depend on metrics, it should
                            // be the other way around.
            }
            return _instance;
        }
    }

    public List<SectionStats> sections;
    [Tooltip("Set the first checkpoint here")]
    public CheckPoint latestCheckpoint;
    public float levelStartTime;
    public float levelCompleteTime;
    public float sceneStartTime;

    Dictionary<int, SectionStats> statDict;
    int totalDeath;
    int totalDrillRequested;
    private int totalDrillCount;
    int totalBlast;
    int totalDash;
    int totalSlide;
    int totalJump;
    bool isPlantCollected = false;
    GameObject playerBase;
    const int StartPointID = -1;
    
    [Serializable]
    public class SectionStats {
        public CheckPoint checkpoint;
        public int death;
        public int drillRequest;
        public int drillCount;
        public int blast;
        public int dash;
        public int slide;
        public int jump;
        public float startTime;
        public float elapsedTime;
        [HideInInspector] public string name;
    }

    private void Awake() {
        // // If there is an instance, and it's not me, destroy old.
        // if (_instance != null && _instance != this)
        // {
        //     Destroy(this.gameObject);
        //     return; // Ensure no further processing is done in this method
        // }
        // MetricManagerScript.instance?.LogString("SceneLoad");
        _instance = this;
    }

    // Start is called before the first frame update
    void Start() {
        playerBase = GameObject.FindGameObjectWithTag("Player");
        sceneStartTime = Time.time;
        
        statDict = new Dictionary<int, SectionStats>();
        statDict.Add(StartPointID, new SectionStats());
        foreach (var section in sections)
        {
            section.name = section.checkpoint.gameObject.name;
            statDict.Add(section.checkpoint.GetInstanceID(), section);
        }
        
        LogEvent(MetricAction.LevelStart);
    }

    public void LogEvent(CheckPoint checkPoint, MetricAction actionName) {
        // burrow
        // if (checkPoint is null)
        // {
        //     MetricManagerScript.instance.LogString(actionName.ToString(), $"{levelName}::NoCheckpoint");
        //     // handle summary
        //     switch (actionName)
        //     {
        //         case MetricsAction.Death:
        //             totalDeath += 1;
        //             break;
        //         case MetricsAction.Drill:
        //             totalDrill += 1;
        //             break;
        //         case MetricsAction.Blast:
        //             totalBlast += 1;
        //             break;
        //         case MetricsAction.Dash:
        //             totalDash += 1;
        //             break;
        //         default:
        //             throw new ArgumentOutOfRangeException(nameof(actionName), actionName, null);
        //     }
        //
        //     return;
        // }
        
        var checkpointName = (checkPoint is null)?"StartPoint":checkPoint.gameObject.name;
        var id = (checkPoint is null)? (StartPointID) :checkPoint.GetInstanceID();

        string dataToLog;
        if (checkPoint is not null) dataToLog = $"{checkpointName}";
        else dataToLog = $"StartPoint";
        // Ref: https://stackoverflow.com/questions/4617935/is-there-a-way-to-include-commas-in-csv-columns-without-breaking-the-formatting
        dataToLog += $",\"{playerBase.transform.position}\"";
        MetricManagerScript.instance?.LogString(actionName.ToString(), dataToLog);
        
        // handle summary
        switch (actionName)
        {
            case MetricAction.Death:
                statDict[id].death += 1;
                totalDeath += 1;
                break;
            case MetricAction.DrillRequest:
                statDict[id].drillRequest += 1;
                totalDrillRequested += 1;
                break;
            case MetricAction.DrillIn:
                statDict[id].drillCount += 1;
                totalDrillCount += 1;
                break;
            case MetricAction.DrillOut:
                // already counted for DrillIn
                break;
            case MetricAction.Blast:
                statDict[id].blast += 1;
                totalBlast += 1;
                break;
            case MetricAction.Dash:
                statDict[id].dash += 1;
                totalDash += 1;
                break;
            case MetricAction.Slide:
                statDict[id].slide += 1;
                totalSlide += 1;
                break;
            case MetricAction.Jump:
                statDict[id].jump += 1;
                totalJump += 1;
                break;
            case MetricAction.ActivateCheckpoint:
                var lastID = latestCheckpoint.GetInstanceID();
                
                // if enter a new section
                if (lastID != id)
                {
                    statDict[lastID].elapsedTime += Time.time - statDict[lastID].startTime;
                    latestCheckpoint = checkPoint;
                    statDict[id].startTime = Time.time; 
                }
                
                // if enter the same section
                if (lastID == id){
                    statDict[id].elapsedTime += Time.time - statDict[id].startTime;
                    statDict[id].startTime = Time.time; 
                }
                
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(actionName), actionName, null);
        }
    }

    public void LogEvent(MetricAction actionName) {
        switch (actionName)
        {
            case MetricAction.LevelStart:
                levelStartTime = Time.time;
                // MetricManagerScript.instance?.LogString(actionName.ToString());
                break;
            
            case MetricAction.GetPlant:
                isPlantCollected = true;
                levelCompleteTime = Time.time - levelStartTime;
                MetricManagerScript.instance?.LogString(actionName.ToString(), 
                    $"{levelCompleteTime}");
                break;
            
            case MetricAction.LevelEnd:
                // wrap up last checkpoint's time
                if (latestCheckpoint is not null)
                {
                    var lastID = latestCheckpoint.GetInstanceID();
                    if (!statDict.ContainsKey(lastID))
                    {
                        return;
                    }
                    
                    statDict[lastID].elapsedTime += Time.time - statDict[lastID].startTime;
                }
                
                // log reason of level end
                MetricManagerScript.instance?.LogString(actionName.ToString(), 
                        $"PlantCollected: {isPlantCollected}");

                LogStats();
                break;
            
            case MetricAction.CutsceneSkip:
                // MetricManagerScript.instance?.LogString(actionName.ToString());
                break;
            
            default:
                LogEvent(latestCheckpoint, actionName);
                break;
        }
    }

    public static void LogEvent(MetricAction actionName, string actionData) {
        switch (actionName)
        {
            case MetricAction.ControllerChange:
                MetricManagerScript.instance?.LogString(actionName.ToString(), 
                    $"{actionData}");
                break;
            default:
                throw new NotSupportedException(
                    $"{actionName.ToString()} not supported with custom ActionData string.");
        }
    }

    public void OnDestroy() {
        // LogEvent(MetricAction.LevelEnd);
        // MetricManagerScript.instance?.LogString("SceneUnload");
        // MetricManagerScript.instance?.WriteToFile();
    }

    private void LogStats() {
        foreach (var s in sections)
        {
            string sectionSummary = $"{s.name}," +
                                    $"Death: {s.death}," +
                                    $"DrillRequest: {s.drillRequest}," +
                                    $"DrillCount: {s.drillCount}," +
                                    $"Blast: {s.blast}," +
                                    $"Dash: {s.dash}," +
                                    $"Slide: {s.slide}," +
                                    $"Jump: {s.jump},";
                                    // $"ElapsedTime: {s.elapsedTime}";
            MetricManagerScript.instance?.LogString("SectionSummary", sectionSummary);
        }

        string levelSummary = $"InLevelTotal," +
                              $"Death: {totalDeath}," +
                              $"DrillRequested: {totalDrillRequested}," +
                              $"DrillCount: {totalDrillCount}," +
                              $"Blast: {totalBlast}," +
                              $"Dash: {totalDash}," +
                              $"Slide: {totalSlide}," +
                              $"Jump: {totalJump}";
        MetricManagerScript.instance?.LogString("LevelSummary", levelSummary);
    }
}
