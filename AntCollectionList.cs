using Godot;
using System.Diagnostics;

public partial class AntCollectionList : ItemList
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Debug.WriteLine("AntCollectionList._Ready()");
        Name = "AntCollectionList";
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void AddDevice(string device, Texture2D texture)
    {
        Debug.WriteLine($"AntCollectionList.AddDevice({device})");
        CallDeferred(ItemList.MethodName.AddItem, device, texture);
    }

    public void RemoveDevice(string device)
    {
        Debug.WriteLine($"AntCollectionList.RemoveDevice({device})");
        int index = FindItemIndex(device);
        if (index >= 0)
        {
            CallDeferred(ItemList.MethodName.RemoveItem, index);
        }
    }

    public void ClearDevices()
    {
        Debug.WriteLine("AntCollectionList.ClearDevices()");
        Clear();
    }

    public void UpdateDevice(string device, Texture2D texture)
    {
        Debug.WriteLine($"AntCollectionList.UpdateDevice({device})");
        int index = FindItemIndex(device);
        if (index >= 0)
        {
            CallDeferred(ItemList.MethodName.SetItemText, index, device);
            CallDeferred(ItemList.MethodName.SetItemIcon, index, texture);
        }
    }

    private int FindItemIndex(string device)
    {
        for (int i = 0; i < GetItemCount(); i++)
        {
            if (GetItemText(i) == device)
            {
                return i;
            }
        }
        return -1;
    }
}
