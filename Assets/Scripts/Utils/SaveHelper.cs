using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class SaveHelper
{
    /// <summary>
    /// 保存NamedScriptableObject
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="obj"></param>
    public static void Write(BinaryWriter writer, NamedScriptableObject obj)
    {
        writer.Write(obj == null ? 0 : obj.NameHash);
    }

    /// <summary>
    /// 保存输入建筑
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="obj"></param>
    public static void Write(BinaryWriter writer, IBuildingCanInputItem obj)
    {
        Write(writer, obj as BuildingBase);
    }

    /// <summary>
    /// 保存输出建筑
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="obj"></param>
    public static void Write(BinaryWriter writer, IBuildingCanOutputItem obj)
    {
        Write(writer, obj as BuildingBase);
    }

    /// <summary>
    /// 保存建筑
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="obj"></param>
    public static void Write(BinaryWriter writer, BuildingBase building)
    {
        if (building == null)
            writer.Write(-1);
        else
            writer.Write(building.id);
    }

    /// <summary>
    /// 保存物品堆
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="obj"></param>
    public static void Write(BinaryWriter writer, ItemStack stack)
    {
        Write(writer, stack.item);
        writer.Write(stack.count);
    }

    /// <summary>
    /// 加载NamedScriptableObject
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T ReadScriptable<T>(BinaryReader reader) where T : NamedScriptableObject
    {
        return NamedScriptableObject.Get<T>(reader.ReadInt32());
    }

    /// <summary>
    /// 加载输入建筑
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static IBuildingCanInputItem ReadBuildingCanInput(BinaryReader reader)
    {
        return ReadBuilding(reader) as IBuildingCanInputItem;
    }

    /// <summary>
    /// 加载输出建筑
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static IBuildingCanOutputItem ReadBuildingCanOutput(BinaryReader reader)
    {
        return ReadBuilding(reader) as IBuildingCanOutputItem;
    }

    /// <summary>
    /// 加载建筑
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static BuildingBase ReadBuilding(BinaryReader reader)
    {
        var id = reader.ReadInt32();
        var building = GameMap.GlobalMap.Get().GetBuildingById(id);
        return building;
    }

    /// <summary>
    /// 读取物品堆
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="obj"></param>
    public static ItemStack ReadItemStack(BinaryReader reader)
    {
        var stack = new ItemStack
        {
            item = ReadScriptable<ItemInfo>(reader),
            count = reader.ReadInt32()
        };
        return stack;
    }
}
