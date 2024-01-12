using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class PlayerCamera : NetworkBehaviour
{
   public override void OnStartClient(){
        base.OnStartClient();
        if(base.IsOwner){
            gameObject.SetActive(true);
        }
   }
}
