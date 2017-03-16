using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemy
{
    public Weapon defaultWeapon;
    public Weapon equippedWeapon;
    public WeaponList weaponsList;
    
    void Awake()
    {
        //RandomWeapon();
    }

    void RandomWeapon()
    {
        int random = Random.Range(1, 10);
        if(random == 10)
        {
            int randomWeapon = Random.Range(1, weaponsList.weapons.Count);
            equippedWeapon = weaponsList.weapons[randomWeapon];
        }
        else
        {
            equippedWeapon = defaultWeapon;
        }
    }
	
}
