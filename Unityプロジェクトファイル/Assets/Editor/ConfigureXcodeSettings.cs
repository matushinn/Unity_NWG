#if UNITY_IOS

using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class ConfigureXcodeSettings
{
    [PostProcessBuild]
    public static void SetPlist(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            XmlDocument document = new XmlDocument();
            XmlDocumentType doctype = document.CreateDocumentType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
            document.AppendChild(doctype);

            XmlElement plist = document.CreateElement("plist");
            plist.SetAttribute("version", "1.0");
            XmlElement dict = document.CreateElement("dict");
            plist.AppendChild(dict);
            document.AppendChild(plist);

            XmlElement e = (XmlElement)document.SelectSingleNode("/plist/dict");

            XmlElement key = document.CreateElement("key");
            key.InnerText = "aps-environment";
            e.AppendChild(key);

            XmlElement value = document.CreateElement("string");
            value.InnerText = "development";
            e.AppendChild(value);

            string entitlementsPath = path + "/Unity-iPhone/{" + PlayerSettings.productName + "}.entitlements";
            document.Save(entitlementsPath);

            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject proj = new PBXProject();
            proj.ReadFromString(File.ReadAllText(projPath));
            string target = proj.TargetGuidByName("Unity-iPhone");
            string guid = proj.AddFile(entitlementsPath, entitlementsPath);
            proj.SetBuildProperty(target, "CODE_SIGN_ENTITLEMENTS", "Unity-iPhone/{" + PlayerSettings.productName + "}.entitlements");
            proj.AddFileToBuild(target, guid);
            proj.WriteToFile(projPath);
        }
    }

    [PostProcessBuild]
    private static void SetCapabilities(BuildTarget buildTarget, string path)
    {
        if (buildTarget == BuildTarget.iOS)
        {
            string projPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
            PBXProject proj = new PBXProject();

            proj.ReadFromString(File.ReadAllText(projPath));

            string targetGuid = proj.TargetGuidByName("Unity-iPhone");
            proj.AddFrameworkToProject(targetGuid, "UserNotifications.framework", true);

            string[] lines = proj.WriteToString().Split('\n');
            List<string> newLines = new List<string>();
            bool editFinish = false;

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (editFinish)
                {
                    newLines.Add(line);
                }
                else if (line.IndexOf("isa = PBXProject;") > -1)
                {
                    do
                    {
                        newLines.Add(line);
                        line = lines[++i];
                    } while (line.IndexOf("TargetAttributes = {") == -1);

                    newLines.Add("TargetAttributes = {");
                    newLines.Add("XXXXXXXXXXXXXXXX = {");
                    newLines.Add("DevelopmentTeam = " + PlayerSettings.iOS.appleDeveloperTeamID + ";");
                    newLines.Add("SystemCapabilities = {");
                    newLines.Add("com.apple.Push = {");
                    newLines.Add("enabled = 1;");
                    newLines.Add("};");
                    newLines.Add("};");
                    newLines.Add("};");
                    editFinish = true;
                }
                else
                {
                    newLines.Add(line);
                }
            }

            File.WriteAllText(projPath, string.Join("\n", newLines.ToArray()));
        }
    }
}
#endif