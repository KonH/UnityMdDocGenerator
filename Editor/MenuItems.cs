using UnityEditor;

namespace UnityMdDocGenerator {
	public static class MenuItems {

		[MenuItem("UnityMdDocGenerator/Generate")]
		public static void Generate() {
			DocTools.Generate();
		}
	}
}
