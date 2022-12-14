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

public sealed partial class SpecialKeyword :  Bright.Config.BeanBase 
{
    public SpecialKeyword(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["phrase"].IsString) { throw new SerializationException(); }  Phrase = _json["phrase"]; }
        PostInit();
    }

    public SpecialKeyword(int id, string phrase ) 
    {
        this.Id = id;
        this.Phrase = phrase;
        PostInit();
    }

    public static SpecialKeyword DeserializeSpecialKeyword(JSONNode _json)
    {
        return new keyword.SpecialKeyword(_json);
    }

    /// <summary>
    /// 序号
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 短语
    /// </summary>
    public string Phrase { get; private set; }

    public const int __ID__ = 1220147637;
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
        + "Phrase:" + Phrase + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}
