using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using UnityEngine.UI;

public class Arcade : NetworkBehaviour
{    
    public GameObject UI;
    private int _stackedSceneHandle;
    private bool SceneStack = true;
    private NetworkObject nob;
    [SerializeField] private OpenArcadeUI openArcadeUI;

    private void Start() {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
        openArcadeUI = new OpenArcadeUI(); 
    }
    public override void OnStartClient(){
        base.OnStartClient();
        if(!base.IsOwner){
            enabled = false;
            return;
        }
        if(UI.activeSelf){
            ToggleInventory();
        }
        nob = openArcadeUI.getnob();
    }   
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
        if(nob != null && nob.Owner.IsActive){
            Debug.Log("test");
            SceneLookupData lookup;
            lookup = new SceneLookupData(_stackedSceneHandle,"2048");
            SceneLoadData sld = new SceneLoadData(lookup);
            sld.Options.AllowStacking = true;
            sld.MovedNetworkObjects = new NetworkObject[] { nob };
            sld.ReplaceScenes = ReplaceOption.All;
            InstanceFinder.SceneManager.LoadConnectionScenes(nob.Owner, sld);
        }
    }
    public void ToTetris(){
        Debug.Log("test");
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
