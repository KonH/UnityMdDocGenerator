using System.IO;
using System.Collections.Generic;

namespace UnityMdDocGenerator {
	public class MdWriter {

		const string FieldsNodeName        = "Fields\nField | About\n--- | ---";
		const string PropertiesNodeName    = "Properties\nProperty | About\n--- | ---";
		const string MethodsNodeName       = "Methods";
		const string NamespaceHeaderFormat = "# {0}";
		const string ItemHeaderFormat      = "## {0}";
		const string GroupHeaderFormat     = "### {0}";
		const string MemberFormat          = "**{0}** | {1}";
		const string MethodFormat          = "**{0}** - {1}";
		string     _outputRoot = null;
		LoggerBase _logger     = null;

		public MdWriter(string outputRoot, LoggerBase logger) {
			_outputRoot = outputRoot;
			_logger = logger;
		}

		public void Write(List<DocNode> nodes) {
			var tree = CreateTree(nodes);
			FindNodesTypes(tree);
			ArrangeNodes(tree);
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
				var nameParts = parentName.Split('.');
				path.AddRange(nameParts);
			}
			var typeNode = GetTypeNode(node);
			if( typeNode != null ) {
				path.Add(typeNode);
			}
			AddNode(tree, newNode, path);
		}

		string GetTypeNode(DocNode node) {
			switch( node.Type ) {
				case DocNodeType.Method  : return MethodsNodeName;
				case DocNodeType.Property: return PropertiesNodeName;
				case DocNodeType.Field   : return FieldsNodeName;
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

		void FindNodesTypes(List<MdNode> nodes) {
			foreach( var item in nodes ) {
				item.Type = FindNodeType(item);
				FindNodesTypes(item.Childs);
			}
		}

		void ArrangeNodes(List<MdNode> nodes) {
			// Fields, Properties, Methods
			var methodsNode    = FindNode(nodes, MethodsNodeName);
			var propertiesNode = FindNode(nodes, PropertiesNodeName);
			var fieldsNode     = FindNode(nodes, FieldsNodeName);
			TrySetFirstNode(nodes, methodsNode);
			TrySetFirstNode(nodes, propertiesNode);
			TrySetFirstNode(nodes, fieldsNode);
			foreach( var node in nodes ) {
				ArrangeNodes(node.Childs);
			}
		}

		MdNode FindNode(List<MdNode> nodes, string name) {
			foreach( var node in nodes ) {
				if( node.Name == name ) {
					return node;
				}
			}
			return null;
		}

		void TrySetFirstNode(List<MdNode> nodes, MdNode node) {
			if( node != null ) {
				nodes.Remove(node);
				nodes.Insert(0, node);
			}
		}

		void SetupTreeLines(List<MdNode> tree, List<string> output, int intend = 0) {
			foreach( var item in tree ) {
				SetupOutput(item, output, intend);
				SetupTreeLines(item.Childs, output, intend + 1);
			}
		}

		void SetupOutput(MdNode node, List<string> output, int intend = 0) {
			var outputHeader = OutputHeader(node);
			var content      = ExtractContent(node.Content);
			if( (node.Type == MdNodeType.Member || node.Type == MdNodeType.Method) && (content != null) ) {
				if(node.Type == MdNodeType.Method) {
					output.Add(string.Format(MethodFormat, outputHeader, content));
					output.Add("");
				} else {
					output.Add(string.Format(MemberFormat, outputHeader, content));
				}
			} else {
				output.Add(outputHeader);
				if( content != null ) {
					output.Add("");
					output.Add(content);
				}
			}
		}

		string ExtractContent(DocNode node) {
			if( node != null ) {
				// Temp
				var rawText = node.Element.ToString();
				var outputText = "";
				bool skip = false;
				bool onLine = false;
				for( int i = 0; i < rawText.Length; i++ ) {
					var c = rawText[i];
					if( c == '\n' ) {
						onLine = false;
						continue;
					} else if( char.IsWhiteSpace(c) && !onLine ) {
						continue;
					} else {
						onLine = true;
					}
					if( c == '<' ) {
						skip = true;
					}
					if( c == '>' && skip ) {
						skip = false;
						continue;
					}
					if( !skip ) {
						outputText += c;
					}
				}
				return outputText;
			}
			return null;
		}

		string FindFormat(MdNodeType type) {
			switch( type) {
				case MdNodeType.MemberGroup  : return GroupHeaderFormat;
				case MdNodeType.Namespace    : return NamespaceHeaderFormat;
				case MdNodeType.NamespaceItem: return ItemHeaderFormat;
				default: return null;
			}
		}

		string OutputHeader(MdNode node) {
			string format = FindFormat(node.Type);
			if( !string.IsNullOrEmpty(format) ) {
				return string.Format(format, node.Name);
			} else {
				return node.Name;
			} 
		}

		MdNodeType FindNodeType(MdNode node) {
			if( IsMemberGroup(node) ) {
				return MdNodeType.MemberGroup;
			}
			if( IsMethod(node) ) {
				return MdNodeType.Method;
			}
			if( IsMember(node) ) {
				return MdNodeType.Member;
			}
			if( (node.Content != null) || HasAnyMemberGroups(node) ) {
				return MdNodeType.NamespaceItem;
			}
			return MdNodeType.Namespace;
		}

		bool IsMemberGroup(MdNode node) {
			return 
				(node.Name == MethodsNodeName) || 
				(node.Name == PropertiesNodeName) || 
				(node.Name == FieldsNodeName);
		}

		bool HasAnyMemberGroups(MdNode node) {
			foreach( var child in node.Childs ) {
				if( IsMemberGroup(child) ) {
					return true;
				}
			}
			return false;
		}

		bool IsMember(MdNode node) {
			return (node.Content != null) && (node.Content.Type != DocNodeType.Type);
		}

		bool IsMethod(MdNode node) {
			return (node.Content != null) && (node.Content.Type == DocNodeType.Method);
		}
	}
}
