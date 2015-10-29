using System;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using JsonArray = Selenium.List;
using JsonObject = Selenium.Dictionary;
using JsonObjectItem = Selenium.DictionaryItem;

namespace Selenium.Serializer {

    /// <summary>
    /// Json serializer that encodes an object to a Json UTF8 binary string. 
    /// Automatically encode a zip to a base64 string.
    /// Provides an external interface to automatically serialize an object (IJsonObject).
    /// </summary>
    [DebuggerDisplay("{DebugValue,nq}")]
    internal class JsonWriter {

        const int DEPTH_MAX = 8;    //maximum depth for arrays and dictionaries
        const int DEFAULT_BUFFER_SIZE = 1024;

        public static JsonWriter Serialize(object value) {
            JsonWriter writer = new JsonWriter();
            writer.SerializeObject(value, 0);
            return writer;
        }


        private byte[] _buffer;
        private int _capacity;
        private int _length;

        public JsonWriter() {
            _buffer = new byte[DEFAULT_BUFFER_SIZE];
            _capacity = DEFAULT_BUFFER_SIZE - 8;
            _length = 0;
        }

        public void CopyTo(Stream stream) {
            stream.Write(_buffer, 0, _length);
        }

        public byte[] GetBuffer() {
            return _buffer;
        }

        public int Length {
            get {
                return _length;
            }
        }

        public string DebugValue {
            get {
                return System.Text.Encoding.UTF8.GetString(_buffer, 0, _length);
            }
        }

        private void SerializeObject(object obj, int depth) {
            if (obj == null) {
                WriteBytes("null"); return;
            }

            Type t = obj.GetType();
            switch (Type.GetTypeCode(t)) {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    SerializeConvertible((IConvertible)obj); return;
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Single:
                    SerializeFloat((IFormattable)obj); return;
                case TypeCode.String:
                    SerializeString((string)obj); return;
                case TypeCode.Char:
                    SerializeCharacter((char)obj); return;
                case TypeCode.Boolean:
                    SerializeBool((bool)obj); return;
                case TypeCode.DBNull:
                    WriteBytes("null"); return;
                case TypeCode.DateTime:
                    SerializeDateTime((System.DateTime)obj); return;
            }

            if (t.IsArray) {
                Type te = t.GetElementType();
                switch (Type.GetTypeCode(te)) {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        SerializeArray((IConvertible[])obj, SerializeConvertible); return;
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.Single:
                        SerializeArray((IFormattable[])obj, SerializeFloat); return;
                    case TypeCode.String:
                        SerializeArray((string[])obj, SerializeString); return;
                    case TypeCode.Char:
                        SerializeArray((char[])obj, SerializeCharacter); return;
                    case TypeCode.Boolean:
                        SerializeArray((bool[])obj, SerializeBool); return;
                    case TypeCode.DBNull:
                        WriteBytes("null"); return;
                    case TypeCode.DateTime:
                        SerializeArray((DateTime[])obj, SerializeDateTime); return;
                }

                IConvertible[] arrConv = obj as IConvertible[];
                if (arrConv != null) {
                    SerializeArray(arrConv, SerializeConvertible); return;
                }
            }

            JsonObject objDict = obj as JsonObject;
            if (objDict != null) {
                SerializeDictionary(objDict, depth); return;
            }

            JsonArray objList = obj as JsonArray;
            if (objList != null) {
                SerializeArray(objList, depth); return;
            }

            IJsonObject objObj = obj as IJsonObject;
            if (objObj != null) {
                var ocdict = objObj.SerializeJson();
                SerializeDictionary(ocdict, depth); return;
            }

            IEnumerable objEnum = obj as IEnumerable;
            if (objEnum != null && t.IsSerializable) {
                SerializeEnumerable(objEnum, depth); return;
            }

            IJsonBinary objBinary = obj as IJsonBinary;
            if (objBinary != null) {
                SerializeBinary(objBinary); return;
            }

            throw new JsonException(string.Format(
                "Object of type {0} is not serializable\n{1}{0}"
                , t.Name, this.DebugValue));
        }

        private void SerializeBinary(IJsonBinary binary) {
            MemoryStream stream = new MemoryStream();
            binary.Save(stream);

            WriteByte((byte)'"');
            WriteBytesBase64(stream.GetBuffer(), (int)stream.Length);
            WriteByte((byte)'"');
        }

        private void SerializeConvertible(IConvertible value) {
            WriteBytes(value.ToString(CultureInfo.InvariantCulture));
        }

        private void SerializeFloat(IFormattable value) {
            WriteBytes(value.ToString("r", CultureInfo.InvariantCulture));
        }

        private void SerializeBool(bool value) {
            WriteBytes(value ? "true" : "false");
        }

        private void SerializeDateTime(DateTime value) {
            WriteByte((byte)'"');
            WriteBytes(value.ToString("s"));
            WriteByte((byte)'"');
        }

        private void SerializeString(string value) {
            WriteByte((byte)'"');
            if (!string.IsNullOrEmpty(value)) {
                WriteString(value);
            }
            WriteByte((byte)'"');
        }

        private void SerializeCharacter(char c) {
            WriteByte((byte)'"');
            WriteCharacter(c);
            WriteByte((byte)'"');
        }

        private void SerializeArray<T>(T[] values, Action<T> action) {
            WriteByte((byte)'[');
            if (values.Length != 0) {
                action(values[0]);
                for (int i = 1; i < values.Length; i++) {
                    WriteByte((byte)',');
                    action(values[i]);
                }
            }
            WriteByte((byte)']');
        }

        private void SerializeDictionary(JsonObject dict, int depth) {
            if (++depth > DEPTH_MAX)
                throw new JsonException("DepthLimitExceeded");

            WriteByte((byte)'{');

            int i = 0;
            foreach (JsonObjectItem item in dict) {
                if (i++ > 0)
                    WriteByte((byte)',');
                SerializeString(item.key); //item name
                WriteByte((byte)':');
                SerializeObject(item.value, depth); //item value
            }
            WriteByte((byte)'}');
        }

        private void SerializeArray(JsonArray array, int depth) {
            if (++depth > DEPTH_MAX)
                throw new JsonException("DepthLimitExceeded");

            object[] items = array._items;
            int count = array.Count;
            WriteByte((byte)'[');
            if (count != 0) {
                SerializeObject(items[0], depth);
                for (int i = 1; i < count; i++) {
                    WriteByte((byte)',');
                    SerializeObject(items[i], depth);
                }
            }
            WriteByte((byte)']');
        }

        private void SerializeEnumerable(IEnumerable enumerable, int depth) {
            if (++depth > DEPTH_MAX)
                throw new JsonException("DepthLimitExceeded");

            IEnumerator iter = enumerable.GetEnumerator();
            WriteByte((byte)'[');
            if (iter.MoveNext()) {
                SerializeObject(iter.Current, depth);
                while (iter.MoveNext()) {
                    WriteByte((byte)',');
                    SerializeObject(iter.Current, depth);
                }
            }
            WriteByte((byte)']');
        }



        #region Base64 encoding Table

        static readonly byte[] BASE64_TABLE = {
            065,066,067,068,069,070,071,072,073,074,075,076,077,078,079,080,
            081,082,083,084,085,086,087,088,089,090,097,098,099,100,101,102,
            103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,
            119,120,121,122,048,049,050,051,052,053,054,055,056,057,043,047};

        #endregion

        private unsafe void WriteBytesBase64(byte[] source, int length) {
            if (length > source.Length)
                throw new ArgumentOutOfRangeException("length");
            int countBlocks = length / 3; //blocks of 3 bytes
            int countRemain = length % 3; //remaining bytes (0, 1 or 2)
            int destCount = countBlocks * 4 + ((countRemain == 0) ? 0 : 4);
            if (_length + destCount > _capacity)
                IncreaseBufferSize(destCount);
            fixed (byte* pSrc0 = source, pDest0 = _buffer) {
                byte* pSrc = pSrc0;
                byte* pDest = pDest0 + _length;
                for (int i = countBlocks; i-- > 0; pSrc += 3, pDest += 4) {
                    pDest[0] = BASE64_TABLE[(pSrc[0] & 0xfc) >> 2];
                    pDest[1] = BASE64_TABLE[((pSrc[0] & 0x03) << 4) | ((pSrc[1] & 0xf0) >> 4)];
                    pDest[2] = BASE64_TABLE[((pSrc[1] & 0x0f) << 2) | ((pSrc[2] & 0xc0) >> 6)];
                    pDest[3] = BASE64_TABLE[(pSrc[2] & 0x3f)];
                }
                if (countRemain == 1) {
                    pDest[0] = BASE64_TABLE[(pSrc[0] & 0xfc) >> 2];
                    pDest[1] = BASE64_TABLE[(pSrc[1] & 0x03) << 4];
                    pDest[2] = (byte)'=';
                    pDest[3] = (byte)'=';
                } else if (countRemain == 2) {
                    pDest[0] = BASE64_TABLE[(pSrc[0] & 0xfc) >> 2];
                    pDest[1] = BASE64_TABLE[((pSrc[0] & 0x03) << 4) | ((pSrc[1] & 0xf0) >> 4)];
                    pDest[2] = BASE64_TABLE[(pSrc[1] & 0x0f) << 2];
                    pDest[3] = (byte)'=';
                }
            }
            _length += destCount;
        }

        private void WriteCharacter(char c) {
            if (c < 0x80) {
                WriteChar1Byte(c); //1 byte UTF8
            } else if (c <= 0x7FF) {
                WriteChar2Bytes(c); //2 bytes UTF8
            } else {
                WriteChar3Bytes(c); //3 bytes UTF8
            }
        }

        private void WriteString(string value) {
            for (int i = 0; i < value.Length; i++) {
                int c = value[i];
                if (c < 0x80) {
                    WriteChar1Byte(c); //1 byte UTF8
                } else if (c <= 0x7FF) {
                    WriteChar2Bytes(c); //2 bytes UTF8
                } else if (c < 0xE000 && c >= 0xD800 && (i + 1) < value.Length
                    && value[i + 1] >= 0xDC00 && value[i + 1] < 0xE000) {
                    c = (c << 10) + value[++i] - 0x35FDC00;  //Add surrogate char
                    WriteChar4Bytes(c); //4 bytes UTF8
                } else {
                    WriteChar3Bytes(c); //3 bytes UTF8
                }
            }
        }

        private void WriteChar1Byte(int c) {
            if (c < 32 || c == '"' || c == '\\') {
                switch (c) {
                    case 8: c = 'b'; break;
                    case 9: c = 't'; break;
                    case 10: c = 'n'; break;
                    case 12: c = 'f'; break;
                    case 13: c = 'r'; break;
                    case 34: c = '"'; break;
                    case 92: c = '\\'; break;
                    default: WriteCharUnicode(c); return;
                }
                WriteCharEscaped(c); return;
            }
            if (_length > _capacity)
                IncreaseBufferSize(1);
            _buffer[_length++] = (byte)c;
        }

        private void WriteCharEscaped(int c) {
            if (_length > _capacity)
                IncreaseBufferSize(2);
            _buffer[_length++] = (byte)'\\';
            _buffer[_length++] = (byte)c;
        }


        #region Base16 Table

        /// <summary>
        /// Translates a number to a base 16 char code
        /// BASE16_TABLE[0] gives 48 ('0')
        /// BASE16_TABLE[15] gives 70 ('F')
        /// </summary>
        static readonly byte[] BASE16_TABLE = {
            48,49,50,51,52,53,54,55,56,57,65,66,67,68,69,70};

        #endregion

        private void WriteCharUnicode(int c) {
            if (_length > _capacity)
                IncreaseBufferSize(6);
            _buffer[_length++] = (byte)'\\';
            _buffer[_length++] = (byte)'u';
            _buffer[_length++] = BASE16_TABLE[(c >> 12) & 0x0F];
            _buffer[_length++] = BASE16_TABLE[(c >> 8) & 0x0F];
            _buffer[_length++] = BASE16_TABLE[(c >> 4) & 0x0F];
            _buffer[_length++] = BASE16_TABLE[c & 0x0F];
        }

        private void WriteChar2Bytes(int c) {
            if (_length > _capacity)
                IncreaseBufferSize(2);
            _buffer[_length++] = (byte)(0xC0 | (c >> 6));
            _buffer[_length++] = (byte)(0x80 | (c & 0x3F));
        }

        private void WriteChar3Bytes(int c) {
            if (_length > _capacity)
                IncreaseBufferSize(3);
            _buffer[_length++] = (byte)(0xE0 | (c >> 12));
            _buffer[_length++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            _buffer[_length++] = (byte)(0x80 | (c & 0x3F));
        }

        private void WriteChar4Bytes(int c) {
            if (_length > _capacity)
                IncreaseBufferSize(4);
            _buffer[_length++] = (byte)(0xF0 | (c >> 18));
            _buffer[_length++] = (byte)(0x80 | ((c >> 12) & 0x3F));
            _buffer[_length++] = (byte)(0x80 | ((c >> 6) & 0x3F));
            _buffer[_length++] = (byte)(0x80 | (c & 0x3F));
        }

        private void WriteByte(byte b) {
            if (_length > _capacity)
                IncreaseBufferSize(1);
            _buffer[_length++] = b;
        }

        private void WriteBytes(string str) {
            if (_length + str.Length > _capacity)
                IncreaseBufferSize(str.Length);
            for (int i = 0; i < str.Length; i++)
                _buffer[_length + i] = (byte)str[i];
            _length += str.Length;
        }

        private void IncreaseBufferSize(int capacity) {
            _capacity = Math.Max(_buffer.Length * 2, _length + capacity);
            byte[] buffer = new byte[_capacity + 8]; //Adds 8 extra bytes for safety
            Buffer.BlockCopy(_buffer, 0, buffer, 0, _length);
            _buffer = buffer;
        }

    }

}
