using FishNet.Object;
using UnityEngine;

public class OwnerAudioListener : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();

        // 确保只有拥有者才添加AudioListener组件
        if (base.IsOwner)
        {
            AudioListener listener = GetComponent<AudioListener>();
            if (listener == null)
            {
                // 如果没有AudioListener组件，则添加一个
                listener = gameObject.AddComponent<AudioListener>();
            }
        }
    }

    public override void OnStopClient()
    {
        base.OnStopClient();

        // 当客户端停止时，如果是Owner，则移除AudioListener组件
        if (base.IsOwner)
        {
            AudioListener listener = GetComponent<AudioListener>();
            if (listener != null)
            {
                Destroy(listener);
            }
        }
    }
}
