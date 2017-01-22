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
			var outputLines = new List<string>();
			foreach( var node in nodes ) {
				var output = node.Name;
				switch( node.Type ) {
					case DocNodeType.Type:
						output = string.Format("- **{0}**", output);
						break;
					default:
						output = string.Format("\t- {0}", output);
						break;
				}
				_logger.Log(output);
				outputLines.Add(output);
			}
			File.WriteAllLines(_outputRoot, outputLines.ToArray());
			_logger.LogFormat("Saved to {0}", _outputRoot);
		}
	}
}
