using Cryptography.Gost.Modes;

namespace Cryptography.Gost
{
    public class Gost28147 : IDisposable
 
    {
        public byte[][] SBlock { get; private set; } =
        {
            new byte[] { 0xA, 0x9, 0xD, 0x6, 0xE, 0xB, 0x4, 0x5, 0xF, 0x1, 0x3, 0xC, 0x7, 0x0, 0x8, 0x2 },
            new byte[] { 0x8, 0x0, 0xC, 0x4, 0x9, 0x6, 0x7, 0xB, 0x2, 0x3, 0x1, 0xF, 0x5, 0xE, 0xA, 0xD },
            new byte[] { 0xF, 0x6, 0x5, 0x8, 0xE, 0xB, 0xA, 0x4, 0xC, 0x0, 0x3, 0x7, 0x2, 0x9, 0x1, 0xD },
            new byte[] { 0x3, 0x8, 0xD, 0x9, 0x6, 0xB, 0xF, 0x0, 0x2, 0x5, 0xC, 0xA, 0x4, 0xE, 0x1, 0x7 },
            new byte[] { 0xF, 0x8, 0xE, 0x9, 0x7, 0x2, 0x0, 0xD, 0xC, 0x6, 0x1, 0x5, 0xB, 0x4, 0x3, 0xA },
            new byte[] { 0x2, 0x8, 0x9, 0x7, 0x5, 0xF, 0x0, 0xB, 0xC, 0x1, 0xD, 0xE, 0xA, 0x3, 0x6, 0x4 },
            new byte[] { 0x3, 0x8, 0xB, 0x5, 0x6, 0x4, 0xE, 0xA, 0x2, 0xC, 0x1, 0x7, 0x9, 0xF, 0xD, 0x0 },
            new byte[] { 0x1, 0x2, 0x3, 0xE, 0x6, 0xD, 0xB, 0x8, 0xF, 0xA, 0xC, 0x5, 0x7, 0x9, 0x0, 0x4 }
        };

        // Subkeys.
        private readonly List<uint> _subKeys;

        // 64 bit IV.
        private byte[] _iv;

        // 256 bit key;
        private byte[] _key;

        // Message.
        private byte[] _message;

        private bool released;

        // GOST stream cipher.
        public Gost28147(byte[] key, byte[] iv = null)
        {
            released = false;
            _key = key;
            _iv = iv;
            _subKeys = new List<uint>();
        }

        // Check key length.
        private byte[] Key
        {
            get => _key;
            set
            {
                if (value.Length != 32)
                    throw new ArgumentException("Wrong key. Try to use 256 bit key.");
                if (value.Length == 32) _key = value;
            }
        }

        // Check message.
        private byte[] Message
        {
            get => _message;
            set
            {
                if (value == null || value.Length == 0)
                    throw new ArgumentException("Empty message!");
                _message = value;
            }
        }

        // Check IV.
        private byte[] IV
        {
            get => _iv;
            set
            {
                if (value.Length < 8)
                    throw new ArgumentException("Wrong IV. Try to use 64 bit IV.");
                if (value.Length >= 8) _iv = value.Take(8).ToArray();
            }
        }

        // Substitution encode. Gets 256 bit key, plain message multiple of 64 bit
        //      and returns encoded message
        public byte[] SubstitutionEncode(byte[] key, byte[] message)
        {
            Key = key;
            Message = AddPadding(message);

            var encode = EcbEncodeOrDecode(true);
            return encode;
        }

        // Substitution decode.
        public byte[] SubstitutionDecode(byte[] key, byte[] message)
        {
            Key = key;
            Message = message;
            if (message.Length % 8 != 0) throw new ArgumentException("Block must have 64 bit length");

            var decode = EcbEncodeOrDecode(false);
            return RemovePadding(decode);
        }

        // XOR encode.
        public byte[] XOREncode(byte[] key, byte[] iv, byte[] message)
        {
            Key = key;
            Message = message;
            IV = iv;

            var encode = XOREncodeOrDecode();
            return encode;
        }

        //XOR decode.
        public byte[] XORDecode(byte[] key, byte[] iv, byte[] message)
        {
            return XOREncode(key, iv, message);
        }

        // CFB encode.
        public byte[] CFBEncode(byte[] key, byte[] iv, byte[] message)
        {
            Key = key;
            Message = message;
            IV = iv;

            var encode = CFBEncodeOrDecode(true);
            return encode;
        }

        // CFB decode.
        public byte[] CFBDecode(byte[] key, byte[] iv, byte[] message)
        {
            Key = key;
            Message = message;
            IV = iv;

            var encode = CFBEncodeOrDecode(false);
            return encode;
        }

        public void Dispose()
        {
            if (!released)
            {
                released = true;

                SBlock = null;
                _message = null;
                _key = null;
                _iv = null;
                _subKeys.Clear();
            }
        }

        // Generate subkeys.
        private void GetSubKeys()
        {
            var res = new byte[4];
            // Stage 1.
            var j = 0;
            for (var i = 0; i != _key.Length; i++)
            {
                res[j] = _key[i];

                if (j % 3 == 0 && j != 0)
                {
                    _subKeys.Add(BitConverter.ToUInt32(res, 0));
                    j = 0;
                }
                else
                {
                    j++;
                }
            }

            // Stage 2.
            for (var i = 0; i != 16; i++) _subKeys.Add(_subKeys[i]);
            // Stage 3.
            for (var i = 7; i != -1; i--) _subKeys.Add(_subKeys[i]);
        }

        // Substitution.
        private byte[] EcbEncodeOrDecode(bool encode)
        {
            var cipher = new SubstitutionMode(SBlock);
            GetSubKeys();
            if (!encode) _subKeys.Reverse();
            var res = new byte[_message.Length];
            var index = 0;

            foreach (var chunk in ReadByChunk())
            {
                Array.Copy(encode ? cipher.EncodeProcess(chunk, _subKeys) : cipher.DecodeProcess(chunk, _subKeys), 0, res,
                    index, 8);
                index += 8;
            }

            return res;
        }

        // XOR.
        private byte[] XOREncodeOrDecode()
        {
            var cipher = new XORMode(SBlock);

            GetSubKeys();

            var res = new byte[_message.Length];
            var index = 0;

            cipher.SetIV(_iv, _subKeys);

            foreach (var chunk in ReadByChunk())
            {
                Array.Copy(cipher.EncodeProcess(chunk, _subKeys), 0, res, index, chunk.Length);
                index += chunk.Length;
            }

            return res;
        }

        // CFB.
        private byte[] CFBEncodeOrDecode(bool encode)
        {
            var cipher = new CFBMode(SBlock);

            GetSubKeys();

            var res = new byte[_message.Length];
            var index = 0;

            cipher.SetIV(_iv);

            foreach (var chunk in ReadByChunk())
            {
                if (encode)
                    Array.Copy(cipher.EncodeProcess(chunk, _subKeys), 0, res, index, chunk.Length);
                else
                    Array.Copy(cipher.DecodeProcess(chunk, _subKeys), 0, res, index, chunk.Length);
                index += chunk.Length;
            }

            return res;
        }

        // Read message by chunks.
        private IEnumerable<byte[]> ReadByChunk()
        {
            for (var i = 0; i < _message.Length; i += 8)
            {
                var min = Math.Min(8, _message.Length - i);

                var res = new byte[min];

                Array.Copy(_message, i, res, 0, min);

                yield return res;
            }
        }

        private static byte[] AddPadding(byte[] bytes)
        {
            if (bytes.Length % 8 == 0) return bytes;

            int paddingLength;
            byte[] sequenceToPad = new byte[0];
            if (bytes.Length % 8 != 0)
            {
                paddingLength = 8 - bytes.Length % 8;
                int startIndex = bytes.Length - (8 - paddingLength);
                if (bytes.Length < 8 || startIndex < 0) startIndex = 0;
                sequenceToPad = bytes.AsSpan(startIndex, bytes.Length % 8).ToArray();
                bytes = bytes.AsSpan(0, bytes.Length - bytes.Length % 8).ToArray();
            }
            byte[] paddedSequence = Pkcs7.AddPkcs7Padding(sequenceToPad, 8);
            List<byte> bytesList = new List<byte>(bytes);
            bytesList.AddRange(paddedSequence);
            return bytesList.ToArray();
        }

        private static byte[] RemovePadding(byte[] bytes)
        {
            return Pkcs7.RemovePkcs7Padding(bytes);
        }
    }
}