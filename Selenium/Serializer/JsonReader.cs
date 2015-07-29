using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using JsonArray = Selenium.List;
using JsonObject = Selenium.Dictionary;
using JsonImage = Selenium.Image;

namespace Selenium.Serializer {

    /// <summary>
    /// Json deserializer for data encoded in UTF8. 
    /// Automatically decode PNG images encoded to a Base64 string.
    /// Primitve numbers are returned as :
    ///  {double} when containing the exponential letter (E or e)
    ///  {decimal} when containing a decimal point
    ///  {int} when containing only digits and fitting in an Int32
    ///  {long} when containing only digits and not fitting in an Int32
    /// </summary>
    [DebuggerDisplay("{DebugValue,nq}")]
    internal class JsonReader {

        const int DEPTH_MAX = 8;    //maximum depth for arrays and dictionaries
        const int DEFAULT_BUFFER_SIZE = 1024;

        public static object Deserialize(Stream stream) {
            byte[] buffer = new byte[DEFAULT_BUFFER_SIZE];
            int length = 0;
            int read = buffer.Length;
            while ((read = stream.Read(buffer, length, read)) > 0) {
                length += read;
                if (buffer.Length < length + read) {
                    byte[] newBuffer = new byte[buffer.Length * 2];
                    Buffer.BlockCopy(buffer, 0, newBuffer, 0, length);
                    buffer = newBuffer;
                }
            }
            object res = new JsonReader().DeserializeBytes(buffer, length);
            return res;
        }

        public static object Deserialize(MemoryStream stream) {
            object res = new JsonReader().DeserializeBytes(stream.GetBuffer(), (int)stream.Length);
            return res;
        }

        public static object Deserialize(byte[] data, int length) {
            object res = new JsonReader().DeserializeBytes(data, length);
            return res;
        }

        public static object Deserialize(string text) {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(text);
            object res = new JsonReader().DeserializeBytes(data, data.Length);
            return res;
        }

        public static object Parse(object value) {
            string json = value as string;
            if (json != null && json.Length > 1) {
                char fc = json[0];
                char lc = json[json.Length - 1];
                if (fc == '{' && lc == '}' || fc == '[' && lc == ']') {
                    byte[] data = System.Text.Encoding.UTF8.GetBytes(json);
                    object res = new JsonReader().DeserializeBytes(data, data.Length);
                    return res;
                }
            }
            return value;
        }


        private byte[] _buffer;   //bytes to deserialize
        private int _length;    //length of data
        private int _index;     //reading index for the data
        private char _current;  //Current character set by MoveNextNonEmptyChar()

        private object DeserializeBytes(byte[] data, int length) {
            _index = 0;
            _buffer = data;
            _length = length;
            object result = DeserializeNext(DEPTH_MAX);
            if (MoveNextNonEmptyChar())
                throw new JsonException("IllegalPrimitive", _index);
            return result;
        }

        private string DebugValue {
            get {
                return System.Text.Encoding.UTF8.GetString(_buffer, 0, _length);
            }
        }

        private object DeserializeNext(int depth) {
            if (depth++ < DEPTH_MAX)
                throw new JsonException("DepthLimitExceeded", _index);

            _index--;
            if (!MoveNextNonEmptyChar())
                return null;

            switch (_current) {
                case '{':  //Object
                    return DeserializeObject(depth);
                case '[':  //Array
                    return DeserializeArray(depth);
                case '\'':  //String
                case '"':  //String
                    if (HasSequence(_buffer, _index + 1, "iVBORw0KG")) //PNG Base64
                        return DeserializePNGBase64();
                    return DeserializeString();
                default:
                    return DeserializePrimitive();
            }
        }

        private object DeserializePrimitive() {
            bool hasExponent = false;
            bool hasDecimalPoint = false;

            //find next element and tag any exponent or decimal point
            int indexNext = _index;
            for (; indexNext < _length; indexNext++) {
                char c = (char)_buffer[indexNext];
                if ((c >= '0' && c <= '9')
                    || (c >= 'A' && c <= 'Z')
                    || (c >= 'a' && c <= 'z')
                    || c == '.' || c == '-' || c == '+') {
                    switch (c) {
                        case 'E':
                        case 'e': hasExponent = true; break;
                        case '.': hasDecimalPoint = true; break;
                    }
                    continue;
                }
                break;
            };

            if (indexNext == _index)
                throw new JsonException("EmptyPrimitive", _index);

            //get the string
            string str = StringASCII(_buffer, _index, indexNext - _index);
            _index = indexNext - 1;

            //parse the string
            if (str == "null")
                return null;
            if (str == "true")
                return true;
            if (str == "false")
                return false;
            if (hasExponent) {
                Double numDouble;
                if (Double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out numDouble))
                    return numDouble;
            } else if (hasDecimalPoint) {
                Decimal numDecimal;
                if (decimal.TryParse(str, NumberStyles.Number, CultureInfo.InvariantCulture, out numDecimal))
                    return numDecimal;
            } else {
                int numInt32;
                if (int.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out numInt32))
                    return numInt32;
                long numInt64;
                if (long.TryParse(str, NumberStyles.Integer, CultureInfo.InvariantCulture, out numInt64))
                    return numInt64;
            }
            throw new JsonException("IllegalPrimitive: " + str, _index - str.Length + 1);
        }

        private static unsafe string StringASCII(byte[] bytes, int index, int count) {
            fixed (byte* ptr = &bytes[index])
                return new string((sbyte*)ptr, 0, count);
        }

        private JsonArray DeserializeArray(int depth) {
            JsonArray list = new JsonArray();
            while (MoveNextNonEmptyChar() && _current != ']' && _current != ',') {
                var obj = DeserializeNext(depth);
                list.Add(obj);
                if (!MoveNextNonEmptyChar() || _current != ',')
                    break;
            }
            if (_current == ']')
                return list;
            if (_index >= _length)
                throw new JsonException("UnexpectedTermination", _index);
            throw new JsonException("InvalideArray", _index);
        }

        private JsonObject DeserializeObject(int depth) {
            JsonObject dictionary = new JsonObject();
            while (MoveNextNonEmptyChar() && (_current == '"' || _current == '\'')) {

                string memberName = DeserializeString();
                if (memberName == string.Empty
                    || !MoveNextNonEmptyChar()
                    || _current != ':'
                    || !MoveNextNonEmptyChar()
                    || _current == '}' || _current == ',')
                    break;

                object propVal = DeserializeNext(depth);
                dictionary.Add(memberName, propVal);

                if (!MoveNextNonEmptyChar() || _current != ',')
                    break;
            }
            if (_current == '}')
                return dictionary;
            if (_index >= _length)
                throw new JsonException("UnexpectedTermination", _index);
            throw new JsonException("InvalideObject", _index);
        }

        private JsonImage DeserializePNGBase64() {
            byte[] data = DeserializeBase64String();
            JsonImage img = new JsonImage(data);
            return img;
        }

        #region Base64 encoding table

        static readonly byte[] BASE64_TABLE = {
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,62,99,99,99,63,
            52,53,54,55,56,57,58,59,60,61,99,99,99,99,99,99,
            99,00,01,02,03,04,05,06,07,08,09,10,11,12,13,14,
            15,16,17,18,19,20,21,22,23,24,25,99,99,99,99,99,
            99,26,27,28,29,30,31,32,33,34,35,36,37,38,39,40,
            41,42,43,44,45,46,47,48,49,50,51,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,
            99,99,99,99,99,99,99,99,99,99,99,99,99,99,99,99};

        #endregion

        /// <summary>
        /// Deserialise a base64 string.
        /// Line breaks are not supported and data are not verified
        /// </summary>
        /// <returns></returns>
        private unsafe byte[] DeserializeBase64String() {
            int quoteChar = _current;
            byte[] src = _buffer;
            int srcIdxStart = _index + 1;

            //find end of data
            int srcIdxEndQuote = _index;
            try {
                while (src[++srcIdxEndQuote] != quoteChar) ;
            } catch (IndexOutOfRangeException) {
                throw new JsonException("UnterminatedString", _length);
            }

            //trim end padding char '='
            int srcIdxEnd = srcIdxEndQuote - 1;
            while (src[srcIdxEnd] == (int)'=')
                srcIdxEnd--;

            int srcCount = (srcIdxEnd - srcIdxStart + 1);
            int blocksCount = srcCount / 4;  //count blocks of 4chars/3bytes
            int remainCount = (((srcCount % 4) + 1) >> 1);  //remaining bytes
            byte[] dest = new byte[blocksCount * 3 + remainCount];
            fixed (byte* pSrc0 = _buffer, pDest0 = dest, pMap0 = BASE64_TABLE) {
                byte* pSrc = pSrc0 + srcIdxStart;
                byte* pDest = pDest0;
                unchecked {
                    for (int i = blocksCount; i-- > 0; pSrc += 4, pDest += 3) {
                        int v = pMap0[pSrc[0]] << 18
                            | pMap0[pSrc[1]] << 12
                            | pMap0[pSrc[2]] << 6
                            | pMap0[pSrc[3]];
                        pDest[0] = (byte)(v >> 16);
                        pDest[1] = (byte)(v >> 8);
                        pDest[2] = (byte)v;
                    }
                }
                if (remainCount > 0) {
                    byte b0 = pMap0[pSrc[0]];
                    byte b1 = pMap0[pSrc[1]];
                    pDest[0] = (byte)(b0 << 2 | b1 >> 4);
                    if (remainCount > 1) {
                        byte b2 = pMap0[pSrc[2]];
                        pDest[1] = (byte)(b1 << 4 | b2 >> 2);
                    }
                }
            }
            _index = srcIdxEndQuote;
            return dest;
        }

        private string DeserializeString() {
            int quoteChar = _current;
            bool escaped = false;
            int txtLength = 0;
            int txtCapacity = 256;
            char[] txtBuffer = new char[txtCapacity + 4];

            while (++_index < _length) {
                if (txtLength > txtCapacity) {
                    txtCapacity *= 2; //double the capacity
                    char[] txtBuffer2 = new char[txtCapacity + 4]; //adds 4 bytes for safety
                    Buffer.BlockCopy(txtBuffer, 0, txtBuffer2, 0, txtLength * 2); //2 bytes per char
                    txtBuffer = txtBuffer2;
                }

                int b0 = _buffer[_index];
                if (b0 < 0x80) {
                    // UTF8 1 bytes
                    if (b0 == '\\') {
                        if (escaped ^= true)
                            continue;
                    } else if (escaped) {
                        escaped = false;
                        switch (b0) {
                            case '"':
                            case '\'':
                            case '/': break;
                            case 'b': b0 = '\b'; break;
                            case 'f': b0 = '\f'; break;
                            case 'n': b0 = '\n'; break;
                            case 'r': b0 = '\r'; break;
                            case 't': b0 = '\t'; break;
                            case 'u': b0 = DeserializeUnicode(); break;
                            default:
                                throw new JsonException("StringBadEscape", _index);
                        };
                    } else if (b0 == quoteChar) {
                        return new string(txtBuffer, 0, txtLength);
                    }
                    txtBuffer[txtLength++] = (char)b0;
                } else {
                    int b1 = _buffer[++_index];
                    if (b0 < 0xE0) {
                        // UTF8 2 bytes sequence
                        txtBuffer[txtLength++] = (char)((b0 << 6) + b1 - 0x3080);
                    } else {
                        int b2 = _buffer[++_index];
                        if (b0 < 0xF0) {
                            // UTF8 3 bytes sequence
                            txtBuffer[txtLength++] = (char)((b0 << 12) + (b1 << 6) + b2 - 0xE2080);
                        } else if (b0 < 0xF5) {
                            // UTF8 4 bytes sequence
                            int b3 = _buffer[++_index];
                            int w = (b0 << 18) + (b1 << 12) + (b2 << 6) + b3 - 0x3C82080;
                            txtBuffer[txtLength++] = (char)((w >> 10) + 0xD7C0);
                            txtBuffer[txtLength++] = (char)((w & 0x3FF) + 0xDC00);
                        }
                    }
                }
            }
            throw new JsonException("UnexpectedTermination", _length);
        }


        #region Base16 encoding table

        /// <summary>
        /// Translates a base 16 char code masked with 0x1F to the corresponding number.
        /// BASE16_TABLE['F' &amp; 0x1f] gives 15
        /// BASE16_TABLE['5' &amp; 0x1f] gives 5
        /// </summary>
        static readonly byte[] BASE16_TABLE = {
            00,10,11,12,13,14,15,00,00,00,00,00,00,00,00,00,
            00,01,02,03,04,05,06,07,08,09,00,00,00,00,00,00};

        #endregion

        /// <summary>
        /// Deserialise an unicode string on 6 chars.
        /// Ex: "\u006A" returns 'j'
        /// </summary>
        /// <returns>Char</returns>
        private char DeserializeUnicode() {
            if (_index + 5 >= _length)
                throw new JsonException("UnexpectedTermination", _index);
            int code = (BASE16_TABLE[_buffer[_index + 1] & 0x1F] << 12)
                | (BASE16_TABLE[_buffer[_index + 2] & 0x1F] << 8)
                | (BASE16_TABLE[_buffer[_index + 3] & 0x1F] << 4)
                | (BASE16_TABLE[_buffer[_index + 4] & 0x1F]);
            _index += 4;
            return (char)code;
        }

        /// <summary>
        /// Move the position to the next empty character.
        /// Empty chars: SPACE, HT, LF, VT, FF, CR, NO-BREAK SPACE, NEXT LINE
        /// </summary>
        /// <returns>True if not end of bytes, false otherwise</returns>
        private bool MoveNextNonEmptyChar() {
            while (++_index < _length) {
                int b0 = _buffer[_index];
                if (b0 > 0x7F) {
                    if (++_index >= _length)
                        throw new JsonException("UnterminatedString", _length);
                    int b1 = _buffer[_index];
                    if (b0 < 0xE0) {
                        // UTF8 2 bytes sequence
                        b0 = (b0 << 6) + b1 - 0x3080;
                    } else if (b0 < 0xF0) {
                        // UTF8 3 bytes sequence
                        if (++_index >= _length)
                            throw new JsonException("UnterminatedString", _length);
                        int b2 = _buffer[_index];
                        b0 = (b0 << 12) + (b1 << 6) + b2 - 0xE2080;
                    } else {
                        // UTF8 4 bytes sequence. Should only be present in a quoted string
                        throw new JsonException("Unexpected4BytesSeq", _index);
                    }
                }
                //Exit if the char is not empty
                if (b0 != ' ' && (b0 < '\x09' || b0 > '\x0D') && b0 != '\x00A0' && b0 != '\x0085') {
                    _current = (char)b0;
                    return true;
                }
            };
            _current = '\0';
            return false;
        }

        private static bool HasSequence(byte[] buffer, int offset, string text) {
            if (buffer.Length - offset < text.Length)
                return false;
            for (int i = 0; i < text.Length; i++) {
                if (buffer[offset + i] != text[i])
                    return false;
            }
            return true;
        }

    }

}
