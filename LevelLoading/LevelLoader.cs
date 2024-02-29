using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using __OasisBlitz.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

/******************************
//Add Item to category
1.Add Prefab to corresponding list under [LevelLoader] in the unity editor

//Remove Item From category
1.Remove Prefab from corresponding list under [LevelLoader] in the unity editor

//Add a new Category
1.Create an empty GameObject under "LevelData(Loaded)" titled "[Insert your category name]"
2.Add your gameobject's transform to the Category Parent Objects list under [LevelLoader] in the unity editor
3.Add a folder in the prefabs folder and title it the same name as your category
4.Add a [SerializeField] private List<GameObject> titled "[Insert your category name]Prefabs" below 
the declaration of private List<List<GameObject>> Prefabs = new List<List<GameObject>>();
5.In the "Awake" function, add your list created in step 4 to Prefabs (Prefabs.[Insert your category name]Prefabs)
6.Find "public enum Categories" below and add an enum titled "[Insert your category name]" 
NOTE: THIS MUST LINE UP WITH THE INDEX OF THE PREFABS LIST CREATED IN STEP 4

//Remove a Category
1.Remove empty GameObject and any children under "LevelData(Loaded)" titled "[Insert your category name]"
2.Remove your gameobject's transform to the Category Parent Objects list under [LevelLoader] in the unity editor
NOTE: YOU MUST PRESERVE THE ORDER OF ALL THE OTHER PARENT TRANSFORMS
3.Remove the folder in the prefabs folder titled "[Insert your category name]"
4.Remove a [SerializeField] private List<GameObject> titled "[Insert your category name]Prefabs" below 
the declaration of private List<List<GameObject>> Prefabs = new List<List<GameObject>>();
5.In the "Awake" function, remove your list created in step 4 to Prefabs (Prefabs.[Insert your category name]Prefabs)
6.Find "public enum Categories" below and remove the enum titled "[Insert your category name]" 
NOTE: THIS MUST LINE UP WITH THE INDEX OF THE PREFABS LIST CREATED IN STEP 4
*******************************/
namespace __OasisBlitz.LevelLoading
{
    [ExecuteAlways]
    public class LevelLoader : MonoBehaviour
    {
        [SerializeField] private int MaxHorizontalDist;//limits horizontal placement
        [SerializeField] private int MaxVerticalDist;//limits vertical placement
        //Enums for each category
        /********************************
     * ADD/REMOVE YOUR ENUM BELOW *
     ********************************/
        public enum Categories
        {
            Enemies,
            Environment,
            RespawnPoints,
            Movement,
            CategoriesCount
        }

        //Player Transform
        [SerializeField] private Transform playerTransform;
        [SerializeField] private CharacterController playerCharacterController;
        //List of category parent objects - objects of each category will be Instantiated with their respective category parent object as their parent
        [SerializeField] private List<Transform> CategoryParentObjects;

        /***************************************
     * ADD/REMOVE YOUR CATEGORY LIST BELOW *
    ****************************************/
        //List containing prototypes for all potential objects
        private List<List<GameObject>> Prefabs = new List<List<GameObject>>();
        [SerializeField] private List<GameObject> EnemyPrefabs;
        [SerializeField] private List<GameObject> EnvironmentPrefabs;
        [SerializeField] private List<GameObject> RespawnPointsPrefabs;
        [SerializeField] private List<GameObject> MovementPrefabs;

        //Dictionary to find subtypes from object name
        private List<Dictionary<string, int>> objectNameToSubtypeIndex;
        private void Awake()
        {
            /***************************************************
        * ADD/REMOVE YOUR CATEGORY LIST TO "Prefabs" BELOW *
        ****************************************************/
            //create list of lists where any prefab can be created using Prefabs[category][subtype]
            Prefabs.Add(EnemyPrefabs);
            Prefabs.Add(EnvironmentPrefabs);
            Prefabs.Add(RespawnPointsPrefabs);
            Prefabs.Add(MovementPrefabs);
#if DEBUG
            if (Constants.DebugLevelLoader)
            {
                for (int i = 0; i < Prefabs.Count; i++)
                {
                    for (int j = 0; j < Prefabs[i].Count; j++)
                    {
                        Debug.Log(i + " " + j + " " + Prefabs[i][j].name);
                    }
                }
            }
#endif
            //create a list of dictionaries containing subtype names as keys and subtype index as value
            objectNameToSubtypeIndex = new List<Dictionary<string, int>>();
            int size = (int)Categories.CategoriesCount;
            for (int i = 0; i < size; i++)
            {
                //fill dictionaries with keys equal to prefab names and value being index such that Prefabs[Category][index] returns a prefab of the original object
                Dictionary<string, int> temp = new Dictionary<string, int>();
                for (int j = 0; j < Prefabs[i].Capacity; j++)
                {
                    string Key = Prefabs[i][j].name;
                    Key = Regex.Replace(Key, @"\((.*?)\)", "");
                    Key = Key.Replace(" ", "");
                    temp.Add(Key, j);
#if DEBUG
                    if (Constants.DebugLevelLoader)
                    {
                        Debug.Log("Category: " + i + " Key: " + Key + " Index: " + j);
                    }
#endif
                }
                objectNameToSubtypeIndex.Add(temp);
            }
        }
        private void OnEnable()
        {
            LevelLoaderUIManager.CreateEmptyAction += DeleteAll;
            LevelLoaderUIManager.LoadAction += LoadLevel;
            LevelLoaderUIManager.SaveAction += SaveLevel;
        }

        private void OnDisable()
        {
            LevelLoaderUIManager.CreateEmptyAction -= DeleteAll;
            LevelLoaderUIManager.LoadAction -= LoadLevel;
            LevelLoaderUIManager.SaveAction -= SaveLevel;
        }
        public class Level
        {
            /// <summary>
            /// An Element is one object (like an enemy or environment piece)
            /// Player just has the information related to the player (not spawned in)
            /// </summary>
            public class Player
            {
                //position
                public float pos_x;
                public float pos_y;
                public float pos_z;
            
                //rotation
                public float rot_x;
                public float rot_y;
                public float rot_z;
                public float rot_w;
            }
            public class Element
            {
                public int m_category;
                public int m_subtype;
                public string m_name;
                //position
                public float pos_x;
                public float pos_y;
                public float pos_z;
            
                //rotation
                public float rot_x;
                public float rot_y;
                public float rot_z;
                public float rot_w;
            
                //Scale
                public float scale_x;
                public float scale_y;
                public float scale_z;
            }
            public List<Element> m_elements;
            public Player m_player;
        }
    
        private void LoadLevel(string saveName)
        {
            //DeleteALL
            DeleteAll();
            //Get level from .xml
            StreamReader sr = new StreamReader(Application.persistentDataPath + "/" + saveName + "Level.xml");
            string text = sr.ReadToEnd();
            sr.Close();
            StringReader textRead = new StringReader(text);
            XmlSerializer xmls = new XmlSerializer(typeof(Level));
            Level savedLevel = xmls.Deserialize(textRead) as Level;
            //set player pos and rot
            MovePlayer(new Vector3(savedLevel.m_player.pos_x, savedLevel.m_player.pos_y, savedLevel.m_player.pos_z),
                new Quaternion(savedLevel.m_player.rot_x, savedLevel.m_player.rot_y, savedLevel.m_player.rot_z,
                    savedLevel.m_player.rot_w));
            //Set to true if we have to reset an index OR item is not found
            bool invalidEntryDetected = false;
            //create all enemies
            int size = savedLevel.m_elements.Count;
            int numCategories = (int)Categories.CategoriesCount;
            for (int i = 0; i < size; i++)
            {
                int objCategory = savedLevel.m_elements[i].m_category;
                int subtypeIndex = savedLevel.m_elements[i].m_subtype;
#if DEBUG
                if (Constants.DebugLevelLoader)
                {
                    if (objCategory < numCategories)
                    {
                        if (subtypeIndex < Prefabs[objCategory].Count)
                        {
                            Debug.Log("Spawning in: " + objCategory + " " + subtypeIndex + " " +
                                      Prefabs[objCategory][subtypeIndex].name);
                        }
                    }
                }
#endif
                //validate the name and index of the obj
                bool found = false;
                if (objCategory < numCategories)
                {
                    if (subtypeIndex < Prefabs[objCategory].Count)
                    {
                        if (Prefabs[objCategory][subtypeIndex].name == savedLevel.m_elements[i].m_name)
                        {
                            //set rot, pos, scale, and parent of each element
                            GameObject temp = Instantiate(Prefabs[objCategory][subtypeIndex],
                                new Vector3(savedLevel.m_elements[i].pos_x, savedLevel.m_elements[i].pos_y,
                                    savedLevel.m_elements[i].pos_z),
                                new Quaternion(savedLevel.m_elements[i].rot_x, savedLevel.m_elements[i].rot_y,
                                    savedLevel.m_elements[i].rot_z, savedLevel.m_elements[i].rot_w),
                                CategoryParentObjects[objCategory]);
                            temp.transform.localScale = new Vector3(savedLevel.m_elements[i].scale_x,
                                savedLevel.m_elements[i].scale_y, savedLevel.m_elements[i].scale_z);
                            found = true;
                        }
                    }
                }
#if DEBUG
                else if(Constants.DebugLevelLoader)
                {
                    Debug.Log("Object out of range: " + savedLevel.m_elements[i].m_name);
                }
#endif
                if (!found)//check to see if the item still exists in the same category- accounts for list reordering
                {
                    invalidEntryDetected = true;
                    if (objCategory < numCategories)
                    {
                        if (objectNameToSubtypeIndex[objCategory].ContainsKey(savedLevel.m_elements[i].m_name))
                        {
                            //reset subtype index
                            subtypeIndex = objectNameToSubtypeIndex[objCategory][savedLevel.m_elements[i].m_name];
                            savedLevel.m_elements[i].m_subtype = subtypeIndex;
                            //set rot, pos, scale, and parent of each element
                            GameObject temp = Instantiate(Prefabs[objCategory][subtypeIndex],
                                new Vector3(savedLevel.m_elements[i].pos_x, savedLevel.m_elements[i].pos_y,
                                    savedLevel.m_elements[i].pos_z),
                                new Quaternion(savedLevel.m_elements[i].rot_x, savedLevel.m_elements[i].rot_y,
                                    savedLevel.m_elements[i].rot_z, savedLevel.m_elements[i].rot_w),
                                CategoryParentObjects[objCategory]);
                            temp.transform.localScale = new Vector3(savedLevel.m_elements[i].scale_x,
                                savedLevel.m_elements[i].scale_y, savedLevel.m_elements[i].scale_z);
                            found = true;
                        }
                    }
                }
                if (!found) //check if item was moved to another category
                {
                    for (int j = 0; j < numCategories; j++)
                    {
                        if (objectNameToSubtypeIndex[j].ContainsKey(savedLevel.m_elements[i].m_name))
                        {
                            //reset subtype index
                            objCategory = j;
                            savedLevel.m_elements[i].m_category = objCategory;
                            subtypeIndex = objectNameToSubtypeIndex[objCategory][savedLevel.m_elements[i].m_name];
                            savedLevel.m_elements[i].m_subtype = subtypeIndex;
                            //set rot, pos, scale, and parent of each element
                            GameObject temp = Instantiate(Prefabs[objCategory][subtypeIndex],
                                new Vector3(savedLevel.m_elements[i].pos_x, savedLevel.m_elements[i].pos_y,
                                    savedLevel.m_elements[i].pos_z),
                                new Quaternion(savedLevel.m_elements[i].rot_x, savedLevel.m_elements[i].rot_y,
                                    savedLevel.m_elements[i].rot_z, savedLevel.m_elements[i].rot_w),
                                CategoryParentObjects[objCategory]);
                            temp.transform.localScale = new Vector3(savedLevel.m_elements[i].scale_x,
                                savedLevel.m_elements[i].scale_y, savedLevel.m_elements[i].scale_z);
                            found = true;
                        }
                    }
                }
                if (!found)//remove item as it is no longer supported by the level loader
                {
                    savedLevel.m_elements.Remove(savedLevel.m_elements[i]);
                    size--;
                    i--;
                }
            }
            if (invalidEntryDetected)
            {
                //Resave the level with the correct data
                StringWriter sw = new StringWriter();
                xmls.Serialize(sw, savedLevel);
                StreamWriter stream = new StreamWriter(Application.persistentDataPath + "/" + saveName + "Level.xml");
#if DEBUG
                if (Constants.DebugLevelLoader)
                {
                    Debug.Log(saveName + "Level.xml successfully resaved to account for data inconsistencies: " + Application.persistentDataPath);
                }
#endif
                stream.Write(sw.ToString());
                stream.Close();
            }
        }
        private void SaveLevel(string saveName)
        {
            OrganizeLooseObjects();
            Level levelToSave = new Level();
            levelToSave.m_elements = new List<Level.Element>();
            //read in player pos and rot
            levelToSave.m_player = new Level.Player();
            levelToSave.m_player.pos_x = playerTransform.localPosition.x;
            levelToSave.m_player.pos_y = playerTransform.localPosition.y;
            levelToSave.m_player.pos_z = playerTransform.localPosition.z;
            levelToSave.m_player.rot_x = playerTransform.localRotation.x;
            levelToSave.m_player.rot_y = playerTransform.localRotation.y;
            levelToSave.m_player.rot_z = playerTransform.localRotation.z;
            levelToSave.m_player.rot_w = playerTransform.localRotation.w;
            //read in each object under each category
            int size = (int)Categories.CategoriesCount;
            for (int i = 0; i < size; i++)
            {
                int numElementsInCategory = CategoryParentObjects[i].childCount;
                for (int j = 0; j < numElementsInCategory; j++)
                {
                    //read in each object e
                    Transform objectToAdd = CategoryParentObjects[i].GetChild(j);
                    //Remove "(Clone)", "(#)", and " " from object name
                    string objectToAddName = objectToAdd.name;
                    objectToAddName = Regex.Replace(objectToAddName, @"\((.*?)\)", "");
                    objectToAddName = objectToAddName.Replace(" ", "");
                    //Find index of subtype
#if DEBUG
                    //check if index is not found(should never happen)
                    if (Constants.DebugLevelLoader)
                    {
                        if (!objectNameToSubtypeIndex[i].ContainsKey(objectToAddName))
                        {
                            Debug.Log("Leveloader has no prefab titled " + objectToAddName + " in Category " + i);
                        }
                    }
#endif
                    //Only add to save file if the item is supported and under the right parent object
                    if (objectNameToSubtypeIndex[i].ContainsKey(objectToAddName))
                    {
                        int index = objectNameToSubtypeIndex[i][objectToAddName];
                        //Create an element
                        Level.Element element = new Level.Element();
                        //Fill in category, subtype, and name
                        element.m_category = i;
                        element.m_subtype = index;
                        element.m_name = objectToAddName;
                        //Fill with pos, rot, and scale
                        element.pos_x = objectToAdd.localPosition.x;
                        element.pos_y = objectToAdd.localPosition.y;
                        element.pos_z = objectToAdd.localPosition.z;
                        element.rot_x = objectToAdd.localRotation.x;
                        element.rot_y = objectToAdd.localRotation.y;
                        element.rot_z = objectToAdd.localRotation.z;
                        element.rot_w = objectToAdd.localRotation.w;
                        element.scale_x = objectToAdd.localScale.x;
                        element.scale_y = objectToAdd.localScale.y;
                        element.scale_z = objectToAdd.localScale.z;
                        //Add to element list in level
                        levelToSave.m_elements.Add(element);
                    }
                }
            }
            //create writer
            StringWriter sw = new StringWriter();
            XmlSerializer xmls = new XmlSerializer(typeof(Level));
            xmls.Serialize(sw, levelToSave);
            StreamWriter stream = new StreamWriter(Application.persistentDataPath + "/" + saveName + "Level.xml");
#if DEBUG
            if (Constants.DebugLevelLoader)
            {
                Debug.Log(saveName + "Level.xml successfully saved at: " + Application.persistentDataPath);
            }
#endif
            stream.Write(sw.ToString());
            stream.Close();
        }
        private void DeleteAll()
        {
            //loop over each parent object and delete all their children
            int size = (int)Categories.CategoriesCount;
            for (int i = 0; i < size; i++)
            {
                while(CategoryParentObjects[i].childCount != 0)
                {
#if DEBUG
                    if (Constants.DebugLevelLoader)
                    {
                        Debug.Log("Destroying " + CategoryParentObjects[i].GetChild(0).name);
                    }
#endif
                    //Note: Destroy Immediate enacts the garbage collector, deleting all references to the object deleted.  Destroy enacts the gc at the end of the frame
                    DestroyImmediate(CategoryParentObjects[i].GetChild(0).gameObject);
                }
            }
            RemoveLooseObjects();
        }
        //Removes objects that have no parents
        private void RemoveLooseObjects()
        {
            int size = (int)Categories.CategoriesCount;
            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.transform.parent == null)
                {
                    //Remove "(Clone)", "(#)", and " " from object name
                    string objectToAddName = obj.name;
                    objectToAddName = Regex.Replace(objectToAddName, @"\((.*?)\)", "");
                    objectToAddName = objectToAddName.Replace(" ", "");
                    for (int i = 0; i < size; i++)
                    {
                        //test if object belongs to category i
                        if (objectNameToSubtypeIndex[i].ContainsKey(objectToAddName))
                        { 
                            //remove object
                            DestroyImmediate(obj);
                            break;
                        }
                    }
                }
            }
        }
        //organizes objects that have no parents (prevents user from having to manually organize the items)
        private void OrganizeLooseObjects()
        {
            int size = (int)Categories.CategoriesCount;
            foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
            {
                if (obj.transform.parent == null)
                {
                    //Remove "(Clone)", "(#)", and " " from object name
                    string objectToAddName = obj.name;
                    objectToAddName = Regex.Replace(objectToAddName, @"\((.*?)\)", "");
                    objectToAddName = objectToAddName.Replace(" ", "");
                    for (int i = 0; i < size; i++)
                    {
                        //test if object belongs to category i
                        if (objectNameToSubtypeIndex[i].ContainsKey(objectToAddName))
                        { 
                            //give object proper parent
                            obj.transform.parent = CategoryParentObjects[i];
                            break;
                        }
                    }
                }
            }
        }
        private void MovePlayer(Vector3 newPos, Quaternion newRot)
        {
            //probably extract this into a helper method.
            playerCharacterController.enabled = false;
            playerTransform.localPosition = newPos;
            playerTransform.localRotation = newRot;
            playerCharacterController.enabled = true;
        
        }

#if UNITY_EDITOR
        public void EditorLevelStorage(string fileName, bool load, bool save)
        {
            if (save)
            {
                SaveLevel(fileName);
            }
            if (load)
            {
                LoadLevel(fileName);
            }
        }

        public void LoadLevelOutOfPlayMode()
        {
            LevelLoaderForScene levelLoaderForScene = GameObject.FindGameObjectWithTag("LevelLoaderScene").GetComponent<LevelLoaderForScene>();
            levelLoaderForScene.LoadOutOfPlayMode();
        }

        public void SaveAndLoadOutOfPlayMode()
        {
            LevelLoaderForScene levelLoaderForScene = GameObject.FindGameObjectWithTag("LevelLoaderScene").GetComponent<LevelLoaderForScene>();
            levelLoaderForScene.SaveAndLoadOutOfPlayMode();
        }
#endif
    }
}
