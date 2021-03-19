using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CameraSystem : ISystem
{
    private VirtualCameraController GetCameraController()
    {
        return UnityEngine.Object.FindObjectOfType<VirtualCameraController>();
    }

    public void Load(BinaryReader reader)
    {
        var x = reader.ReadSingle();
        var y = reader.ReadSingle();

        var controller = GetCameraController();
        controller.MoveTo(new Vector2(x, y));
    }

    public void Save(BinaryWriter writer)
    {
        var controller = GetCameraController();
        var pos = controller.GetPosition();
        writer.Write(pos.x);
        writer.Write(pos.y);
    }

    public void Init()
    {

    }

    public void Update()
    {
        
    }
}
