using System.Xml.Linq;
using System.Collections.Generic;

namespace UnityMdDocGenerator {
	public class DocNode {
		public XElement      Element    { get; private set; }
		public string        FullName   { get; private set; }
		public string        Name       { get; private set; }
		public string        ParentName { get; private set; }
		public DocNodeType   Type       { get; private set; }
		public List<DocNode> Childs     { get; private set; }

		public DocNode(XElement element) {
			Childs = new List<DocNode>();
			Element = element;
			FullName = element.Attribute(XName.Get("name")).Value;
		}

		public void FindInfo() {
			Type = FindType();
			SetupNames();
		}

		DocNodeType FindType() {
			var typeStr = FullName.Substring(0, 1);
			switch( typeStr ) {
				case "T": return DocNodeType.Type;
				default: return DocNodeType.Unknown;
			}
		}

		void SetupNames() {
			var nameParts = SafeSplitName(FullName);
			Name = nameParts[nameParts.Count - 1];
			if( nameParts.Count > 1 ) {
				ParentName = "";
				for( int i = 0; i <= nameParts.Count - 2; i++ ) {
					ParentName += nameParts[i];
					if( i < nameParts.Count - 2 ) {
						ParentName += ".";
					}
				}
			}
		}

		List<string> SafeSplitName(string name) {
			var parts = new List<string>();
			var curPart = "";
			for( int i = 2; i < name.Length; i++ ) {
				var c = name[i];
				if( c == '.' ) {
					parts.Add(curPart);
					curPart = "";
				} else if( c != '(' ) {
					curPart += c;
				} else {
					curPart += name.Substring(i);
					break;
				}
			}
			if( !string.IsNullOrEmpty(curPart) ) {
				parts.Add(curPart);
			}
			return parts;
		}

		public override string ToString() {
			return string.Format("'{0}' = ('{1}', '{2}', {3})", FullName, ParentName, Name, Type);
		}
	}
}
