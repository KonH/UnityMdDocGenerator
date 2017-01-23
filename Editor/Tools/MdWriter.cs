using System.IO;
using System.Collections.Generic;

namespace UnityMdDocGenerator {
	public class MdWriter {
		string     _outputRoot = null;
		LoggerBase _logger     = null;

		public MdWriter(string outputRoot, LoggerBase logger) {
			_outputRoot = outputRoot;
			_logger = logger;
		}

		public void Write(List<DocNode> nodes) {
			var tree = CreateTree(nodes);
			var outputLines = new List<string>();
			SetupTreeLines(tree, outputLines, 0);
			File.WriteAllLines(_outputRoot, outputLines.ToArray());
			_logger.LogFormat("Saved to {0}", _outputRoot);
		}

		List<MdNode> CreateTree(List<DocNode> nodes) {
			var tree = new List<MdNode>();
			foreach( var node in nodes ) {
				AddNode(tree, node);
			}
			return tree;
		}

		void AddNode(List<MdNode> tree, DocNode node) {
			var parentName = node.ParentName;
			var newNode = new MdNode(node.Name, node);
			var path = new List<string>();
			if( string.IsNullOrEmpty(parentName) ) {
				// Empty path
			} else if( !parentName.Contains(".") ) {
				path.Add(parentName);
			} else {
				path.AddRange(parentName.Split('.'));
			}
			var typeNode = GetTypeNode(node);
			if( typeNode != null ) {
				path.Add(typeNode);
			}
			AddNode(tree, newNode, path);
		}

		string GetTypeNode(DocNode node) {
			switch( node.Type ) {
				case DocNodeType.Method: return "Methods:";
				case DocNodeType.Property: return "Properties:";
				case DocNodeType.Field: return "Fields:";
				default: return null;
			}
		}

		void AddNode(List<MdNode> tree, MdNode node, List<string> path) {
			if ( path.Count == 0 ) {
				tree.Add(node);
			} else {
				var parentName = path[0];
				MdNode childNode = null;
				foreach( var child in tree ) {
					if( child.Name == parentName ) {
						childNode = child;
						break;
					}
				}
				if( childNode == null ) {
					childNode = new MdNode(parentName, null);
					tree.Add(childNode);
				}
				path.RemoveAt(0);
				AddNode(childNode.Childs, node, path);
			}
		}

		void SetupTreeLines(List<MdNode> tree, List<string> output, int intend = 0) {
			foreach( var item in tree ) {
				SetupOutput(item, output, intend);
				SetupTreeLines(item.Childs, output, intend + 1);
			}
		}

		void SetupOutput(MdNode node, List<string> output, int intend = 0) {
			var intendLine = "";
			for( int i = 0; i <= intend; i++ ) {
				intendLine += "#";
			}
			var outputHeader = string.Format("{0} {1}", intendLine, node.Name);
			output.Add(outputHeader);
			if( node.Content != null ) {
				output.Add(node.Content.Element.ToString());
			}
		}
	}
}
