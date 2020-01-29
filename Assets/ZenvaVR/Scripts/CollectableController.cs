using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenva.VR;

[RequireComponent(typeof(Interactable))]
public class CollectableController : MonoBehaviour {

    // is the item consumable? (it will be destroyed after collecting)
    public bool isConsumable = true;

    //serializable class for ANY item stat (health, coins, ammo)
    [Serializable]
    public class ItemStat
    {
        public string label;
        public float amount;
    }

    // flexible array of properties
    public ItemStat[] properties;

    // player collectable ctrl
    PlayerCollectableCtrl playerCollect;

    bool isOver;

    void Awake()
    {
        // get the PlayerCollectableCtr object
        playerCollect = FindObjectOfType<PlayerCollectableCtrl>();

        if(playerCollect == null)
        {
            Debug.LogError("There needs to be a PlayerCollectableCtrl item in your scene");
        }
    }

    public void HandleClick()
    {
        if (Vector3.Distance(transform.position, playerCollect.gameObject.transform.position) <= playerCollect.maxDistance)
        {
            // collect the item
            playerCollect.Collect(gameObject);

            // destroy the item if consumable
            if (isConsumable) Destroy(gameObject);
        }
    }

    public void HandleOut()
    {
        isOver = false;

        //the player collectable ctr knows we are looking away
        playerCollect.SelectionOut();

        // unselect the item
        Highlight(false);
    }

    public void HandleOver()
    {
        if(Vector3.Distance(transform.position, playerCollect.gameObject.transform.position) <= playerCollect.maxDistance)
        {
            isOver = true;

            //the player collectable ctr knows we are selecting
            playerCollect.SelectionOver();

            // highlight the item
            Highlight(true);
        }        
    }

    void Highlight(bool flag)
    {
        Renderer rend = GetComponent<Renderer>();

        if(rend != null)
            rend.material.SetFloat("_Outline", flag ? 0.003f : 0f);
    }

    // get the "amount" of a property with label "label"
    public float GetProperty(string label)
    {
        //iterating through the array to search for the label
        for (int i = 0; i < properties.Length; i++)
        {
            if (properties[i].label == label)
            {
                return properties[i].amount;
            }
        }

        return 0;
    }

    void Update()
    {
        // fix for when swapping items
        // if we are looking at the item, and if the player collect ctr is not "selecting"
        // we need to force selection
        if(isOver == true && !playerCollect.IsSelecting())
        {
            HandleOver();
        }
    }
}