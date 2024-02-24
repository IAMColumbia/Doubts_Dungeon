using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSaveManager : MonoBehaviour
{
    [SerializeField] Player SavedPlayer;
    [SerializeField] Player CurrentPlayer;


    // Start is called before the first frame update
    void Start()
    {
        GetCurrentPlayer();
    }

    private void OnLevelWasLoaded(int level)
    {
        GetCurrentPlayer();

        SetValues();
    }

    // Update is called once per frame
    void Update()
    {
        SaveValues();
    }

    public void GetCurrentPlayer()
    {
        GameObject PlayablePlayer = GameObject.Find("PlayablePlayer");
        Transform Menty = PlayablePlayer.transform.Find("MentyPlayer");//Breaking
        CurrentPlayer = Menty.GetComponent<Player>();
    }

    public void SaveValues()
    {
        //Heath
        SavedPlayer.health = CurrentPlayer.health;
        //Health Pickups
        SavedPlayer.healthPickups = CurrentPlayer.healthPickups;
        //Currenty
        SavedPlayer.Currancy = CurrentPlayer.Currancy;
        //Inventory
    }

    public void SetValues()
    {
        if(SavedPlayer != null) 
        {
            //Heath
            CurrentPlayer.health = SavedPlayer.health;
            //Health Pickups
            CurrentPlayer.healthPickups = SavedPlayer.healthPickups;
            //Currenty
            CurrentPlayer.Currancy = SavedPlayer.Currancy;
            //Inventory
        }

    }
}
