using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using UnityEngine.UI;

public class Arcade : MonoBehaviour
{    
    public GameObject UI;
    private int _stackedSceneHandle;
    private bool SceneStack = true;
    private NetworkObject nob;

    private void Start() {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        UI = GameObject.FindWithTag("Arcade");
        UI.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        nob = other.GetComponent<NetworkObject>();
        if(nob != null){
            ToggleInventory();
        }
        
    }
    /*public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        if(UI.activeSelf){
            ToggleInventory();
        }
    }   */
    public void ToggleInventory(){
        if(UI.activeSelf){
            UI.SetActive(false);
        }
        else if(!UI.activeSelf){
            UI.SetActive(true);
        }
    }
    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null) InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
    }
    public void To2048(){
        Debug.Log("Before everything");
        if (!nob.Owner.IsActive) { return; }
        SceneLookupData lookup;
        lookup = new SceneLookupData(_stackedSceneHandle,"2048");
        SceneLoadData sld = new SceneLoadData(lookup);
        sld.Options.AllowStacking = true;
        sld.MovedNetworkObjects = new NetworkObject[] { nob };
        Debug.Log("Before calling SceneManager.LoadConnectionScenes");
        InstanceFinder.SceneManager.LoadConnectionScenes(nob.Owner, sld);
        Debug.Log("After calling SceneManager.LoadConnectionScenes");
    }
    public void UnloadMain(){
        if(!InstanceFinder.IsServer){
            return;
        }
        SceneUnloadData sld = new SceneUnloadData("MainScene");
        InstanceFinder.SceneManager.UnloadConnectionScenes(nob.Owner, sld);
    }
    public void ToTetris(){
        if(nob != null && nob.Owner.IsActive){
            SceneLookupData lookup;
            lookup = new SceneLookupData(_stackedSceneHandle,"Tetris");
            SceneLoadData sld = new SceneLoadData(lookup);
            sld.Options.AllowStacking = true;
            sld.MovedNetworkObjects = new NetworkObject[] { nob };
            sld.ReplaceScenes = ReplaceOption.All;
            InstanceFinder.SceneManager.LoadConnectionScenes(nob.Owner, sld);
        }
    }
    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
    {
        if (!obj.QueueData.AsServer) return;
        if (!SceneStack) return;
        if (_stackedSceneHandle != 0) return;
        if (obj.LoadedScenes.Length > 0) _stackedSceneHandle = obj.LoadedScenes[0].handle;
    }
}
