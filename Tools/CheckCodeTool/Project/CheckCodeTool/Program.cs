using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CheckCodeTool
{
    class Program
    {
        private List<ClassPoolObjInfo> m_infoList = new List<ClassPoolObjInfo>();

        private List<string> m_skipBaseTypeList = new List<string>()
        {
            "IPointerClickHandler",
            "IPointerDownHandler",
            "IPointerUpHandler",
            "IPointerEnterHandler",
            "IPointerExitHandler",
            "IDragHandler",
            "ISerializationCallbackReceiver",
            "Editor",
            "UIBehaviour",
            "Image",
            "BaseMeshEffect",
            "LayoutGroup",
            "MonoBehaviour",
            "ScriptableObject",
            "PropertyDrawer",
        };

        private class ClassPoolObjInfo
        {
            public string className;
            public string fullName;
            public List<ClassDeclarationSyntax> classSyntaxList = new List<ClassDeclarationSyntax>();
            public ClassPoolObjInfo parent;
            public List<ClassPoolObjInfo> childList = new List<ClassPoolObjInfo>(); 

            public override string ToString()
            {
                return fullName;
            }
        }

        static void Main(string[] args)
        {
            List<string> logList = new List<string>();
            Program program = new Program();
            string path1 = @"E:\projects\MonMoose\MonMoose\Assets\Assemblies";
            string path2 = @"E:\projects\MonMoose\MonMoose\Assets\Scripts";
            List<string> folderPathList = new List<string>() {path1, path2};
            program.AnalyzeAllFiles(folderPathList, logList);
            foreach (var log in logList)
            {
                Console.WriteLine(log);
            }
            while (true) ;
        }

        private void AnalyzeAllFiles(List<string> folderPathList, List<string> logList)
        {
            List<FileInfo> fileInfoList = GetFileInfoList(folderPathList);
            foreach (var fileInfo in fileInfoList)
            {
                AnalyzeFile(fileInfo);
            }
            foreach (var classInfo in m_infoList)
            {
                foreach (var classSyntax in classInfo.classSyntaxList)
                {
                    if(classSyntax.BaseList != null)
                    {
                        foreach(var childNode in classSyntax.BaseList.ChildNodes())
                        {
                            string childName = string.Empty;
                            if(childNode is SimpleBaseTypeSyntax)
                            {
                                var simpleBaseSyntax = childNode as SimpleBaseTypeSyntax;
                                if (simpleBaseSyntax.Type is GenericNameSyntax)
                                {
                                    var genTypeSyntax = simpleBaseSyntax.Type as GenericNameSyntax;
                                    childName = string.Format("{0}<{1}>", genTypeSyntax.Identifier.ToString(), genTypeSyntax.TypeArgumentList.ChildNodes().Count());
                                }
                                else if(simpleBaseSyntax.Type is IdentifierNameSyntax)
                                {
                                    childName = (simpleBaseSyntax.Type as IdentifierNameSyntax).ToString();
                                }
                                else if (simpleBaseSyntax.Type is AliasQualifiedNameSyntax)
                                {
                                    
                                }
                                else
                                {
                                    Console.WriteLine("Find Unknown Type:" + simpleBaseSyntax.Type.ToString());
                                }
                            }
                            var baseClassInfoList = GetInfoListByClassName(childName);
                            if (baseClassInfoList.Count == 1)
                            {
                                var baseClassInfo = baseClassInfoList[0];
                                classInfo.parent = baseClassInfo;
                                baseClassInfo.childList.Add(classInfo);
                            }
                            else if (baseClassInfoList.Count > 1)
                            {
                                Console.WriteLine("Find Same Type:" + childName);
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(childName) || m_skipBaseTypeList.Find(s => s == childName) != null)
                                {

                                }
                                else
                                {
                                    Console.WriteLine("Find Unknown Base Type:" + childName);
                                }
                            }
                        }
                    }
                }
            }
            var poolObjClassInfo = GetInfoByClassName("IClassPoolObj");
            if(poolObjClassInfo != null)
            {
                Check(poolObjClassInfo, logList);
            }
        }

        private void Check(ClassPoolObjInfo info, List<string> logList)
        {
            foreach (var syntax in info.classSyntaxList)
            {
                Check(info, syntax, logList);
            }
            foreach (var child in info.childList)
            {
                Check(child, logList);
            }
        }

        private void Check(ClassPoolObjInfo info, ClassDeclarationSyntax syntax, List<string> logList)
        {
            List<string> fieldNameList = new List<string>();
            List<string> releaseNameList = new List<string>();
            foreach (var member in syntax.Members)
            {
                if (member is FieldDeclarationSyntax)
                {
                    var fieldSyntax = member as FieldDeclarationSyntax;
                    if (fieldSyntax.Modifiers.Any(m => m.Text == "static" || m.Text == "const"))
                    {
                        continue;
                    }
                    foreach (var variable in fieldSyntax.Declaration.Variables)
                    {
                        string variableStr = variable.ToString();
                        if (variableStr.Contains("="))
                        {
                            string[] splits = variableStr.Split('=');
                            fieldNameList.Add(splits[0].Trim());
                        }
                        else
                        {
                            fieldNameList.Add(variableStr);
                        }
                    }
                }
                if (member is MethodDeclarationSyntax)
                {
                    var methodSyntax = member as MethodDeclarationSyntax;
                    if (methodSyntax.Identifier.ToString() != "OnRelease")
                    {
                        continue;
                    }
                    List<BlockSyntax> blockSyntaxList = new List<BlockSyntax>();
                    if(methodSyntax.Body != null)
                    {
                        blockSyntaxList.Add(methodSyntax.Body);
                    }
                    while (blockSyntaxList.Count > 0)
                    {
                        BlockSyntax blockSyntax = blockSyntaxList[0];
                        blockSyntaxList.RemoveAt(0);

                        foreach (var statement in blockSyntax.Statements)
                        {
                            if (statement is ExpressionStatementSyntax)
                            {
                                var expSyntax = statement as ExpressionStatementSyntax;
                                if (expSyntax.Expression is InvocationExpressionSyntax)
                                {
                                    var invokeSyntax = expSyntax.Expression as InvocationExpressionSyntax;
                                    if(invokeSyntax.Expression is MemberAccessExpressionSyntax)
                                    {
                                        var memberAccSyntax = invokeSyntax.Expression as MemberAccessExpressionSyntax;
                                        string fieldName = memberAccSyntax.Expression.ToString();
                                        string methodName = memberAccSyntax.Name.ToString();
                                        //if (methodName == "Clear" || methodName == "ReleaseAll" || methodName == "Release")
                                        //{
                                        //    releaseNameList.Add(fieldName);
                                        //}
                                        releaseNameList.Add(fieldName);
                                    }
                                }
                                else if (expSyntax.Expression is AssignmentExpressionSyntax)
                                {
                                    var assignSyntax = expSyntax.Expression as AssignmentExpressionSyntax;
                                    string fieldName = assignSyntax.Left.ToString();
                                    releaseNameList.Add(fieldName);
                                }
                            }
                            else if(statement is IfStatementSyntax)
                            {
                                var ifSyntax = statement as IfStatementSyntax;
                                blockSyntaxList.Add(ifSyntax.Statement as BlockSyntax);
                            }
                        }
                    }
                }
            }
            for(int i=0;i< fieldNameList.Count; ++i)
            {
                string fieldName = fieldNameList[i];
                if (releaseNameList.Find(s => s == fieldNameList[i]) == null)
                {
                    logList.Add(string.Format("{0} {1}", info.fullName, fieldName));
                }
            }
        }

        private void AnalyzeFile(FileInfo fileInfo)
        {
            using (StreamReader sr = new StreamReader(fileInfo.FullName))
            {
                AnalyzeCodeStr(sr.ReadToEnd());
            }
        }

        private void AnalyzeCodeStr(string str)
        {
            var tree = CSharpSyntaxTree.ParseText(str);

            var root = (CompilationUnitSyntax)tree.GetRoot();
            foreach (var member in root.Members)
            {
                AnalyzeMember(member);
            }
        }

        private void AnalyzeMember(MemberDeclarationSyntax memberSyntax)
        {
            if (memberSyntax is NamespaceDeclarationSyntax)
            {
                AnalyzeNamespace(memberSyntax as NamespaceDeclarationSyntax);
            }
            else if (memberSyntax is ClassDeclarationSyntax)
            {
                AnalyzeClass(memberSyntax as ClassDeclarationSyntax);
            }
            else if (memberSyntax is InterfaceDeclarationSyntax)
            {
                AnalyzeInterface(memberSyntax as InterfaceDeclarationSyntax);
            }
            else if (memberSyntax is MethodDeclarationSyntax)
            {
                AnalyzeMethod(memberSyntax as MethodDeclarationSyntax);
            }
        }

        private void AnalyzeNamespace(NamespaceDeclarationSyntax namespaceSyntax)
        {
            foreach (var member in namespaceSyntax.Members)
            {
                AnalyzeMember(member);
            }
        }

        private void AnalyzeClass(ClassDeclarationSyntax classSyntax)
        {
            if (classSyntax.Modifiers.Any(m => m.Text == "static" || m.Text == "const"))
            {
                return;
            }
            string className = GetName(classSyntax);
            string fullName = GetFullName(classSyntax);
            ClassPoolObjInfo info = GetInfoByFullName(fullName);
            if (info == null)
            {
                info = new ClassPoolObjInfo()
                {
                    className = className,
                    fullName = fullName,
                };
                m_infoList.Add(info);
            }
            info.classSyntaxList.Add(classSyntax);
        }

        private void AnalyzeInterface(InterfaceDeclarationSyntax syntax)
        {
            string className = GetName(syntax);
            string fullName = GetFullName(syntax);
            ClassPoolObjInfo info = GetInfoByFullName(fullName);
            if (info == null)
            {
                info = new ClassPoolObjInfo()
                {
                    className = className,
                    fullName = fullName,
                };
                m_infoList.Add(info);
            }
        }


        private string GetFullName(TypeDeclarationSyntax syntax)
        {
            SyntaxNode node = syntax;
            string fullName = string.Empty;
            while (node != null)
            {
                string nodeName = string.Empty;
                bool success = false;
                if (node is NamespaceDeclarationSyntax)
                {
                    nodeName = GetName(node as NamespaceDeclarationSyntax);
                    success = true;
                }
                else if (node is ClassDeclarationSyntax)
                {
                    nodeName = GetName(node as ClassDeclarationSyntax);
                    success = true;
                }
                else if (node is InterfaceDeclarationSyntax)
                {
                    nodeName = GetName(node as InterfaceDeclarationSyntax);
                    success = true;
                }
                if (success)
                {
                    if (string.IsNullOrEmpty(fullName))
                    {
                        fullName = nodeName;
                    }
                    else
                    {
                        fullName = nodeName + "." + fullName;
                    }
                    node = node.Parent;
                }
                else
                {
                    break;
                }
            }
            return fullName;
        }

        private string GetName(NamespaceDeclarationSyntax syntax)
        {
            return syntax.Name.ToString();
        }

        private string GetName(ClassDeclarationSyntax syntax)
        {
            if (syntax.TypeParameterList != null && syntax.TypeParameterList.ChildNodes().Any())
            {
                return string.Format("{0}<{1}>", syntax.Identifier.ToString(), syntax.TypeParameterList.ChildNodes().Count());
            }
            return syntax.Identifier.ToString();
        }

        private string GetName(InterfaceDeclarationSyntax syntax)
        {
            if (syntax.TypeParameterList != null && syntax.TypeParameterList.ChildNodes().Any())
            {
                return string.Format("{0}<{1}>", syntax.Identifier.ToString(), syntax.TypeParameterList.ChildNodes().Count());
            }
            return syntax.Identifier.ToString();
        }

        private void AnalyzeMethod(MethodDeclarationSyntax methodSyntax)
        {
            int a = 0;
        }

        private List<FileInfo> GetFileInfoList(List<string> folderPathList)
        {
            List<FileInfo> list = new List<FileInfo>();
            foreach (var folderPath in folderPathList)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderPath);
                if (directoryInfo.Exists)
                {
                    foreach (var fileInfo in directoryInfo.GetFiles("*.cs", SearchOption.AllDirectories))
                    {
                        if (!list.Contains(fileInfo))
                        {
                            list.Add(fileInfo);
                        }
                    }
                }
            }
            return list;
        }

        private ClassPoolObjInfo GetInfoByFullName(string fullName)
        {
            foreach (var info in m_infoList)
            {
                if (info.fullName == fullName)
                {
                    return info;
                }
            }
            return null;
        }

        private ClassPoolObjInfo GetInfoByClassName(string className)
        {
            foreach (var info in m_infoList)
            {
                if (info.className == className)
                {
                    return info;
                }
            }
            return null;
        }

        private List<ClassPoolObjInfo> GetInfoListByClassName(string className)
        {
            List<ClassPoolObjInfo> list = new List<ClassPoolObjInfo>();
            foreach (var info in m_infoList)
            {
                if (info.className == className)
                {
                    list.Add(info);
                }
            }
            return list;
        }
    }
}
