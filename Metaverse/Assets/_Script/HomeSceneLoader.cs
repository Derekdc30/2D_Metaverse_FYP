using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Managing.Logging;
using FishNet.Managing.Scened;
using UnityEngine.UI;

public class HomeSceneLoader : MonoBehaviour
{
    private int _stackedSceneHandle;
    public bool SceneStack = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        NetworkObject nob = other.GetComponent<NetworkObject>();
        if (nob != null)
        {
            LoadScene(nob, true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        NetworkObject nob = other.GetComponent<NetworkObject>();
        if (nob != null)
        {
            LoadScene(nob, false);
        }
    }

    void LoadScene(NetworkObject nob, bool entering)
    {
        if (!nob.Owner.IsActive) { return; }

        SceneLookupData lookup;
        if (this.tag == "TP_Home")
        {
            if (entering){
                InstanceFinder.SceneManager.UnloadConnectionScenes(nob.Owner, new SceneUnloadData(new SceneLookupData(_stackedSceneHandle, "MainScene")));
            }
            lookup = new SceneLookupData(_stackedSceneHandle, "HomeScene");
            
        }
        else if (this.tag == "TP_Main")
        {
            if (entering)
            {
                // Unload the home scene when the player enters it.
                InstanceFinder.SceneManager.UnloadConnectionScenes(nob.Owner, new SceneUnloadData(new SceneLookupData(_stackedSceneHandle, "HomeScene")));
            }
            lookup = new SceneLookupData(_stackedSceneHandle, "MainScene");
        }
        else
        {
            lookup = new SceneLookupData(_stackedSceneHandle, "MainScene");
        }

        SceneLoadData sld = new SceneLoadData(lookup);
        sld.Options.AllowStacking = true;
        sld.MovedNetworkObjects = new NetworkObject[] { nob };
        sld.ReplaceScenes = ReplaceOption.All;
        InstanceFinder.SceneManager.LoadConnectionScenes(nob.Owner, sld);
    }

    private void Start()
    {
        InstanceFinder.SceneManager.OnLoadEnd += SceneManager_OnLoadEnd;
    }

    private void OnDestroy()
    {
        if (InstanceFinder.SceneManager != null) InstanceFinder.SceneManager.OnLoadEnd -= SceneManager_OnLoadEnd;
    }

    private void SceneManager_OnLoadEnd(SceneLoadEndEventArgs obj)
    {
        if (!obj.QueueData.AsServer) return;
        if (!SceneStack) return;
        if (_stackedSceneHandle != 0) return;
        if (obj.LoadedScenes.Length > 0) _stackedSceneHandle = obj.LoadedScenes[0].handle;
    }
}
