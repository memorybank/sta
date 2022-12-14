//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using System.Collections.Generic;
using SimpleJSON;



namespace cfg.keyword
{

public sealed partial class KeywordRegex :  Bright.Config.BeanBase 
{
    public KeywordRegex(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["pattern"].IsString) { throw new SerializationException(); }  Pattern = _json["pattern"]; }
        PostInit();
    }

    public KeywordRegex(int id, string name, string pattern ) 
    {
        this.Id = id;
        this.Name = name;
        this.Pattern = pattern;
        PostInit();
    }

    public static KeywordRegex DeserializeKeywordRegex(JSONNode _json)
    {
        return new keyword.KeywordRegex(_json);
    }

    /// <summary>
    /// 序号
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 匹配模式（都是正则表达式，标点用中文版本，英文小写）
    /// </summary>
    public string Pattern { get; private set; }

    public const int __ID__ = -281661469;
    public override int GetTypeId() => __ID__;

    public  void Resolve(Dictionary<string, object> _tables)
    {
        PostResolve();
    }

    public  void TranslateText(System.Func<string, string, string> translator)
    {
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "Name:" + Name + ","
        + "Pattern:" + Pattern + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
