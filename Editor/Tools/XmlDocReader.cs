using System.Xml.Linq;
using System.Collections.Generic;

namespace UnityMdDocGenerator {
	public class XmlDocReader {
		string[] _pathes = null;
		LoggerBase _logger = null;

		public XmlDocReader(string[] pathes, LoggerBase logger) {
			_pathes = pathes;
			_logger = logger;
		}

		public List<DocNode> Read() {
			var docs = ReadDocs();
			var nodes = CreateNodes(docs);
			return nodes;
		}

		List<XDocument> ReadDocs() {
			var docs = new List<XDocument>();
			foreach ( var path in _pathes ) {
				var newDoc = XDocument.Load(path);
				if( newDoc != null ) {
					docs.Add(newDoc);
				} else {
					_logger.LogFormat(LogLevel.Error, "Can't load XML from {0}", path);
				}
			}
			return docs;
		}

		List<DocNode> CreateNodes(List<XDocument> docs) {
			var allNodes = new List<DocNode>();
			foreach( var doc in docs ) {
				var nodes = doc.Root.Nodes();
				foreach( var node in nodes) {
					var elem = (XElement)node;
					if( elem.Name == "members") {
						var members = new List<XElement>(elem.Elements());
						members.Sort((a, b) => 
							a.Attribute(XName.Get("name")).Value.Substring(2).CompareTo(
                    		b.Attribute(XName.Get("name")).Value.Substring(2)));
						foreach( var item in members ) {
							allNodes.Add(CreateNode(item));
						}
					}
				}
			}
			return allNodes;
		}

		DocNode CreateNode(XElement element) {
			var node = new DocNode(element);
			node.FindInfo();
			return node;
		}
	}
}
