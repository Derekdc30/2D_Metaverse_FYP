using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using FishNet.Connection;
using Unity.Mathematics;
using UnityEngine;

public class CharactorSelection : NetworkBehaviour
{
    [SerializeField] private List<GameObject> Types = new List<GameObject>();
    [SerializeField] private GameObject charactorSelectorPanel;
    [SerializeField] private GameObject canvasObject;

    public override void OnStartClient()
    {
        base.OnStartClient();

        if(!base.IsOwner)
            canvasObject.SetActive(false);
    }

    public void SpawnPlayerType1()
    {
        charactorSelectorPanel.SetActive(false);
        Spawn(0, LocalConnection);
    }

    public void SpawnPlayerType2()
    {
        charactorSelectorPanel.SetActive(false);
        Spawn(1, LocalConnection);
    }

    [ServerRpc(RequireOwnership = false)]
    void Spawn(int spawnIndex, NetworkConnection conn)
    {
        GameObject player = Instantiate(Types[spawnIndex], SpawnPoint.instance.transform.position, quaternion.identity);
        Spawn(player, conn);     
    }

}
