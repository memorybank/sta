//using java.io;
//using edu.stanford.nlp.process;
//using edu.stanford.nlp.ling;
//using edu.stanford.nlp.trees;
//using edu.stanford.nlp.parser.lexparser;
//using Console = System.Console;
//using UnityEngine;
//using System;

//namespace Playa.NLP
//{
//    //deprecated
//    class Program
//    {
//        public static void Main()
//        {
//            // Path to models extracted from `stanford-parser-4.2.0-models.jar`
//            //var jarRoot = "nlp.stanford.edu\\stanford-parser-4.2.0\\models";
//            //var jarRoot = "D:\\szh\\demo\\Assets\\StreamingAssets\\nlp.stanford.edu\\stanford-parser-4.2.0-models";
//            var jarRoot = ".\\Assets\\StreamingAssets\\nlp.stanford.edu\\stanford-parser-4.2.0-models";
//            var modelsDirectory = jarRoot + "\\edu\\stanford\\nlp\\models";

//            // Loading english PCFG parser from file
//            var lp = LexicalizedParser.loadModel(modelsDirectory + "\\lexparser\\chineseFactored.ser.gz");

//            // This sample shows parsing a list of correctly tokenized words
//            var sent = new[] { "我", "去", "大润发", "买", "鱼", "。" };
//            Debug.Log("已分词: "+ String.Join("，", sent));
//            var rawWords = SentenceUtils.toCoreLabelList(sent);
//            var tree = lp.apply(rawWords);
//            tree.pennPrint();
//            Debug.Log(tree);

//            // This option shows loading and using an explicit tokenizer
//            var sent2 = "我去大润发买鱼。";
//            Debug.Log("未分词: " + sent2);
//            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
//            var sent2Reader = new StringReader(sent2);
//            var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
//            sent2Reader.close();
//            var tree2 = lp.apply(rawWords2);

//            // Extract dependencies from lexical tree
//            //var tlp = new PennTreebankLanguagePack();
//            //var gsf = tlp.grammaticalStructureFactory();
//            //var gs = gsf.newGrammaticalStructure(tree2);
//            //var tdl = gs.typedDependenciesCCprocessed();
//            //Console.WriteLine("\n{0}\n", tdl);

//            // Extract collapsed dependencies from parsed tree
//            //var tp = new TreePrint("penn,typedDependenciesCollapsed");
//            //tp.printTree(tree2);
//            Debug.Log(tree2);
//        }
//    }
//}

