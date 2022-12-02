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



namespace cfg.gesture
{

public sealed partial class GestureSequence :  Bright.Config.BeanBase 
{
    public GestureSequence(JSONNode _json) 
    {
        { if(!_json["id"].IsNumber) { throw new SerializationException(); }  Id = _json["id"]; }
        { if(!_json["name"].IsString) { throw new SerializationException(); }  Name = _json["name"]; }
        { if(!_json["startPoint"].IsNumber) { throw new SerializationException(); }  StartPoint = (gesture.Type)_json["startPoint"].AsInt; }
        { var _json1 = _json["sequence"]; if(!_json1.IsArray) { throw new SerializationException(); } Sequence = new System.Collections.Generic.List<gesture.Type>(_json1.Count); foreach(JSONNode __e in _json1.Children) { gesture.Type __v;  { if(!__e.IsNumber) { throw new SerializationException(); }  __v = (gesture.Type)__e.AsInt; }  Sequence.Add(__v); }   }
        PostInit();
    }

    public GestureSequence(int id, string name, gesture.Type startPoint, System.Collections.Generic.List<gesture.Type> sequence ) 
    {
        this.Id = id;
        this.Name = name;
        this.StartPoint = startPoint;
        this.Sequence = sequence;
        PostInit();
    }

    public static GestureSequence DeserializeGestureSequence(JSONNode _json)
    {
        return new gesture.GestureSequence(_json);
    }

    /// <summary>
    /// 序号
    /// </summary>
    public int Id { get; private set; }
    /// <summary>
    /// 名字
    /// </summary>
    public string Name { get; private set; }
    /// <summary>
    /// 起始点
    /// </summary>
    public gesture.Type StartPoint { get; private set; }
    /// <summary>
    /// 序列
    /// </summary>
    public System.Collections.Generic.List<gesture.Type> Sequence { get; private set; }

    public const int __ID__ = -1072881307;
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
        + "StartPoint:" + StartPoint + ","
        + "Sequence:" + Bright.Common.StringUtil.CollectionToString(Sequence) + ","
        + "}";
    }
    
    partial void PostInit();
    partial void PostResolve();
}
}