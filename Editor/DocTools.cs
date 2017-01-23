using UnityEditor;

namespace UnityMdDocGenerator {
	public static class DocTools {

		static string[] Input = new string[]{
			"RootFilesCopy/Prepared/Temp/UnityVS_bin/Debug/Assembly-CSharp.XML",
			"RootFilesCopy/Prepared/Temp/UnityVS_bin/Debug/Assembly-CSharp-Editor.XML"};
		static string Output = "Assets/CurrentResult.md";

		public static void Generate() {
			// Prototype
			var logger = new UnityLogger();
			var reader = new XmlDocReader(Input, logger);
			var writer = new MdWriter(Output, logger);
			var generator = new MdGenerator(reader, writer, logger);
			generator.Create();
			AssetDatabase.Refresh();
		}
	}
}
