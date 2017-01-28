namespace UnityMdDocGenerator {
	public class MdGenerator {

		XmlDocReader _reader = null;
		MdWriter     _writer = null;

		public MdGenerator(XmlDocReader reader, MdWriter writer) {
			_reader = reader;
			_writer = writer;
		}

		public void Create() {
			var nodes = _reader.Read();
			_writer.Write(nodes);
		}
	}
}
