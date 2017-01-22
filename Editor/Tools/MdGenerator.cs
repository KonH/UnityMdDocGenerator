namespace UnityMdDocGenerator {
	public class MdGenerator {

		XmlDocReader _reader = null;
		MdWriter  _writer = null;
		LoggerBase _logger = null;

		public MdGenerator(XmlDocReader reader, MdWriter writer, LoggerBase logger) {
			_reader = reader;
			_writer = writer;
			_logger = logger;
		}

		public void Create() {
			var nodes = _reader.Read();
			_writer.Write(nodes);
		}
	}
}
