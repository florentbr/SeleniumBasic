
namespace vbsc {

    class ConsoleArgument {

        public string Id;
        public string Pattern;
        public object Value;
        public string Help;
        public string Info = null;

        public override string ToString() {
            if (Info != null) {
                return ' ' + Help + "\r\n   " + Info;
            } else {
                return ' ' + Help;
            }
        }
    }

}
