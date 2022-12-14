//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using Bright.Serialization;
using SimpleJSON;

namespace cfg
{
   
public sealed partial class Tables
{
    public gesture.TbGestureMark TbGestureMark {get; }
    public gesture.TbGestureSequence TbGestureSequence {get; }
    public keyword.TbKeywordRegex TbKeywordRegex {get; }
    public keyword.TbKeywordRules TbKeywordRules {get; }
    public keyword.TbSpecialKeyword TbSpecialKeyword {get; }

    public Tables(System.Func<string, JSONNode> loader)
    {
        var tables = new System.Collections.Generic.Dictionary<string, object>();
        TbGestureMark = new gesture.TbGestureMark(loader("gesture_tbgesturemark")); 
        tables.Add("gesture.TbGestureMark", TbGestureMark);
        TbGestureSequence = new gesture.TbGestureSequence(loader("gesture_tbgesturesequence")); 
        tables.Add("gesture.TbGestureSequence", TbGestureSequence);
        TbKeywordRegex = new keyword.TbKeywordRegex(loader("keyword_tbkeywordregex")); 
        tables.Add("keyword.TbKeywordRegex", TbKeywordRegex);
        TbKeywordRules = new keyword.TbKeywordRules(loader("keyword_tbkeywordrules")); 
        tables.Add("keyword.TbKeywordRules", TbKeywordRules);
        TbSpecialKeyword = new keyword.TbSpecialKeyword(loader("keyword_tbspecialkeyword")); 
        tables.Add("keyword.TbSpecialKeyword", TbSpecialKeyword);
        PostInit();

        TbGestureMark.Resolve(tables); 
        TbGestureSequence.Resolve(tables); 
        TbKeywordRegex.Resolve(tables); 
        TbKeywordRules.Resolve(tables); 
        TbSpecialKeyword.Resolve(tables); 
        PostResolve();
    }

    public void TranslateText(System.Func<string, string, string> translator)
    {
        TbGestureMark.TranslateText(translator); 
        TbGestureSequence.TranslateText(translator); 
        TbKeywordRegex.TranslateText(translator); 
        TbKeywordRules.TranslateText(translator); 
        TbSpecialKeyword.TranslateText(translator); 
    }
    
    partial void PostInit();
    partial void PostResolve();
}

}
