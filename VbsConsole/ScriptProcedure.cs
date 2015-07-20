
namespace vbsc {

    class ScriptProcedure {

        private readonly ScriptModule _module;
        private readonly string _name;
        private object[] _arguments;

        public ScriptProcedure(ScriptModule module, string name) {
            _module = module;
            _name = name;
            _arguments = new object[0];
        }

        public ScriptProcedure(ScriptModule module, string name, object[] args) {
            _module = module;
            _name = name;
            _arguments = args;
        }

        public string Name {
            get {
                return _name;
            }
        }

        public object[] Arguments {
            get {
                return _arguments;
            }
            set {
                _arguments = value;
            }
        }

        public object Run() {
            return _module.Run(_name, this.Arguments);
        }

        public bool HasArguments {
            get {
                return _arguments != null && _arguments.Length != 0;
            }
        }

        public override string ToString() {
            if (!this.HasArguments)
                return _name;
            if (_name.ToLower() == "onerror")
                return string.Concat(_name, "(\"", _arguments[0], "\", )");
            return string.Concat(_name, '(', _arguments.Join(',', 20), ')');
        }

    }

}
