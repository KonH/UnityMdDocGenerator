using System.Collections.Generic;

namespace UnityMdDocGenerator {
	public class MdNode {

		public string       Name    { get; private set; }
		public DocNode      Content { get; private set; }
		public List<MdNode> Childs  { get; private set; }

		public MdNode(string name, DocNode content) {
			Name    = name;
			Content = content;
			Childs  = new List<MdNode>();
		}
	}
}
