using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PowerUpsGenerate : MonoBehaviour {
    private float screenWidthInPoints; // cache screen size in points.

    public GameObject[] availableObjects; // All the objects such as lasers,coins, collectables etc.
    public List<GameObject> objects; // used store all created objects.

    public float objectsMinDistance = 5.0f; //min distance to place away
    public float objectsMaxDistance = 10.0f;  //max distance to place away

    public float objectsMinY = -1.4f; // minimum height to place objects at.
    public float objectsMaxY = 1.4f;  //maximum height to place objects at.

    public float objectsMinRotation = -45.0f; //min rotation for objects
    public float objectsMaxRotation = 45.0f;  //max rotation for objects

    bool test = false;
    void Start() {
        //Start spawning powerups after 50s
        Invoke("GenerateObjectsIfRequired", 50.0f);
        float height = 2.0f * Camera.main.orthographicSize;
        screenWidthInPoints = height * Camera.main.aspect;
    }

    void FixedUpdate() {
    }

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
        print("ja");
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
            farthestObjectX = this.transform.position.x +2;

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
        //Repeat void randomly every x to x seconds.
        float delay = Random.Range(25f, 40f);
        Invoke("GenerateObjectsIfRequired", delay);
    }
}
