using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ISystem
{
    void Update();

    void Init();

    void Save(BinaryWriter writer);
    void Load(BinaryReader reader);
}
