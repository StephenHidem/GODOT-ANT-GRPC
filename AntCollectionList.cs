using Godot;
using SmallEarthTech.AntPlus;
using System.Diagnostics;
using System.IO;

public partial class AntCollectionList : ItemList
{
    public void AddDevice(AntDevice device)
    {
        Debug.WriteLine($"AntCollectionList.AddDevice({device})");
        CallDeferred(ItemList.MethodName.AddItem, device.ToString(), CreateDeviceTexture(device));
    }

    public void RemoveDevice(AntDevice device)
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
        CallDeferred(ItemList.MethodName.Clear);
    }

    public void UpdateDevice(AntDevice device)
    {
        Debug.WriteLine($"AntCollectionList.UpdateDevice({device})");
        int index = FindItemIndex(device);
        if (index >= 0)
        {
            CallDeferred(ItemList.MethodName.SetItemText, index, device.ToString());
            CallDeferred(ItemList.MethodName.SetItemIcon, index, CreateDeviceTexture(device));
        }
    }

    private int FindItemIndex(AntDevice device)
    {
        for (int i = 0; i < GetItemCount(); i++)
        {
            if (GetItemText(i) == device.ToString())
            {
                return i;
            }
        }
        return -1;
    }

    private static Texture2D CreateDeviceTexture(AntDevice item)
    {
        using MemoryStream ms = new();
        item.DeviceImageStream.CopyTo(ms);
        ms.Position = 0;
        Image image = new();
        image.LoadPngFromBuffer(ms.ToArray());
        image.Resize(64, 64);
        return ImageTexture.CreateFromImage(image);
    }
}
