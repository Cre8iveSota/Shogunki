using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterModel
{
    int id;
    bool isMasterPlayerTapped = false;
    bool isClientPlayerTapped= false;
    bool hasMasterOwnership= true;
    bool isAlive= true;
    CharacterModel(bool isMaster){
        this.hasMasterOwnership = isMaster;
    }
}
