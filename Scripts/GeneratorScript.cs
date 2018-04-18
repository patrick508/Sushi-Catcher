using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GeneratorScript : MonoBehaviour {
    #region powerups variables
    public GameObject[] availablePowerups; // All the powerups
    public List<GameObject> powerups; // used store all created powerups.
    #endregion

    public GameObject[] AllRooms; // all available rooms to use for spawning
    public List<GameObject> ActiveRooms; // Current spawned rooms
    private float screenWidthInPoints; // cache screen size in points.

    public GameObject[] availableObjects; // All the objects such as lasers,coins, collectables etc.
    public List<GameObject> objects; // used store all created objects.

    public GameObject[] availableObjects2; // All the objects such as lasers,coins, collectables etc.
    public List<GameObject> objects2; // used store all created objects.

    public GameObject[] availableCoins; // All the objects such as lasers,coins, collectables etc.
    public List<GameObject> coins; // used store all created objects.

    public float objectsMinDistance = 5.0f; //min distance to place away
    public float objectsMaxDistance = 10.0f;  //max distance to place away

    public float objectsMinY = -1.4f; // minimum height to place objects at.
    public float objectsMaxY = 1.4f;  //maximum height to place objects at.

    public float objectsHighMinY = -1.4f; // minimum height to place objects at.
    public float objectsHighMaxY = 1.4f;  //maximum height to place objects at.

    public float objectsLowMinY = -1.4f; // minimum height to place objects at.
    public float objectsLowMaxY = 1.4f;  //maximum height to place objects at.

    public float objectsMinRotation = -45.0f; //min rotation for objects
    public float objectsMaxRotation = 45.0f;  //max rotation for objects

    public GameObject StartRoom;
    void Start () {
        //Start spawning powerups after 50s
        Invoke("GeneratePowerupsIfRequired", 20.0f); //start powerups after x seconds
        Invoke("GenerateSideObjectsIfRequired", 10.0f); //start extralasers after x seconds
        //calculate the size of the screen in points. The screen size will be used to determine if you need to generate a new room.
        float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;
        ActiveRooms.Add(StartRoom);
    }

    void FixedUpdate() {
        GenerateRoomIfRequired();
        GenerateObjectsIfRequired();
        GenerateCoinsIfRequired();
    }

    #region AddObjects
    void AddObject(float lastObjectX) {
        //pick something out of the array, and instantiate it with a random height and interval(based on parameters)
        int randomIndex = Random.Range(0, availableObjects.Length);
        GameObject obj = (GameObject)Instantiate(availableObjects[randomIndex]);
        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(objectsMinY, objectsMaxY);
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        //Add a random rotation to the object, and add it to the list.
        float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        objects.Add(obj);
    }

    void GenerateObjectsIfRequired() {
        //calculate points ahead and behind the player(used for deleting and adding) and make a new float
        float playerX = transform.position.x;
        float removeObjectsX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObjectX = 0;

        //make a new list with objects to remove after the loop.
        List<GameObject> objectsToRemove = new List<GameObject>();

        //set position for every object in objects list.
        foreach (var obj in objects) {
            float objX = obj.transform.position.x;

            //calculate a maximum objX value based on farthestObjectX
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            //if object is to far behind, mark it as removed and remove all objects inside list.
            if (objX < removeObjectsX)
                objectsToRemove.Add(obj);
        }
        foreach (var obj in objectsToRemove) {
            objects.Remove(obj);
            Destroy(obj);
        }

        //add more objects if player is about to see the last object.
        if (farthestObjectX < addObjectX)
            AddObject(farthestObjectX);
    }
    #endregion
    #region AddSideObjects
    void AddSideObject(float lastObject2X) {
        //pick something out of the array, and instantiate it with a random height and interval(based on parameters)
        int randomIndex = Random.Range(0, availableObjects2.Length);
        GameObject obj = (GameObject)Instantiate(availableObjects2[randomIndex]);
        if (obj.tag == "LasersHigh") {
            float objectPositionX = lastObject2X + Random.Range(objectsMinDistance, objectsMaxDistance);
            float randomY = Random.Range(objectsHighMinY, objectsHighMaxY);
            obj.transform.position = new Vector3(objectPositionX, randomY, 0);
        } else if (obj.tag == "LasersLow") {
            float objectPositionX = lastObject2X + Random.Range(objectsMinDistance, objectsMaxDistance);
            float randomY = Random.Range(objectsLowMinY, objectsLowMaxY);
            obj.transform.position = new Vector3(objectPositionX, randomY, 0);
        } 

        //Add a random rotation to the object, and add it to the list.
        float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        objects2.Add(obj);
    }

    void GenerateSideObjectsIfRequired() {
        //calculate points ahead and behind the player(used for deleting and adding) and make a new float
        float playerX = transform.position.x;
        float removeObjectsX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObject2X = 0;

        //make a new list with objects to remove after the loop.
        List<GameObject> objectsToRemove = new List<GameObject>();

        //set position for every object in objects list.
        foreach (var obj in objects2) {
            float objX = obj.transform.position.x;

            //calculate a maximum objX value based on farthestObjectX
            farthestObject2X = this.transform.position.x + 6;

            //if object is to far behind, mark it as removed and remove all objects inside list.
            if (objX < removeObjectsX)
                objectsToRemove.Add(obj);
        }
        foreach (var obj in objectsToRemove) {
            objects2.Remove(obj);
            Destroy(obj);
        }

        //add more objects if player is about to see the last object.
        if (farthestObject2X < addObjectX)
            AddSideObject(farthestObject2X);
        float powerupdelay = Random.Range(3f, 7f);
        Invoke("GenerateSideObjectsIfRequired", powerupdelay);
    }
    #endregion
    #region AddCoins
    void AddCoins(float lastObjectX) {
        //pick something out of the array, and instantiate it with a random height and interval(based on parameters)
        int randomIndex = Random.Range(0, availableCoins.Length);
        GameObject obj = (GameObject)Instantiate(availableCoins[randomIndex]);
        float objectPositionX = lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(objectsMinY, objectsMaxY);
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        //Add a random rotation to the object, and add it to the list.
        float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        coins.Add(obj);
    }

    void GenerateCoinsIfRequired() {
        //calculate points ahead and behind the player(used for deleting and adding) and make a new float
        float playerX = transform.position.x;
        float removeObjectsX = playerX - screenWidthInPoints;
        float addObjectX = playerX + screenWidthInPoints;
        float farthestObjectX = 0;

        //make a new list with objects to remove after the loop.
        List<GameObject> objectsToRemove = new List<GameObject>();

        //set position for every object in objects list.
        foreach (var obj in coins) {
            float objX = obj.transform.position.x;

            //calculate a maximum objX value based on farthestObjectX
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            //if object is to far behind, mark it as removed and remove all objects inside list.
            if (objX < removeObjectsX)
                objectsToRemove.Add(obj);
        }
        foreach (var obj in objectsToRemove) {
            coins.Remove(obj);
            Destroy(obj);
        }

        //add more objects if player is about to see the last object.
        if (farthestObjectX < addObjectX)
                AddCoins(farthestObjectX);
    }
    #endregion
    #region Generate Rooms
    void AddRoom(float farhtestRoomEndX) {
        //pick a random room from the array and instantiate it.
        int randomRoom = Random.Range(0, AllRooms.Length);
        GameObject room = (GameObject)Instantiate(AllRooms[randomRoom]);

        //Find the floor, and pos to instantiate(half of the screen) and add it to active rooms.
        float roomWidth = room.transform.FindChild("Floor").localScale.x;
        float roomCenter = farhtestRoomEndX + roomWidth * 0.5f; 
        room.transform.position = new Vector3(roomCenter, 0, 0);
        ActiveRooms.Add(room);
    }

    void GenerateRoomIfRequired() {
        //Create a new list with rooms to be deleted, and make a bool to check if rooms need to be added
        List<GameObject> DeleteRooms = new List<GameObject>();
        bool AddRooms = true;

        //Save the playerPos, , calculate the point after when the room needs to be removed and calculate the starting pos for instantiating a new room
        float playerX = transform.position.x;
        float removeRoomX = playerX - screenWidthInPoints;
        float addRoomX = playerX + screenWidthInPoints;

        //store the point where the room ens, this is used to instantiate a new room aswell
        float farthestRoomEndX = 0;

       // use the floor to get the room width and calculate the roomStartX and roomEndX
        foreach (var room in ActiveRooms) {
            float roomWidth = room.transform.FindChild("Floor").localScale.x;
            float roomStartX = room.transform.position.x - (roomWidth * 0.5f);
            float roomEndX = roomStartX + roomWidth;

            //If there is a room that starts after addRoomX, delete it. or ends to the left of removeRoomX, delete it
            if (roomStartX > addRoomX)
                AddRooms = false;
            if (roomEndX < removeRoomX)
                DeleteRooms.Add(room);
            //the most right point of the level.
            farthestRoomEndX = Mathf.Max(farthestRoomEndX, roomEndX -0.01f);
        }
        //remove rooms that are marked as removal, and if addRooms is still true, add a new room
        foreach (var room in DeleteRooms) {
            ActiveRooms.Remove(room);
            Destroy(room);
        }
        if (AddRooms)
            AddRoom(farthestRoomEndX);
            
    }
    #endregion
    #region AddPowerup
    void AddPowerup(float lastPowerupX) {
        //pick something out of the array, and instantiate it with a random height and interval(based on parameters)
        int randomIndex = Random.Range(0, availablePowerups.Length);
        GameObject pwrup = (GameObject)Instantiate(availablePowerups[randomIndex]);
        float powerupPositionX = lastPowerupX + Random.Range(objectsMinDistance, objectsMaxDistance);
        float randomY = Random.Range(objectsMinY, objectsMaxY);
        pwrup.transform.position = new Vector3(powerupPositionX, randomY, 0);

        powerups.Add(pwrup);
    }
    void GeneratePowerupsIfRequired() {
        //calculate points ahead and behind the player(used for deleting and adding) and make a new float
        float playerX = transform.position.x;
        float removePowerupsX = playerX - screenWidthInPoints;
        float addPowerupsX = playerX + screenWidthInPoints;
        float farthestPowerupX = 0;

        //make a new list with powerup to remove after the loop.
        List<GameObject> powerupsToRemove = new List<GameObject>();

        //set position for every powerup in list.
        foreach (var pwrup in powerups) {
            float pwupx = pwrup.transform.position.x;

            //set farthestPowerupX to my pos + 6
            farthestPowerupX = this.transform.position.x + 6;
            print(this.transform.position.x + 6);

            //if powerup is to far behind, mark it as removed and remove all objects inside list.
            if (pwupx < removePowerupsX)
                powerupsToRemove.Add(pwrup);
        }
        foreach (var pwrup in powerupsToRemove) {
            powerups.Remove(pwrup);
            Destroy(pwrup);
        }

        //add more powerups if player is about to see the last object.
        if (farthestPowerupX < addPowerupsX)
        AddPowerup(farthestPowerupX);

        //Repeat void randomly every x to x seconds.
        float powerupdelay = Random.Range(13f, 17f);
        Invoke("GeneratePowerupsIfRequired", powerupdelay);
    }
    #endregion
}
