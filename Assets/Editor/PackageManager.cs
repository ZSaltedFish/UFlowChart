using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Assets
{
    public class PackageManager : EditorWindow
    {
        #region 编译系
        public string DevenvPath = "";
        public string BatPath;
        public string SlnFilePath;
        #endregion

        #region 拷贝系
        public string ScriptLibFolder;
        public string TargetLibFolider;
        public string PackageJson;
        public string ResourceFolder;
        #endregion

        public void OnGUI()
        {
            //using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            //{
            //    DevenvPath = GetFile("编译器路径", DevenvPath, "exe");
            //    BatPath = GetFile("Bat文件路径", BatPath, "bat");
            //    SlnFilePath = GetFile("sln文件路径", SlnFilePath, "sln");
            //}

            //using (new EditorGUI.DisabledGroupScope(string.IsNullOrEmpty(DevenvPath)))
            //{
            //    if (GUILayout.Button("Go"))
            //    {
            //        _ = Task.Run(RunTask);
            //    }
            //}

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                PackageJson = GetFile("Package", PackageJson, "json");
                ScriptLibFolder = GetPath("DLL文件路径", ScriptLibFolder);
                ResourceFolder = GetPath("资源文件夹", ResourceFolder);
                TargetLibFolider = GetPath("输出文件夹", TargetLibFolider);
            }

            bool disable = string.IsNullOrEmpty(PackageJson) || string.IsNullOrEmpty(TargetLibFolider) || string.IsNullOrEmpty(ScriptLibFolder) || string.IsNullOrEmpty(ResourceFolder);
            using (new EditorGUI.DisabledScope(disable))
            {
                if (GUILayout.Button("Copy"))
                {
                    _ = Task.Run(RunCopy);
                }
            }
        }

        private void RunCopy()
        {
            try
            {
                var targetJsonPath = Path.Combine(TargetLibFolider, Path.GetFileName(PackageJson));
                var editorDLL = "ZKnight.UFlowChart.Editor.dll";
                var runtimeDLL = "ZKnight.UFlowChart.Runtime.dll";

                var sourceEditorDLL = Path.Combine(ScriptLibFolder, editorDLL);
                var sourceRuntimeDLL = Path.Combine(ScriptLibFolder, runtimeDLL);

                var editorPath = Path.Combine(TargetLibFolider, editorDLL);
                var runtimeDPath = Path.Combine(TargetLibFolider, runtimeDLL);
                File.Copy(PackageJson, targetJsonPath, true);
                File.Copy(sourceEditorDLL, editorPath, true);
                File.Copy(sourceRuntimeDLL, runtimeDPath, true);

                var folderName = new DirectoryInfo(ResourceFolder).Name;
                var resourceTargetPath = Path.Combine(TargetLibFolider, folderName);
                CopyDirectory(ResourceFolder, resourceTargetPath);
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError(ex);
            }
        }

        private static void CopyDirectory(string srcPath, string targetPath)
        {
            var dir = new DirectoryInfo(srcPath);
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            foreach (var file in dir.GetFiles())
            {
                var name = file.Name;
                if (file.Extension == "meta")
                {
                    continue;
                }
                var path = Path.Combine(targetPath, name);
                File.Copy(file.FullName, path, true);
            }

            foreach (var directory in dir.GetDirectories())
            {
                var tp = Path.Combine(targetPath, directory.Name);
                CopyDirectory(directory.FullName, tp);
            }
        }

        private void RunTask()
        {
            var devenvPath = DevenvPath.Replace(".exe", ".com").Replace('/', '\\');
            var batPath = BatPath.Replace("/", "\\");
            var slnFilePath = SlnFilePath.Replace("/", "\\");
            var param = $"\"{devenvPath}\" {slnFilePath}";
            using (var process = Process.Start(batPath, param))
            {
                process.WaitForExit();
            }
        }

        public static string GetPath(string title, string path)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                path = EditorGUILayout.TextField(title, path);
                if (GUILayout.Button(">>", GUILayout.Width(30)))
                {
                    var p = EditorUtility.OpenFolderPanel(title, path, "");
                    if (p != null)
                    {
                        path = p;
                    }
                }
                return path;
            }
        }

        public static string GetFile(string title, string path, string extend)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                path = EditorGUILayout.TextField(title, path);
                if (GUILayout.Button(">>", GUILayout.Width(30)))
                {
                    var p = EditorUtility.OpenFilePanel(title, path, extend);
                    if (p != null)
                    {
                        path = p;
                    }
                }
                return path;
            }
        }

        private void InitPath()
        {
            var rootPath = Directory.GetParent(Application.dataPath).FullName;
            BatPath = Path.Combine(Application.dataPath, @"Resources\complie.bat");
            SlnFilePath = Path.Combine(rootPath, "UFlowChart.sln");
            ScriptLibFolder = Path.Combine(rootPath, @"Library\ScriptAssemblies\");
            ResourceFolder = Path.Combine(rootPath, @"Packages\FlowChart\Resources");
            PackageJson = Path.Combine(rootPath, @"Packages\FlowChart\package.json");
        }

        [MenuItem("Tools/Package Asset")]
        public static void Init()
        {
            var window = GetWindow<PackageManager>();
            window.InitPath();
            window.Show();
        }
    }
}