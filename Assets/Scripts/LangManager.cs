using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class LangManager : MonoBehaviour
{
    public static InstanceHelper<LangManager> GlobalLangManager;

    public Lang lang;

    private Dictionary<string, FieldInfo> _propertys = new Dictionary<string, FieldInfo>();

    private void Awake()
    {
        GlobalLangManager = new InstanceHelper<LangManager>(this);

        foreach (var field in typeof(Lang).GetFields())
        {
            if (field.FieldType != typeof(string))
                continue;
            _propertys[field.Name] = field;
        }
    }

    private string GetInner(string key)
    {
        var fieldInfo = _propertys[key];
        return fieldInfo.GetValue(lang) as string;
    }

    public static string Get(string key)
    {
        return GlobalLangManager.Get().GetInner(key);
    }

    public static Lang Current { get => GlobalLangManager.Get().lang; }
}
