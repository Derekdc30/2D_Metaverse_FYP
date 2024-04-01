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
    public GameObject Button1;
    public GameObject Button2;
    private int _stackedSceneHandle;
    private bool SceneStack = true;

    private void Start() {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        NetworkObject nob = other.GetComponent<NetworkObject>();
        if(nob != null){
            Button1.GetComponent<Button>().onClick.AddListener(()=>{
                To2048(nob);
            });
            Button2.GetComponent<Button>().onClick.AddListener(()=>{
                ToTetris(nob);
            });
            ToggleInventory();
        }
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
    public void To2048(NetworkObject nob){
        if (!nob.Owner.IsActive) { return; }
        SceneLookupData lookup;
        lookup = new SceneLookupData(_stackedSceneHandle,"HomeScene");
        SceneLoadData sld =  new SceneLoadData(lookup);
        sld.Options.AllowStacking = true;
        sld.MovedNetworkObjects = new NetworkObject[] {nob};
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadConnectionScenes(nob.Owner,sld);
    }
    public void ToTetris(NetworkObject nob){
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
