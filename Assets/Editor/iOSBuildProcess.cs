using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

// 参考
// https://mushikago.com/i/?p=7935

public class iOSBuildProcess : MonoBehaviour 
{
#if UNITY_IOS
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{

        if (buildTarget == BuildTarget.iOS) 
		{
			string projPath = PBXProject.GetPBXProjectPath (path);
			PBXProject proj = new PBXProject ();
			proj.ReadFromString (File.ReadAllText (projPath));
			string target = proj.TargetGuidByName ("Unity-iPhone");

			// ここにビルド後の処理を記述。
			proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

			File.WriteAllText(projPath, proj.WriteToString());
		}
	}
#endif
}
