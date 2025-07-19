using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace TestAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            string logFile = "test_analysis.log";
            using StreamWriter logWriter = new StreamWriter(logFile, false) { AutoFlush = true };

            Console.WriteLine("=== RecallTests 測試項目分析 ===");
            logWriter.WriteLine("=== RecallTests 測試項目分析 ===");
            logWriter.WriteLine($"分析時間: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            logWriter.WriteLine();

            string testsDirectory = "../RecallTests";
            if (!Directory.Exists(testsDirectory))
            {
                Console.WriteLine($"錯誤: 找不到 {testsDirectory} 目錄");
                logWriter.WriteLine($"錯誤: 找不到 {testsDirectory} 目錄");
                return;
            }

            // 只分析測試檔案，排除編譯產生的檔案
            var testFiles = Directory.GetFiles(testsDirectory, "*.cs", SearchOption.AllDirectories)
                .Where(f => !f.Contains("obj") && 
                           !f.Contains("bin") && 
                           !f.Contains("GlobalUsings.g.cs") &&
                           !f.Contains("AssemblyInfo.cs") &&
                           !f.Contains(".NETCoreApp,Version="))
                .ToArray();
            var testClasses = new List<TestClassInfo>();
            var testMethods = new List<TestMethodInfo>();

            Console.WriteLine($"找到 {testFiles.Length} 個 C# 檔案");
            logWriter.WriteLine($"找到 {testFiles.Length} 個 C# 檔案");

            foreach (var file in testFiles)
            {
                AnalyzeTestFile(file, testClasses, testMethods, logWriter);
            }

            // 生成統計報告
            GenerateReport(testClasses, testMethods, logWriter);

            Console.WriteLine($"\n分析完成！詳細報告已寫入 {logFile}");
        }

        static void AnalyzeTestFile(string filePath, List<TestClassInfo> testClasses, List<TestMethodInfo> testMethods, StreamWriter logWriter)
        {
            string relativePath = Path.GetRelativePath("RecallTests", filePath);
            Console.WriteLine($"分析檔案: {relativePath}");
            logWriter.WriteLine($"\n--- 分析檔案: {relativePath} ---");

            string[] lines = File.ReadAllLines(filePath);
            string currentNamespace = "";
            string currentClass = "";
            int lineNumber = 0;

            foreach (string line in lines)
            {
                lineNumber++;

                // 檢查命名空間
                var namespaceMatch = Regex.Match(line, @"namespace\s+(\S+)");
                if (namespaceMatch.Success)
                {
                    currentNamespace = namespaceMatch.Groups[1].Value;
                    logWriter.WriteLine($"  命名空間: {currentNamespace}");
                }

                // 檢查測試類別
                var classMatch = Regex.Match(line, @"public\s+class\s+(\w+)");
                if (classMatch.Success)
                {
                    currentClass = classMatch.Groups[1].Value;
                    var testClass = new TestClassInfo
                    {
                        Name = currentClass,
                        Namespace = currentNamespace,
                        FilePath = relativePath,
                        LineNumber = lineNumber
                    };
                    testClasses.Add(testClass);
                    logWriter.WriteLine($"  測試類別: {currentClass} (第 {lineNumber} 行)");
                }

                // 檢查測試方法
                var testMethodMatch = Regex.Match(line, @"public\s+void\s+(\w+)\s*\(");
                if (testMethodMatch.Success && !string.IsNullOrEmpty(currentClass))
                {
                    var methodName = testMethodMatch.Groups[1].Value;
                    var testMethod = new TestMethodInfo
                    {
                        Name = methodName,
                        ClassName = currentClass,
                        Namespace = currentNamespace,
                        FilePath = relativePath,
                        LineNumber = lineNumber
                    };
                    testMethods.Add(testMethod);
                    logWriter.WriteLine($"    測試方法: {methodName} (第 {lineNumber} 行)");
                }
            }
        }

        static void GenerateReport(List<TestClassInfo> testClasses, List<TestMethodInfo> testMethods, StreamWriter logWriter)
        {
            logWriter.WriteLine("\n" + new string('=', 60));
            logWriter.WriteLine("測試項目統計報告");
            logWriter.WriteLine(new string('=', 60));

            // 按命名空間分組
            var namespaceGroups = testMethods.GroupBy(m => m.Namespace).OrderBy(g => g.Key);
            
            logWriter.WriteLine($"\n總計:");
            logWriter.WriteLine($"  測試類別: {testClasses.Count} 個");
            logWriter.WriteLine($"  測試方法: {testMethods.Count} 個");

            foreach (var namespaceGroup in namespaceGroups)
            {
                logWriter.WriteLine($"\n命名空間: {namespaceGroup.Key}");
                logWriter.WriteLine($"  測試方法數量: {namespaceGroup.Count()} 個");

                var classGroups = namespaceGroup.GroupBy(m => m.ClassName).OrderBy(g => g.Key);
                foreach (var classGroup in classGroups)
                {
                    logWriter.WriteLine($"\n  類別: {classGroup.Key}");
                    logWriter.WriteLine($"    測試方法數量: {classGroup.Count()} 個");
                    
                    foreach (var method in classGroup.OrderBy(m => m.Name))
                    {
                        logWriter.WriteLine($"      - {method.Name}");
                    }
                }
            }

            // 按檔案分組
            logWriter.WriteLine($"\n按檔案分組:");
            var fileGroups = testMethods.GroupBy(m => m.FilePath).OrderBy(g => g.Key);
            foreach (var fileGroup in fileGroups)
            {
                logWriter.WriteLine($"\n檔案: {fileGroup.Key}");
                logWriter.WriteLine($"  測試方法數量: {fileGroup.Count()} 個");
                
                foreach (var method in fileGroup.OrderBy(m => m.Name))
                {
                    logWriter.WriteLine($"    - {method.ClassName}.{method.Name}");
                }
            }

            // 測試方法名稱分析
            logWriter.WriteLine($"\n測試方法名稱分析:");
            var methodNamePatterns = testMethods.GroupBy(m => GetMethodPattern(m.Name)).OrderByDescending(g => g.Count());
            foreach (var pattern in methodNamePatterns)
            {
                logWriter.WriteLine($"\n模式: {pattern.Key}");
                logWriter.WriteLine($"  數量: {pattern.Count()} 個");
                foreach (var method in pattern.Take(5)) // 只顯示前5個
                {
                    logWriter.WriteLine($"    - {method.Name}");
                }
                if (pattern.Count() > 5)
                {
                    logWriter.WriteLine($"    ... 還有 {pattern.Count() - 5} 個");
                }
            }
        }

        static string GetMethodPattern(string methodName)
        {
            // 分析測試方法命名模式
            if (methodName.Contains("_Should"))
                return "Should_模式";
            else if (methodName.Contains("_When"))
                return "When_模式";
            else if (methodName.Contains("_With"))
                return "With_模式";
            else if (methodName.StartsWith("Test"))
                return "Test_前綴";
            else
                return "其他模式";
        }
    }

    class TestClassInfo
    {
        public string Name { get; set; } = "";
        public string Namespace { get; set; } = "";
        public string FilePath { get; set; } = "";
        public int LineNumber { get; set; }
    }

    class TestMethodInfo
    {
        public string Name { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string Namespace { get; set; } = "";
        public string FilePath { get; set; } = "";
        public int LineNumber { get; set; }
    }
} 