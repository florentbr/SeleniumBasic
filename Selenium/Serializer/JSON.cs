using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;

using JsonArray = Selenium.List;
using JsonImage = Selenium.Image;
using JsonObject = Selenium.Dictionary;
using JsonObjectItem = Selenium.DictionaryItem;

//using JsonObject = System.Collections.Hashtable;
//using JsonObjectItem = System.Collections.DictionaryEntry;
//using JsonArray = System.Collections.ArrayList;
//using JsonImage = System.Drawing.Bitmap;

namespace Selenium.Serializer {

    [DebuggerDisplay("{ToString(),nq}")]
    public class JSON {

        public static int DEPTH_MAX = 8;        // maximum depth for arrays and dictionaries
        public static int BUFFER_SIZE = 1024;

        #region Error messages

        private const string E_DepthLimitExceeded = "DepthLimitExceeded";
        private const string E_UnexpectedTermination = "UnexpectedTermination";
        private const string E_UnexpectedChar = "UnexpectedCharacter";
        private const string E_InvalidLiteral = "InvalidLiteral";
        private const string E_InvalidNumber = "InvalidNumber";
        private const string E_InvalidArray = "InvalidArray";
        private const string E_InvalidObject = "InvalidObject";
        private const string E_InvalidEscape = "InvalidEscape";
        private const string E_InvalidUnicode = "InvalidUnicode";

        #endregion


        #region Serializer

        public static JSON Serialize(object value) {
            var json = new JSON();
            json.Write(DEPTH_MAX, value);
            return json;
        }

        public static void Serialize(object value, Stream target) {
            var json = new JSON();
            json.Write(DEPTH_MAX, value);
            target.Write(json._buffer, 0, json._length);
        }

        byte[] _buffer = new byte[BUFFER_SIZE];
        int _length = 0;

        public byte[] GetBuffer() {
            return _buffer;
        }

        public int Length {
            get {
                return _length;
            }
        }

        public override string ToString() {
            return System.Text.Encoding.UTF8.GetString(_buffer, 0, _length);
        }

        private void Write(int depth, object obj) {
            if (obj == null) {
                WriteLiteral("null");
                return;
            }

            Type objType = obj.GetType();
            switch (Type.GetTypeCode(objType)) {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    WriteNumber((IConvertible)obj);
                    return;
                case TypeCode.Double:
                case TypeCode.Decimal:
                case TypeCode.Single:
                    WriteNumber((IFormattable)obj);
                    return;
                case TypeCode.String:
                    WriteQuotedString((string)obj);
                    return;
                case TypeCode.Char:
                    WriteQuotedString((char)obj);
                    return;
                case TypeCode.Boolean:
                    WriteBool((bool)obj);
                    return;
                case TypeCode.DBNull:
                    WriteLiteral("null");
                    return;
                case TypeCode.DateTime:
                    WriteDateTime((System.DateTime)obj);
                    return;
            }

            if (objType.IsArray) {
                Type eleType = objType.GetElementType();
                switch (Type.GetTypeCode(eleType)) {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.UInt16:
                    case TypeCode.UInt32:
                    case TypeCode.UInt64:
                        WriteArray(depth, (IConvertible[])obj, WriteNumber);
                        return;
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                    case TypeCode.Single:
                        WriteArray(depth, (IFormattable[])obj, WriteNumber);
                        return;
                    case TypeCode.String:
                        WriteArray(depth, (string[])obj, WriteQuotedString);
                        return;
                    case TypeCode.Char:
                        WriteArray(depth, (char[])obj, WriteQuotedString);
                        return;
                    case TypeCode.Boolean:
                        WriteArray(depth, (bool[])obj, WriteBool);
                        return;
                    case TypeCode.DBNull:
                        WriteLiteral("null");
                        return;
                    case TypeCode.DateTime:
                        WriteArray(depth, (DateTime[])obj, WriteDateTime);
                        return;
                }

                IConvertible[] arrConv = obj as IConvertible[];
                if (arrConv != null) {
                    WriteArray(depth, arrConv, WriteNumber);
                    return;
                }
            }

            JsonObject objDict = obj as JsonObject;
            if (objDict != null) {
                WriteDictionary(depth, objDict);
                return;
            }

            JsonArray objList = obj as JsonArray;
            if (objList != null) {
                WriteArray(depth, objList);
                return;
            }

            IJsonObject objObj = obj as IJsonObject;
            if (objObj != null) {
                var ocdict = objObj.SerializeJson();
                WriteDictionary(depth, ocdict);
                return;
            }

            IEnumerable objEnum = obj as IEnumerable;
            if (objEnum != null && objType.IsSerializable) {
                WriteEnumerable(depth, objEnum);
                return;
            }

            IJsonBinary objBinary = obj as IJsonBinary;
            if (objBinary != null) {
                WriteBinary(objBinary);
                return;
            }

            Bitmap objBitmap = obj as Bitmap;
            if (objBitmap != null) {
                WriteImage(objBitmap);
                return;
            }

            throw new JsonException(string.Format("Object of type {0} is not serializable.", objType.Name));
        }

        private void WriteBinary(IJsonBinary binary) {
            MemoryStream stream = new MemoryStream();
            binary.Save(stream);

            WriteByte((byte)'"');
            WriteDataBase64(stream.GetBuffer(), (int)stream.Length);
            WriteByte((byte)'"');
        }

        private void WriteImage(Bitmap image) {
            MemoryStream stream = new MemoryStream();
            image.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            WriteByte((byte)'"');
            WriteDataBase64(stream.GetBuffer(), (int)stream.Length);
            WriteByte((byte)'"');
        }

        private void WriteNumber(IConvertible value) {
            WriteLiteral(value.ToString(CultureInfo.InvariantCulture));
        }

        private void WriteNumber(IFormattable value) {
            WriteLiteral(value.ToString("r", CultureInfo.InvariantCulture));
        }

        private void WriteBool(bool value) {
            WriteLiteral(value ? "true" : "false");
        }

        private void WriteDateTime(DateTime value) {
            WriteByte((byte)'"');
            WriteLiteral(value.ToString("s"));
            WriteByte((byte)'"');
        }


        private void WriteQuotedString(string value) {
            WriteByte((byte)'"');
            if (!string.IsNullOrEmpty(value)) {
                WriteWideString(value);
            }
            WriteByte((byte)'"');
        }

        private void WriteQuotedString(char c) {
            WriteByte((byte)'"');
            WriteWideString(c);
            WriteByte((byte)'"');
        }

        private void WriteArray<T>(int depth, T[] values, Action<T> write) {
            if (depth-- < 0)
                throw new JsonException(E_DepthLimitExceeded, _buffer, _length);

            WriteByte((byte)'[');
            if (values.Length > 0) {
                write(values[0]);
                for (int i = 1; i < values.Length; i++) {
                    WriteByte((byte)',');
                    write(values[i]);
                }
            }
            WriteByte((byte)']');
        }

        private void Write(int depth, string key, object obj) {
            WriteQuotedString(key);
            WriteByte((byte)':');
            Write(depth, obj);
        }

        private void WriteDictionary(int depth, JsonObject dict) {
            if (depth-- < 0)
                throw new JsonException(E_DepthLimitExceeded, _buffer, _length);

            WriteByte((byte)'{');
            int len_start = _length;
            foreach (JsonObjectItem item in dict) {
                if (_length > len_start)
                    WriteByte((byte)',');
                WriteQuotedString(item.Key.ToString()); //item Key
                WriteByte((byte)':');
                Write(depth, item.Value);   //item Value
            }
            WriteByte((byte)'}');
        }


        private void WriteArray(int depth, JsonArray array) {
            if (depth-- < 0)
                throw new JsonException(E_DepthLimitExceeded, _buffer, _length);

            object[] items = array.ToArray();
            WriteByte((byte)'[');
            if (items.Length > 0) {
                Write(depth, items[0]);
                for (int i = 1; i < items.Length; i++) {
                    WriteByte((byte)',');
                    Write(depth, items[i]);
                }
            }
            WriteByte((byte)']');
        }


        private void WriteEnumerable(int depth, IEnumerable enumerable) {
            if (depth-- < 0)
                throw new JsonException(E_DepthLimitExceeded, _buffer, _length);

            IEnumerator iter = enumerable.GetEnumerator();
            WriteByte((byte)'[');
            if (iter.MoveNext()) {
                Write(depth, iter.Current);
                while (iter.MoveNext()) {
                    WriteByte((byte)',');
                    Write(depth, iter.Current);
                }
            }
            WriteByte((byte)']');
        }


        #region Encoding table

        /// <summary>
        /// Encode a base 64 number to the corresponding charater.
        /// 0-25  => 65-90  [A-F]
        /// 26-51 => 97-122 [a-f]
        /// 52-61 => 49-57  [0-9]
        /// 62    => 43     [+]
        /// 63    => 47     [/]
        /// </summary>
        private static readonly byte[] ENC_BASE64 = {
            065,066,067,068,069,070,071,072,073,074,075,076,077,078,079,080,
            081,082,083,084,085,086,087,088,089,090,097,098,099,100,101,102,
            103,104,105,106,107,108,109,110,111,112,113,114,115,116,117,118,
            119,120,121,122,048,049,050,051,052,053,054,055,056,057,043,047};

        #endregion

        private unsafe void WriteDataBase64(byte[] data, int dataLen) {
            if (dataLen > data.Length)
                throw new ArgumentOutOfRangeException("length");

            int countBlocks = dataLen / 3; //blocks of 3 bytes
            int countRemain = dataLen % 3; //remaining bytes (0, 1 or 2)
            int destCount = countBlocks * 4 + ((countRemain == 0) ? 0 : 4);

            _length += destCount;
            if (_length > _buffer.Length)
                IncreaseBuffer();

            fixed (byte* src0 = data, dest0 = &_buffer[_length - destCount]) {
                byte* src = src0;
                byte* dest = dest0;
                for (int i = 0; i < countBlocks; i++) {
                    dest[0] = ENC_BASE64[(src[0] & 0xfc) >> 2];
                    dest[1] = ENC_BASE64[((src[0] & 0x03) << 4) | ((src[1] & 0xf0) >> 4)];
                    dest[2] = ENC_BASE64[((src[1] & 0x0f) << 2) | ((src[2] & 0xc0) >> 6)];
                    dest[3] = ENC_BASE64[(src[2] & 0x3f)];
                    src += 3;
                    dest += 4;
                }
                if (countRemain == 1) {
                    dest[0] = ENC_BASE64[(src[0] & 0xfc) >> 2];
                    dest[1] = ENC_BASE64[(src[1] & 0x03) << 4];
                    dest[2] = (byte)'=';
                    dest[3] = (byte)'=';
                } else if (countRemain == 2) {
                    dest[0] = ENC_BASE64[(src[0] & 0xfc) >> 2];
                    dest[1] = ENC_BASE64[((src[0] & 0x03) << 4) | ((src[1] & 0xf0) >> 4)];
                    dest[2] = ENC_BASE64[(src[1] & 0x0f) << 2];
                    dest[3] = (byte)'=';
                }
            }
        }


        private void WriteLiteral(string str) {
            int i = _length;
            _length += str.Length;
            if (_length > _buffer.Length)
                IncreaseBuffer();
            for (int ii = 0; ii < str.Length; ii++)
                _buffer[i + ii] = (byte)str[ii];
        }


        private void WriteWideString(char c) {
            if (c < 0x80) {
                WriteUnicode1Byte(c);  // 1 byte UTF8
            } else if (c <= 0x7FF) {
                WriteUnicode2Bytes(c); // 2 bytes UTF8
            } else {
                WriteUnicode3Bytes(c); // 3 bytes UTF8
            }
        }


        private void WriteWideString(string str) {
            for (int i = 0; i < str.Length; i++) {
                int c = str[i];
                if (c < 0x80) {
                    WriteUnicode1Byte(c);  //1 byte UTF8
                } else if (c <= 0x7FF) {
                    WriteUnicode2Bytes(c); //2 bytes UTF8
                } else if (c < 0xE000 && c >= 0xD800 && (i + 1) < str.Length
                    && str[i + 1] >= 0xDC00 && str[i + 1] < 0xE000) {
                    c = (c << 10) + str[++i] - 0x35FDC00;     //Add surrogate char
                    WriteUnicode4Bytes(c); //4 bytes UTF8
                } else {
                    WriteUnicode3Bytes(c); //3 bytes UTF8
                }
            }
        }


        #region Encoding table

        /// <summary>
        /// Encode a base 16 number to the corresponding character.
        /// 0-9   => 48-57 [0-9]
        /// 10-15 => 65-70 [A-F]
        /// </summary>
        private static readonly byte[] ENC_BASE16 = {
            48,49,50,51,52,53,54,55,56,57,65,66,67,68,69,70};

        #endregion

        private void WriteEscapedUnicode(int c) {
            _length += 6;
            if (_length > _buffer.Length)
                IncreaseBuffer();
            _buffer[_length - 6] = (byte)'\\';
            _buffer[_length - 5] = (byte)'u';
            _buffer[_length - 4] = ENC_BASE16[(c >> 12) & 0x0F];
            _buffer[_length - 3] = ENC_BASE16[(c >> 8) & 0x0F];
            _buffer[_length - 2] = ENC_BASE16[(c >> 4) & 0x0F];
            _buffer[_length - 1] = ENC_BASE16[c & 0x0F];
        }


        private void WriteUnicode1Byte(int c) {
            if (c < 32 || c == '"' || c == '\\') {
                switch (c) {
                    case 8: c = 'b'; break;
                    case 9: c = 't'; break;
                    case 10: c = 'n'; break;
                    case 12: c = 'f'; break;
                    case 13: c = 'r'; break;
                    case 34: c = '"'; break;
                    case 92: c = '\\'; break;
                    default:
                        WriteEscapedUnicode(c);
                        return;
                }
                WriteEscapedChar(c);
            } else {
                _length++;
                if (_length >= _buffer.Length)
                    IncreaseBuffer();
                _buffer[_length - 1] = (byte)c;
            }
        }


        private void WriteUnicode2Bytes(int c) {
            _length += 2;
            if (_length > _buffer.Length)
                IncreaseBuffer();
            _buffer[_length - 2] = (byte)(0xC0 | (c >> 6));
            _buffer[_length - 1] = (byte)(0x80 | (c & 0x3F));
        }


        private void WriteUnicode3Bytes(int c) {
            _length += 3;
            if (_length > _buffer.Length)
                IncreaseBuffer();
            _buffer[_length - 3] = (byte)(0xE0 | (c >> 12));
            _buffer[_length - 2] = (byte)(0x80 | ((c >> 6) & 0x3F));
            _buffer[_length - 1] = (byte)(0x80 | (c & 0x3F));
        }


        private void WriteUnicode4Bytes(int c) {
            _length += 4;
            if (_length > _buffer.Length)
                IncreaseBuffer();
            _buffer[_length - 4] = (byte)(0xF0 | (c >> 18));
            _buffer[_length - 3] = (byte)(0x80 | ((c >> 12) & 0x3F));
            _buffer[_length - 2] = (byte)(0x80 | ((c >> 6) & 0x3F));
            _buffer[_length - 1] = (byte)(0x80 | (c & 0x3F));
        }


        private void WriteEscapedChar(int c) {
            _length += 2;
            if (_length > _buffer.Length)
                IncreaseBuffer();
            _buffer[_length - 2] = (byte)'\\';
            _buffer[_length - 1] = (byte)c;
        }


        private void WriteByte(byte b) {
            _length += 1;
            if (_length > _buffer.Length)
                IncreaseBuffer();
            _buffer[_length - 1] = b;
        }


        private void IncreaseBuffer() {
            byte[] newbuf = new byte[_buffer.Length * (1 + _length / _buffer.Length)];
            Buffer.BlockCopy(_buffer, 0, newbuf, 0, _buffer.Length);
            _buffer = newbuf;
        }

        #endregion


        #region Parser

        public static object Parse(string json_utf16) {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(json_utf16);
            object res = Parse(data, data.Length);
            return res;
        }


        public static object Parse(MemoryStream json_utf8) {
            object res = Parse(json_utf8.GetBuffer(), (int)json_utf8.Length);
            return res;
        }


        public static object Parse(Stream json_utf8) {
            byte[] buffer = new byte[BUFFER_SIZE];
            int length = 0;
            int read = buffer.Length;
            while ((read = json_utf8.Read(buffer, length, read)) > 0) {
                length += read;
                if (length + read > buffer.Length) {
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Buffer.BlockCopy(buffer, 0, newBuffer, 0, length);
                    buffer = newBuffer;
                }
            }
            object res = Parse(buffer, length);
            return res;
        }


        public static object Parse(object value) {
            string json = value as string;
            if (json != null && json.Length > 1) {
                char firstChar = json[0];
                char lastChar = json[json.Length - 1];
                if (firstChar == '{' && lastChar == '}' || firstChar == '[' && lastChar == ']') {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                    object res = Parse(data, data.Length);
                    return res;
                }
            }
            return value;
        }


        public static object Parse(byte[] json_utf8, int length) {
            int index = 0;
            object result = ParseNext(json_utf8, ref index, DEPTH_MAX);
            if (index >= length)
                throw new JsonException(E_UnexpectedTermination);
            return result;
        }


        private static object ParseNext(byte[] data, ref int index, int depth) {
            int c = PeekChar(data, ref index);
            if (c == -1)
                return null;

            switch (c) {
                case '{':  // Object
                    return ParseObject(data, ref index, depth);
                case '[':  // Array
                    return ParseArray(data, ref index, depth);
                case '"':  // String double quote
                case '\'': // String single quote
                    if (Contains(data, index + 1, "iVBORw0KG")) // PNG Base64
                        return ParseImagePNG(data, ref index);
                    return ParseQuotedString(data, ref index);
                case 'f':  // false
                    ParseLiteral(data, ref index, "false");
                    return false;
                case 't':  // true
                    ParseLiteral(data, ref index, "true");
                    return true;
                case 'n':  // null
                    ParseLiteral(data, ref index, "null");
                    return null;
                default:
                    return ParseNumber(data, ref index);
            }
        }


        private static void ParseLiteral(byte[] data, ref int index, string literal) {
            int i = index;
            index += literal.Length - 1;
            if (index >= data.Length)
                throw new JsonException(E_UnexpectedTermination, data, index);

            for (int ii = 0; ii < literal.Length; ii++) {
                if (data[i + ii] != literal[ii])
                    throw new JsonException(E_InvalidLiteral, data, index);
            }
        }


        #region Conversion table

        /// <summary>
        /// Converts a number character to the corresponding code.
        /// [+-]   43,45  => 0
        /// [0-9]  48-57  => 1   (bit 1-6)
        /// [.]    64     => 64  (bit 7)
        /// [eE]   69,101 => 128 (bit 8)
        /// Other         => 255
        /// </summary>
        private static readonly byte[] CV_DIGIT = {
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,000,255,000,064,255,
            001,001,001,001,001,001,001,001,001,001,255,255,255,255,255,255,
            255,255,255,255,255,128,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,128,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255};

        #endregion

        /// <summary>
        /// Parses a number.
        /// </summary>
        /// <returns>Json object</returns>
        private static object ParseNumber(byte[] data, ref int index) {
            int info = CV_DIGIT[data[index]];
            if (info > 1)  // if not [+-0-9]
                throw new JsonException(E_InvalidNumber, data, index);

            // find end of number
            int i = index;
            for (index++; index < data.Length; index++) {
                int v = CV_DIGIT[data[index]];
                if (v == 255)  // if char is not part of the number
                    break;
                info = info + v;
            };

            int digitsCount = info & 63;              // number of digits [0-9]
            bool hasDecimalPoint = (info & 64) != 0;  // if decimal point (bit 7)
            bool hasExponent = (info & 128) != 0;     // if exponent (bit 8)

            // extract the string
            string str;
            unsafe {
                fixed (byte* ptr = &data[i]) {
                    str = new string((sbyte*)ptr, 0, index - i);   // Ascii to Utf16
                }
            }
            index--;

            // parse the string
            try {
                if (digitsCount > 15) {
                    return decimal.Parse(str, NumberStyles.Number, CultureInfo.InvariantCulture);
                } else {
                    double dbl = double.Parse(str, NumberStyles.Float, CultureInfo.InvariantCulture);
                    if (hasDecimalPoint || hasExponent || dbl < int.MinValue || dbl > int.MaxValue)
                        return dbl;
                    return (int)dbl;
                }
            } catch {
                throw new JsonException(E_InvalidNumber, data, index);
            }
        }


        /// <summary>
        /// Parses a collection of items.
        /// </summary>
        /// <returns>Json object</returns>
        private static JsonArray ParseArray(byte[] data, ref int index, int depth) {
            if (depth-- < 0)
                throw new JsonException(E_DepthLimitExceeded, data, index);

            JsonArray list = new JsonArray();
            int c = -1;
            for (index++; index < data.Length; index++) {
                c = PeekChar(data, ref index);
                if (c == ']')
                    return list;

                object value = ParseNext(data, ref index, depth);
                list.Add(value);

                index++;
                c = PeekChar(data, ref index);
                if (c != ',')
                    break;
            }
            if (c == ']')
                return list;
            if (index >= data.Length)
                throw new JsonException(E_UnexpectedTermination, data, index);
            throw new JsonException(E_InvalidArray, data, index);
        }


        /// <summary>
        /// Parses a collection of key/value.
        /// </summary>
        /// <returns>Json object</returns>
        private static JsonObject ParseObject(byte[] data, ref int index, int depth) {
            if (depth-- < 0)
                throw new JsonException(E_DepthLimitExceeded, data, index);

            JsonObject members = new JsonObject();
            int c = -1;
            while (++index < data.Length) {
                c = PeekChar(data, ref index);
                if (c == '}')
                    return members;

                string key = ParseQuotedString(data, ref index);

                index++;
                c = PeekChar(data, ref index);
                if (c != ':' || key.Length == 0)
                    break;

                index++;
                object value = ParseNext(data, ref index, depth);
                members.Add(key, value);

                index++;
                c = PeekChar(data, ref index);
                if (c != ',')
                    break;
            }
            if (c == '}')
                return members;
            if (index >= data.Length)
                throw new JsonException(E_UnexpectedTermination, data, index);
            throw new JsonException(E_InvalidObject, data, index);
        }


        /// <summary>
        /// Parses a PNG image from a base64 string delimited by a double or single quote.
        /// </summary>
        /// <returns>Json image</returns>
        private static JsonImage ParseImagePNG(byte[] data, ref int index) {
            MemoryStream mstream = ParseBase64String(data, ref index);
            JsonImage img = new JsonImage(mstream);
            return img;
        }


        #region Conversion table

        /// <summary>
        /// Decode a base 64 charater to the corresponding number.
        /// [A-F] 65-90  => 0-25
        /// [a-f] 97-122 => 26-51
        /// [0-9] 49-57  => 52-61
        /// [+]   43     => 62
        /// [/]   47     => 63
        /// Other        => 255
        /// </summary>
        private static readonly byte[] DEC_BASE64 = {
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,062,255,255,255,063,
            052,053,054,055,056,057,058,059,060,061,255,255,255,255,255,255,
            255,000,001,002,003,004,005,006,007,008,009,010,011,012,013,014,
            015,016,017,018,019,020,021,022,023,024,025,255,129,255,255,255,
            255,026,027,028,029,030,031,032,033,034,035,036,037,038,039,040,
            041,042,043,044,045,046,047,048,049,050,051,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255};

        #endregion

        /// <summary>
        /// Parses a base64 string delimited by a double or single quote.
        /// </summary>
        /// <returns>Memory stream</returns>
        private static unsafe MemoryStream ParseBase64String(byte[] data, ref int index) {
            int quoteChar = data[index];
            int i = index + 1;

            // find end of data
            while (++index < data.Length && data[index] != quoteChar) ;
            if (index >= data.Length)
                throw new JsonException(E_UnexpectedTermination, data, index);

            // trim end of data
            int ii = index - 1;
            for (ii -= 1; ii > i; ii--) {
                if (data[ii - 1] == '\\') {
                    ii -= 1;
                } else if (DEC_BASE64[data[ii]] <= 63) {
                    break;
                }
            }

            int srcCount = (ii - i + 1);
            byte[] dest = new byte[(srcCount / 4) * 3 + 3];
            fixed (byte* pSrc0 = data, pDest0 = dest) {
                byte* ptrSrc = pSrc0 + i;
                byte* ptrSrcBreak = pSrc0 + ii - 3;
                byte* ptrDest = pDest0;
                while (ptrSrc < ptrSrcBreak) {
                    int code = DEC_BASE64[ptrSrc[0]];
                    if (code <= 63) {
                        int val = code << 18
                                | DEC_BASE64[ptrSrc[1]] << 12
                                | DEC_BASE64[ptrSrc[2]] << 6
                                | DEC_BASE64[ptrSrc[3]];
                        ptrDest[0] = (byte)(val >> 16);
                        ptrDest[1] = (byte)(val >> 8);
                        ptrDest[2] = (byte)val;
                        ptrSrc += 4;
                        ptrDest += 3;
                    } else {
                        ptrSrc += (*ptrSrc == '\\' ? 2 : 1);
                    }
                }
                int remain = (int)(pSrc0 + ii - ptrSrc);
                if (remain > 1) {
                    byte b0 = DEC_BASE64[ptrSrc[0]];
                    byte b1 = DEC_BASE64[ptrSrc[1]];
                    *ptrDest++ = (byte)(b0 << 2 | b1 >> 4);
                    if (remain > 2) {
                        byte b2 = DEC_BASE64[ptrSrc[2]];
                        *ptrDest++ = (byte)(b1 << 4 | b2 >> 2);
                    }
                }
                if (ptrDest == pDest0)
                    return new MemoryStream(0);
                return new MemoryStream(dest, 0, (int)(ptrDest - pDest0));
            }
        }


        /// <summary>
        /// Parses a string delimited by a double or single quote.
        /// </summary>
        /// <returns>String</returns>
        private static string ParseQuotedString(byte[] data, ref int index) {
            int quoteChar = data[index];
            int i = index;

            // find end of data
            while (++index < data.Length) {
                if (data[index] == '\\') {
                    index++;
                } else if (data[index] == quoteChar) {
                    break;
                }
            }
            if (index >= data.Length)
                throw new JsonException(E_UnexpectedTermination, data, index);

            int len = 0;
            char[] buf = new char[index - i];
            while (++i < index) {
                int c = data[i];
                if (c < 0x80) {
                    // UTF8 1 bytes
                    if (c == '\\') {
                        c = data[++i];
                        switch (c) {
                            case 'b': c = '\b'; break;
                            case 'f': c = '\f'; break;
                            case 'n': c = '\n'; break;
                            case 'r': c = '\r'; break;
                            case 't': c = '\t'; break;
                            case 'u': c = ParseUnicode(data, ref i); break;
                        };
                    }
                    buf[len++] = (char)c;
                } else {
                    int b1 = data[++i];
                    if (c < 0xE0) {
                        // UTF8 2 bytes sequence
                        buf[len++] = (char)((c << 6) + b1 - 0x3080);
                    } else {
                        int b2 = data[++i];
                        if (c < 0xF0) {
                            // UTF8 3 bytes sequence
                            buf[len++] = (char)((c << 12) + (b1 << 6) + b2 - 0xE2080);
                        } else if (c < 0xF5) {
                            // UTF8 4 bytes sequence
                            int b3 = data[++i];
                            int w = (c << 18) + (b1 << 12) + (b2 << 6) + b3 - 0x3C82080;
                            buf[len++] = (char)((w >> 10) + 0xD7C0);
                            buf[len++] = (char)((w & 0x3FF) + 0xDC00);
                        }
                    }
                }
            }
            return new string(buf, 0, len);
        }


        #region Conversion table

        /// <summary>
        /// Decode a base 16 character to the corresponding number.
        /// [0-9] 48-57  => 0-9
        /// [A-F] 65-70  => 10-15
        /// [a-f] 97-102 => 10-15
        /// </summary>
        private static readonly byte[] DEC_BASE16 = {
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            000,001,002,003,004,005,006,007,008,009,255,255,255,255,255,255,
            255,010,011,012,013,014,015,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,
            255,010,011,012,013,014,015,255,255,255,255,255,255,255,255,255,
            255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255};

        #endregion

        /// <summary>
        /// Parses an unicode string on 6 chars. Ex: \u20AC.
        /// </summary>
        /// <returns>Character</returns>
        private static char ParseUnicode(byte[] data, ref int index) {
            index += 4;
            if (index >= data.Length)
                throw new JsonException(E_UnexpectedTermination, data, index);

            int b0 = DEC_BASE16[data[index - 3]];
            int b1 = DEC_BASE16[data[index - 2]];
            int b2 = DEC_BASE16[data[index - 1]];
            int b3 = DEC_BASE16[data[index]];
            if ((b0 | b1 | b2 | b3) == 255)
                throw new JsonException(E_InvalidUnicode, data, index);

            int code = (b0 << 12) | (b1 << 8) | (b2 << 4) | b3;
            return (char)code;
        }


        /// <summary>
        /// Returns a non empty character
        /// </summary>
        /// <returns>Unicode code or -1 if end of bytes</returns>
        private static int PeekChar(byte[] data, ref int index) {
            for (; index < data.Length; index++) {
                int c = data[index];
                if (c > 0x7F) {     // If more than one byte
                    if (++index >= data.Length)
                        throw new JsonException(E_UnexpectedTermination, data, index);
                    if (c < 0xE0) {
                        // UTF8 2 bytes sequence
                        c = (c << 6) + data[index] - 0x3080;
                    } else if (c < 0xF0) {
                        // UTF8 3 bytes sequence
                        if (++index >= data.Length)
                            throw new JsonException(E_UnexpectedTermination, data, index);
                        c = (c << 12) + (data[index - 1] << 6) + data[index] - 0xE2080;
                    } else {
                        // UTF8 4 bytes sequence. Should only be present in a quoted string
                        throw new JsonException(E_UnexpectedChar, data, index);
                    }
                }

                // Return the character if it's not a control or space
                if ((c > 32 && c < 127) || c > 160)
                    return c;
            };
            return -1;
        }


        private static bool Contains(byte[] data, int index, string text) {
            if (index + text.Length >= data.Length)
                return false;

            for (int i = 0; i < text.Length; i++) {
                if (data[index + i] != text[i])
                    return false;
            }
            return true;
        }

        #endregion

    }
}
