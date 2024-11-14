using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class IosBuildPostprocessor
{

    [PostProcessBuild(1)]
    public static void EditPlist(BuildTarget target, string path)
    {
        if (target != BuildTarget.iOS)
            return;

        string pbxProjectPath = PBXProject.GetPBXProjectPath(path);
        string plistPath = path + "/Info.plist";

        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        PBXProject pbxProject = new PBXProject();
        pbxProject.ReadFromFile(pbxProjectPath);

#if UNITY_2019_3_OR_NEWER
        string targetGUID = pbxProject.GetUnityFrameworkTargetGuid();
#endif
        pbxProject.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-weak_framework PhotosUI");
        pbxProject.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-framework Photos");
        pbxProject.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-framework MobileCoreServices");
        pbxProject.AddBuildProperty(targetGUID, "OTHER_LDFLAGS", "-framework ImageIO");

        pbxProject.RemoveFrameworkFromProject(targetGUID, "Photos.framework");
        pbxProject.RemoveFrameworkFromProject(targetGUID, "PhotosUI.framework");

        File.WriteAllText(pbxProjectPath, pbxProject.WriteToString());

        PlistElementDict rootDict = plist.root;

        // Add ITSAppUsesNonExemptEncryption to Info.plist
        rootDict.SetString("ITSAppUsesNonExemptEncryption", "false");
        rootDict.SetString("NSPhotoLibraryUsageDescription", "The app requires access to Photos to interact with it.");
        rootDict.SetString("NSPhotoLibraryAddUsageDescription", "The app requires access to Photos to save media to it.");
        rootDict.SetBoolean("PHPhotoLibraryPreventAutomaticLimitedAccessAlert", true);


        File.WriteAllText(plistPath, plist.WriteToString());
    }
}