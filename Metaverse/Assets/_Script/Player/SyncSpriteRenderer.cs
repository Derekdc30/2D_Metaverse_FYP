using FishNet.Object;
using FishNet.Object.Synchronizing;
using UnityEngine;

public class SyncSpriteRenderer : NetworkBehaviour
{
    [SerializeField]
    private SpriteRenderer spriteRenderer;

    // Assume you have a way to reference sprites by some kind of ID
    [SyncVar(OnChange = nameof(OnSpriteIdChanged))]
    private string currentSpriteId;

    private void ChangeSprite(string newSpriteId)
    {
        if (IsServer) // Check if we are on the server
        {
            currentSpriteId = newSpriteId; // This will trigger OnSpriteIdChanged across all clients
        }
    }

    private void OnSpriteIdChanged(string oldId, string newId, bool isServer)
    {
        // Update the sprite based on newId
        // This method needs to correctly fetch and apply the sprite based on the ID
        spriteRenderer.sprite = GetSpriteById(newId);
    }

    private Sprite GetSpriteById(string id)
    {
        // Implement your method to fetch the sprite based on the ID
        // This could be a lookup in a dictionary, an array, or something else
        return null;
    }
}
