using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PetManager : MonoBehaviour
{
    public Pet pet;
    public int attack;
    public int defense;
    public int intelligence;
    public int skills;

    public int level;
    public bool isBot;

    public int strategy;

    // Start is called before the first frame update
    private void Start()
    {
        strategy = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   
    

    public int GetPetTotalStats()
    {
        isBot = pet.isBot;
        switch (level)
        {
            case 0:
                attack = Random.Range(50, 500);
                defense = Random.Range(50, 500);
                intelligence = Random.Range(50, 500);
                skills = Random.Range(50, 500);
                break;
            case 1:
                attack = Random.Range(200, 2000);
                defense = Random.Range(200, 2000);
                intelligence = Random.Range(200, 2000);
                skills = Random.Range(200, 2000);
                break;
            case 2:
                attack = Random.Range(1000, 10000);
                defense = Random.Range(1000, 10000);
                intelligence = Random.Range(1000, 10000);
                skills = Random.Range(1000, 10000);
                break;
        }
        //attack = pet.Attack;
        //defense = pet.Defense;
        //intelligence = pet.Intelligence;
        //skills = pet.Skill;
        int total = skills + intelligence + defense + attack;
        return total;
        //Debug.Log(name + " Attack: " + attack + " Defense: " + defense + " Intelligence: " + intelligence + " Skills: " + skills);
    }
}
