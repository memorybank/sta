//using java.io;
//using edu.stanford.nlp.process;
//using edu.stanford.nlp.ling;
//using edu.stanford.nlp.trees;
//using edu.stanford.nlp.parser.lexparser;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using System;
//using System.Linq;
//using UnityEngine.UI;

//using Playa.Common;

//namespace Playa.NLP
//{
//    public class IdeationalUnitStanfordNLPGenerator : IdeationalUnitGenerator
//    {
//        private LexicalizedParser lp;

//        private Dictionary<string, UserTextGrammarType> _GrammarTagTypeDict;

//        public Toggle rule1;
//        public Toggle rule2;
//        public Toggle rule4;
//        public Toggle rule5;

//        public override void Init()
//        {
//            lp = LexicalizedParser.loadModel(_ModelFilePath);

//            InitGrammarTypeTagDict();
//        }

//        private void InitGrammarTypeTagDict()
//        {
//            _GrammarTagTypeDict = new Dictionary<string, UserTextGrammarType>();
//            //标点
//            _GrammarTagTypeDict["PU"] = UserTextGrammarType.Invalid;
//            //主谓宾
//            _GrammarTagTypeDict["NN"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["NR"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["PN"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["VC"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["VE"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["VV"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["AS"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["FW"]= UserTextGrammarType.ZhuWeiBing;
//            _GrammarTagTypeDict["LC"]= UserTextGrammarType.ZhuWeiBing;
//            //修饰词
//            _GrammarTagTypeDict["AD"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["CD"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["DT"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["JJ"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["LB"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["M"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["MSP"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["NT"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["OD"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["P"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["SB"]= UserTextGrammarType.XiuShiYu;
//            _GrammarTagTypeDict["VA"]= UserTextGrammarType.XiuShiYu;
//            //得的地
//            _GrammarTagTypeDict["DEC"]= UserTextGrammarType.DeDeDi;
//            _GrammarTagTypeDict["DER"]= UserTextGrammarType.DeDeDi;
//            _GrammarTagTypeDict["DEV"]= UserTextGrammarType.DeDeDi;
//            //连词
//            _GrammarTagTypeDict["CC"]= UserTextGrammarType.LianCi;
//            _GrammarTagTypeDict["CS"]= UserTextGrammarType.LianCi;
//            _GrammarTagTypeDict["DEG"]= UserTextGrammarType.LianCi;
//            //插入语
//            _GrammarTagTypeDict["ETC"]= UserTextGrammarType.ChaRuYu;
//            _GrammarTagTypeDict["IJ"]= UserTextGrammarType.ChaRuYu;
//            _GrammarTagTypeDict["ON"]= UserTextGrammarType.ChaRuYu;
//            _GrammarTagTypeDict["SP"]= UserTextGrammarType.ChaRuYu;
//        }

//        public override IdeationalUnit GenerateUnitWithTextAndDuration(string[] sent, float[] duration)
//        {
//            edu.stanford.nlp.trees.Tree tree = Parse(sent);
//            return _GenUnitByTree(tree, duration);
//        }

//        public override IdeationalUnit GenerateUnitWithTextAndDuration(string sent2, float duration2)
//        {
//            edu.stanford.nlp.trees.Tree tree = Parse(sent2);
//            return _GenUnitByTree(tree, duration2);
//        }

//        public edu.stanford.nlp.trees.Tree Parse(string[] sent)
//        {
//            var rawWords = SentenceUtils.toCoreLabelList(sent);
//            // Debug.Log(rawWords);
//            if (!rawWords.isEmpty())
//            {
//                var tree = lp.apply(rawWords);
//                tree.pennPrint();
//                // Debug.Log(tree);
//                // PrintTreeElements(tree);
//                return tree;
//            }
//            return null;
//        }

//        public edu.stanford.nlp.trees.Tree Parse(string sent2)
//        {
//            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
//            var sent2Reader = new StringReader(sent2);
//            var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
//            if (!rawWords2.isEmpty())
//            {
//                sent2Reader.close();
//                var tree2 = lp.apply(rawWords2);
//                Debug.Log(tree2);
//                // PrintTreeElements(tree2);
//                return tree2;
//            }
//            return null;
//        }

//        private void PrintTreeElements(edu.stanford.nlp.trees.Tree t)
//        {
//            if (t == null)
//            {
//                return;
//            }

//            Queue queue = new Queue();
//            queue.Enqueue(t);
//            while (queue.Count > 0)
//            {
//                edu.stanford.nlp.trees.Tree ti = (edu.stanford.nlp.trees.Tree)queue.Dequeue();
//                Debug.Log("tilabel" + ti.label().ToString());
//                edu.stanford.nlp.trees.Tree[] array = ti.children();
//                Debug.Log("child length" + array.Length);
//                for (int i = 0; i < array.Length; i++)
//                {
//                    queue.Enqueue(array[i]);
//                }
//            }
//        }

//        private IdeationalUnit _GenUnitByTree(edu.stanford.nlp.trees.Tree t, float[] duration)
//        {
//            if (t == null)
//            {
//                return null;
//            }

//            IdeationalUnit ideationalUnit = new IdeationalUnit();
//            ideationalUnit.Phrases = new List<Phrase>();
//            List<edu.stanford.nlp.trees.Tree> leafList = new List<edu.stanford.nlp.trees.Tree>();

//            Queue queue = new Queue();
//            queue.Enqueue(t);

//            while (queue.Count > 0)
//            {
//                edu.stanford.nlp.trees.Tree ti = (edu.stanford.nlp.trees.Tree)queue.Dequeue();
//                if (ti.isLeaf())
//                {
//                    _ProcessGrammarTreeLeaf(ideationalUnit, leafList, ti, t, duration);
//                }

//                edu.stanford.nlp.trees.Tree[] array = ti.children();
//                for (int i = 0; i < array.Length; i++)
//                {
//                    queue.Enqueue(array[i]);
//                }
//            }
//            _DumpLeafListToIdeationalUnit(ideationalUnit, leafList, duration);

//            return ideationalUnit;
//        }

//        private IdeationalUnit _GenUnitByTree(edu.stanford.nlp.trees.Tree t, float duration2)
//        {
//            if (t == null)
//            {
//                return null;
//            }

//            int durationCount = 0;
//            List<edu.stanford.nlp.trees.Tree> leafsWithNull = (List<edu.stanford.nlp.trees.Tree>)t.getLeaves();
//            foreach (edu.stanford.nlp.trees.Tree t1 in leafsWithNull)
//            {
//                durationCount++;
//            }

//            float[] duration = new float[durationCount];
//            for (int i = 0; i < durationCount; i++)
//            {
//                duration[i] = duration2 / durationCount;
//            }

//            return _GenUnitByTree(t, duration);
//        }

//        private void _ProcessGrammarTreeLeaf(IdeationalUnit ideationalUnit, List<edu.stanford.nlp.trees.Tree> leafList, edu.stanford.nlp.trees.Tree tree, edu.stanford.nlp.trees.Tree root, float[] duration)
//        {
//            string label = tree.parent(root).label().ToString();
//            UserTextGrammarType v;
//            _GrammarTagTypeDict.TryGetValue(label, out v);
//            switch (v)
//            {
//                case UserTextGrammarType.Invalid:
//                    return;
//                case UserTextGrammarType.ZhuWeiBing:
//                    if (rule1.isOn)
//                    {
//                        if (leafList.Count > 0 && _IsNoun(leafList[leafList.Count - 1].parent(root).label().ToString()) && _IsNoun(label))
//                        {
//                            // 双宾
//                            _DumpLeafListToIdeationalUnit(ideationalUnit, leafList, duration);
//                        }
//                        break;
//                    }
//                    goto default;
//                case UserTextGrammarType.XiuShiYu:
//                    if (rule2.isOn)
//                    {
//                        if (leafList.Count > 0)
//                        {
//                            // 修饰语
//                            _DumpLeafListToIdeationalUnit(ideationalUnit, leafList, duration);
//                        }
//                        break;
//                    }
//                    goto default;
//                case UserTextGrammarType.LianCi:
//                    if (rule4.isOn)
//                    {
//                        if (leafList.Count > 0)
//                        {
//                            // 连词
//                            _DumpLeafListToIdeationalUnit(ideationalUnit, leafList, duration);
//                        }
//                        break;
//                    }
//                    goto default;
//                case UserTextGrammarType.ChaRuYu:
//                    if (rule5.isOn)
//                    {
//                        if (leafList.Count > 0)
//                        {
//                            // 插入语
//                            _DumpLeafListToIdeationalUnit(ideationalUnit, leafList, duration);
//                        }
//                        // 插入语自身成为短语
//                        ideationalUnit.Phrases.Add(new Phrase(tree.label().ToString(), duration[0]));
//                        duration = duration.Skip(1).ToArray();
//                        return;
//                    }
//                    goto default;
//                default:
//                    break;
//            }
//            leafList.Add(tree);
//        }

//        private void _DumpLeafListToIdeationalUnit(IdeationalUnit ideationalUnit, List<edu.stanford.nlp.trees.Tree> leafList, float[] duration)
//        {
//            string resultPhrase = "";
//            float resultDuration = 0;
//            for (int i = 0; i < leafList.Count; i++)
//            {
//                if (duration.Length <= i)
//                {
//                    //todo 异常处理
//                    throw (new IndexOutOfRangeException("_DumpLeafListToIdeationalUnit"));
//                }

//                resultPhrase += leafList[i].label().ToString();
//                resultDuration += duration[i];
//            }

//            ideationalUnit.Phrases.Add(new Phrase(resultPhrase, resultDuration));
//            duration = duration.Skip(leafList.Count).ToArray();
//            leafList.Clear();
//            leafList.TrimExcess();
//        }

//        private bool _IsNoun(string label)
//        {
//            if (label == "NN" || label == "PN" || label == "NR")
//            {
//                return true;
//            }
//            return false;
//        }
//    }
//}
