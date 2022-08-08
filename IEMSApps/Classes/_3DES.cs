using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IEMSApps.Utils;

namespace IEMSApps.Classes
{
    public class _3DES
    {
        public int DES3_BLOCKSIZE = 8;
        int x = 0;
        public struct des3_context
        {
            public uint[] esk;    /* Triple-DES encryption subkeys */
            public uint[] dsk;     /* Triple-DES decryption subkeys */
        }

        static uint[] SB1 = new uint[64]
        {
            0x01010400, 0x00000000, 0x00010000, 0x01010404,
            0x01010004, 0x00010404, 0x00000004, 0x00010000,
            0x00000400, 0x01010400, 0x01010404, 0x00000400,
            0x01000404, 0x01010004, 0x01000000, 0x00000004,
            0x00000404, 0x01000400, 0x01000400, 0x00010400,
            0x00010400, 0x01010000, 0x01010000, 0x01000404,
            0x00010004, 0x01000004, 0x01000004, 0x00010004,
            0x00000000, 0x00000404, 0x00010404, 0x01000000,
            0x00010000, 0x01010404, 0x00000004, 0x01010000,
            0x01010400, 0x01000000, 0x01000000, 0x00000400,
            0x01010004, 0x00010000, 0x00010400, 0x01000004,
            0x00000400, 0x00000004, 0x01000404, 0x00010404,
            0x01010404, 0x00010004, 0x01010000, 0x01000404,
            0x01000004, 0x00000404, 0x00010404, 0x01010400,
            0x00000404, 0x01000400, 0x01000400, 0x00000000,
            0x00010004, 0x00010400, 0x00000000, 0x01010004
        };

        static uint[] SB2 = new uint[64]
        {
            0x80108020, 0x80008000, 0x00008000, 0x00108020,
            0x00100000, 0x00000020, 0x80100020, 0x80008020,
            0x80000020, 0x80108020, 0x80108000, 0x80000000,
            0x80008000, 0x00100000, 0x00000020, 0x80100020,
            0x00108000, 0x00100020, 0x80008020, 0x00000000,
            0x80000000, 0x00008000, 0x00108020, 0x80100000,
            0x00100020, 0x80000020, 0x00000000, 0x00108000,
            0x00008020, 0x80108000, 0x80100000, 0x00008020,
            0x00000000, 0x00108020, 0x80100020, 0x00100000,
            0x80008020, 0x80100000, 0x80108000, 0x00008000,
            0x80100000, 0x80008000, 0x00000020, 0x80108020,
            0x00108020, 0x00000020, 0x00008000, 0x80000000,
            0x00008020, 0x80108000, 0x00100000, 0x80000020,
            0x00100020, 0x80008020, 0x80000020, 0x00100020,
            0x00108000, 0x00000000, 0x80008000, 0x00008020,
            0x80000000, 0x80100020, 0x80108020, 0x00108000
        };

        static uint[] SB3 = new uint[64]
        {
            0x00000208, 0x08020200, 0x00000000, 0x08020008,
            0x08000200, 0x00000000, 0x00020208, 0x08000200,
            0x00020008, 0x08000008, 0x08000008, 0x00020000,
            0x08020208, 0x00020008, 0x08020000, 0x00000208,
            0x08000000, 0x00000008, 0x08020200, 0x00000200,
            0x00020200, 0x08020000, 0x08020008, 0x00020208,
            0x08000208, 0x00020200, 0x00020000, 0x08000208,
            0x00000008, 0x08020208, 0x00000200, 0x08000000,
            0x08020200, 0x08000000, 0x00020008, 0x00000208,
            0x00020000, 0x08020200, 0x08000200, 0x00000000,
            0x00000200, 0x00020008, 0x08020208, 0x08000200,
            0x08000008, 0x00000200, 0x00000000, 0x08020008,
            0x08000208, 0x00020000, 0x08000000, 0x08020208,
            0x00000008, 0x00020208, 0x00020200, 0x08000008,
            0x08020000, 0x08000208, 0x00000208, 0x08020000,
            0x00020208, 0x00000008, 0x08020008, 0x00020200
        };

        static uint[] SB4 = new uint[64]
        {
            0x00802001, 0x00002081, 0x00002081, 0x00000080,
            0x00802080, 0x00800081, 0x00800001, 0x00002001,
            0x00000000, 0x00802000, 0x00802000, 0x00802081,
            0x00000081, 0x00000000, 0x00800080, 0x00800001,
            0x00000001, 0x00002000, 0x00800000, 0x00802001,
            0x00000080, 0x00800000, 0x00002001, 0x00002080,
            0x00800081, 0x00000001, 0x00002080, 0x00800080,
            0x00002000, 0x00802080, 0x00802081, 0x00000081,
            0x00800080, 0x00800001, 0x00802000, 0x00802081,
            0x00000081, 0x00000000, 0x00000000, 0x00802000,
            0x00002080, 0x00800080, 0x00800081, 0x00000001,
            0x00802001, 0x00002081, 0x00002081, 0x00000080,
            0x00802081, 0x00000081, 0x00000001, 0x00002000,
            0x00800001, 0x00002001, 0x00802080, 0x00800081,
            0x00002001, 0x00002080, 0x00800000, 0x00802001,
            0x00000080, 0x00800000, 0x00002000, 0x00802080
        };

        static uint[] SB5 = new uint[64]
        {
            0x00000100, 0x02080100, 0x02080000, 0x42000100,
            0x00080000, 0x00000100, 0x40000000, 0x02080000,
            0x40080100, 0x00080000, 0x02000100, 0x40080100,
            0x42000100, 0x42080000, 0x00080100, 0x40000000,
            0x02000000, 0x40080000, 0x40080000, 0x00000000,
            0x40000100, 0x42080100, 0x42080100, 0x02000100,
            0x42080000, 0x40000100, 0x00000000, 0x42000000,
            0x02080100, 0x02000000, 0x42000000, 0x00080100,
            0x00080000, 0x42000100, 0x00000100, 0x02000000,
            0x40000000, 0x02080000, 0x42000100, 0x40080100,
            0x02000100, 0x40000000, 0x42080000, 0x02080100,
            0x40080100, 0x00000100, 0x02000000, 0x42080000,
            0x42080100, 0x00080100, 0x42000000, 0x42080100,
            0x02080000, 0x00000000, 0x40080000, 0x42000000,
            0x00080100, 0x02000100, 0x40000100, 0x00080000,
            0x00000000, 0x40080000, 0x02080100, 0x40000100
        };

        static uint[] SB6 = new uint[64]
        {
            0x20000010, 0x20400000, 0x00004000, 0x20404010,
            0x20400000, 0x00000010, 0x20404010, 0x00400000,
            0x20004000, 0x00404010, 0x00400000, 0x20000010,
            0x00400010, 0x20004000, 0x20000000, 0x00004010,
            0x00000000, 0x00400010, 0x20004010, 0x00004000,
            0x00404000, 0x20004010, 0x00000010, 0x20400010,
            0x20400010, 0x00000000, 0x00404010, 0x20404000,
            0x00004010, 0x00404000, 0x20404000, 0x20000000,
            0x20004000, 0x00000010, 0x20400010, 0x00404000,
            0x20404010, 0x00400000, 0x00004010, 0x20000010,
            0x00400000, 0x20004000, 0x20000000, 0x00004010,
            0x20000010, 0x20404010, 0x00404000, 0x20400000,
            0x00404010, 0x20404000, 0x00000000, 0x20400010,
            0x00000010, 0x00004000, 0x20400000, 0x00404010,
            0x00004000, 0x00400010, 0x20004010, 0x00000000,
            0x20404000, 0x20000000, 0x00400010, 0x20004010
        };

        static uint[] SB7 = new uint[64]
        {
            0x00200000, 0x04200002, 0x04000802, 0x00000000,
            0x00000800, 0x04000802, 0x00200802, 0x04200800,
            0x04200802, 0x00200000, 0x00000000, 0x04000002,
            0x00000002, 0x04000000, 0x04200002, 0x00000802,
            0x04000800, 0x00200802, 0x00200002, 0x04000800,
            0x04000002, 0x04200000, 0x04200800, 0x00200002,
            0x04200000, 0x00000800, 0x00000802, 0x04200802,
            0x00200800, 0x00000002, 0x04000000, 0x00200800,
            0x04000000, 0x00200800, 0x00200000, 0x04000802,
            0x04000802, 0x04200002, 0x04200002, 0x00000002,
            0x00200002, 0x04000000, 0x04000800, 0x00200000,
            0x04200800, 0x00000802, 0x00200802, 0x04200800,
            0x00000802, 0x04000002, 0x04200802, 0x04200000,
            0x00200800, 0x00000000, 0x00000002, 0x04200802,
            0x00000000, 0x00200802, 0x04200000, 0x00000800,
            0x04000002, 0x04000800, 0x00000800, 0x00200002
        };

        static uint[] SB8 = new uint[64]
        {
            0x10001040, 0x00001000, 0x00040000, 0x10041040,
            0x10000000, 0x10001040, 0x00000040, 0x10000000,
            0x00040040, 0x10040000, 0x10041040, 0x00041000,
            0x10041000, 0x00041040, 0x00001000, 0x00000040,
            0x10040000, 0x10000040, 0x10001000, 0x00001040,
            0x00041000, 0x00040040, 0x10040040, 0x10041000,
            0x00001040, 0x00000000, 0x00000000, 0x10040040,
            0x10000040, 0x10001000, 0x00041040, 0x00040000,
            0x00041040, 0x00040000, 0x10041000, 0x00001000,
            0x00000040, 0x10040040, 0x00001000, 0x00041040,
            0x10001000, 0x00000040, 0x10000040, 0x10040000,
            0x10040040, 0x10000000, 0x00040000, 0x10001040,
            0x00000000, 0x10041040, 0x00040040, 0x10000040,
            0x10040000, 0x10001000, 0x10001040, 0x00000000,
            0x10041040, 0x00041000, 0x00041000, 0x00001040,
            0x00001040, 0x00040040, 0x10000000, 0x10041000
        };

        public int des3_set_3keys(des3_context ctx, byte[] key1, byte[] key2, byte[] key3)
        {
            int i;

            des_main_ks(ctx.esk, key1);
            des_main_ks(ctx.dsk, key2, 32);
            des_main_ks(ctx.esk, key3, 64);

            for (i = 0; i < 32; i += 2)
            {
                ctx.dsk[i] = ctx.esk[94 - i];
                ctx.dsk[i + 1] = ctx.esk[95 - i];

                ctx.esk[i + 32] = ctx.dsk[62 - i];
                ctx.esk[i + 33] = ctx.dsk[63 - i];

                ctx.dsk[i + 64] = ctx.esk[30 - i];
                ctx.dsk[i + 65] = ctx.esk[31 - i];
            }

            return (0);
        }

        public void des3_encrypt(ref des3_context ctx, byte[] input, ref byte[] output)
        {

            byte[] inputNew;
            if (input.Length < 8)
            {
                inputNew = new byte[8];
                if (input.Length > 0)
                {
                    for (int s = 0; s <= input.Length - 1; s++)
                    {
                        if (s < 8)
                        {
                            inputNew[s] = input[s];

                        }

                    }

                }

            }
            else
            {
                inputNew = input.Take(8).ToArray();
            }


            byte[] outputNew;
            if (output.Length < 8)
            {
                outputNew = new byte[8];
                if (output.Length > 0)
                {
                    for (int s = 0; s <= output.Length - 1; s++)
                    {
                        if (s < 8)
                        {
                            outputNew[s] = output[s];

                        }

                    }

                }

            }
            else
            {
                outputNew = output.Take(8).ToArray();
            }

            des3_crypt(ctx.esk, inputNew, ref outputNew);
            output = outputNew;

        }

        public void des3_decrypt(ref des3_context ctx, byte[] input, ref byte[] output)
        {
            byte[] inputNew;
            if (input.Length < 8)
            {
                inputNew = new byte[8];
                if (input.Length > 0)
                {
                    for (int s = 0; s <= input.Length - 1; s++)
                    {
                        if (s < 8)
                        {
                            inputNew[s] = input[s];

                        }

                    }

                }

            }
            else
            {
                inputNew = input.Take(8).ToArray();
            }


            byte[] outputNew;
            if (output.Length < 8)
            {
                outputNew = new byte[8];
                if (output.Length > 0)
                {
                    for (int s = 0; s <= output.Length - 1; s++)
                    {
                        if (s < 8)
                        {
                            outputNew[s] = output[s];

                        }

                    }

                }

            }
            else
            {
                outputNew = output.Take(8).ToArray();
            }

            des3_crypt(ctx.dsk, inputNew, ref outputNew);
            output = outputNew;
        }

        int des_main_ks(uint[] SK, byte[] key, int startIndex = 0)
        {
            int i;
            uint X = 0, Y = 0, T;

            GET_UINT32(ref X, key, 0);
            GET_UINT32(ref Y, key, 4);

            /* Permuted Choice 1 */

            T = ((Y >> 4) ^ X) & 0x0F0F0F0F; X ^= T; Y ^= (T << 4);
            T = ((Y) ^ X) & 0x10101010; X ^= T; Y ^= (T);

            X = (LHs[(X) & 0xF] << 3) | (LHs[(X >> 8) & 0xF] << 2)
                | (LHs[(X >> 16) & 0xF] << 1) | (LHs[(X >> 24) & 0xF])
                | (LHs[(X >> 5) & 0xF] << 7) | (LHs[(X >> 13) & 0xF] << 6)
                | (LHs[(X >> 21) & 0xF] << 5) | (LHs[(X >> 29) & 0xF] << 4);

            Y = (RHs[(Y >> 1) & 0xF] << 3) | (RHs[(Y >> 9) & 0xF] << 2)
                | (RHs[(Y >> 17) & 0xF] << 1) | (RHs[(Y >> 25) & 0xF])
                | (RHs[(Y >> 4) & 0xF] << 7) | (RHs[(Y >> 12) & 0xF] << 6)
                | (RHs[(Y >> 20) & 0xF] << 5) | (RHs[(Y >> 28) & 0xF] << 4);

            X &= 0x0FFFFFFF;
            Y &= 0x0FFFFFFF;

            /* calculate subkeys */

            for (i = 0; i < 16; i++)
            {
                if (i < 2 || i == 8 || i == 15)
                {
                    X = ((X << 1) | (X >> 27)) & 0x0FFFFFFF;
                    Y = ((Y << 1) | (Y >> 27)) & 0x0FFFFFFF;
                }
                else
                {
                    X = ((X << 2) | (X >> 26)) & 0x0FFFFFFF;
                    Y = ((Y << 2) | (Y >> 26)) & 0x0FFFFFFF;
                }

                SK[startIndex++] = ((X << 4) & 0x24000000) | ((X << 28) & 0x10000000)
                        | ((X << 14) & 0x08000000) | ((X << 18) & 0x02080000)
                        | ((X << 6) & 0x01000000) | ((X << 9) & 0x00200000)
                        | ((X >> 1) & 0x00100000) | ((X << 10) & 0x00040000)
                        | ((X << 2) & 0x00020000) | ((X >> 10) & 0x00010000)
                        | ((Y >> 13) & 0x00002000) | ((Y >> 4) & 0x00001000)
                        | ((Y << 6) & 0x00000800) | ((Y >> 1) & 0x00000400)
                        | ((Y >> 14) & 0x00000200) | ((Y) & 0x00000100)
                        | ((Y >> 5) & 0x00000020) | ((Y >> 10) & 0x00000010)
                        | ((Y >> 3) & 0x00000008) | ((Y >> 18) & 0x00000004)
                        | ((Y >> 26) & 0x00000002) | ((Y >> 24) & 0x00000001);
                //     startIndex++;
                SK[startIndex++] = ((X << 15) & 0x20000000) | ((X << 17) & 0x10000000)
                        | ((X << 10) & 0x08000000) | ((X << 22) & 0x04000000)
                        | ((X >> 2) & 0x02000000) | ((X << 1) & 0x01000000)
                        | ((X << 16) & 0x00200000) | ((X << 11) & 0x00100000)
                        | ((X << 3) & 0x00080000) | ((X >> 6) & 0x00040000)
                        | ((X << 15) & 0x00020000) | ((X >> 4) & 0x00010000)
                        | ((Y >> 2) & 0x00002000) | ((Y << 8) & 0x00001000)
                        | ((Y >> 14) & 0x00000808) | ((Y >> 9) & 0x00000400)
                        | ((Y) & 0x00000200) | ((Y << 7) & 0x00000100)
                        | ((Y >> 7) & 0x00000020) | ((Y >> 3) & 0x00000011)
                        | ((Y << 2) & 0x00000004) | ((Y >> 21) & 0x00000002);
                // startIndex++;
            }

            return (0);
        }

        void GET_UINT32(ref uint n, byte[] b, int i)
        {
            (n) = ((uint)(b)[(i)] << 24) | ((uint)(b)[(i) + 1] << 16) | ((uint)(b)[(i) + 2] << 8) | ((uint)(b)[(i) + 3]);
        }

        static uint[] LHs = new uint[16]
        {
            0x00000000, 0x00000001, 0x00000100, 0x00000101,
            0x00010000, 0x00010001, 0x00010100, 0x00010101,
            0x01000000, 0x01000001, 0x01000100, 0x01000101,
            0x01010000, 0x01010001, 0x01010100, 0x01010101
        };


        static uint[] RHs = new uint[16]
        {
            0x00000000, 0x01000000, 0x00010000, 0x01010000,
            0x00000100, 0x01000100, 0x00010100, 0x01010100,
            0x00000001, 0x01000001, 0x00010001, 0x01010001,
            0x00000101, 0x01000101, 0x00010101, 0x01010101,
        };

        void DES_IP(ref uint X, ref uint Y)
        {
            uint T;
            T = ((X >> 4) ^ Y) & 0x0F0F0F0F; Y ^= T; X ^= (T << 4);
            T = ((X >> 16) ^ Y) & 0x0000FFFF; Y ^= T; X ^= (T << 16);
            T = ((Y >> 2) ^ X) & 0x33333333; X ^= T; Y ^= (T << 2);
            T = ((Y >> 8) ^ X) & 0x00FF00FF; X ^= T; Y ^= (T << 8);
            Y = ((Y << 1) | (Y >> 31)) & 0xFFFFFFFF;
            T = (X ^ Y) & 0xAAAAAAAA; Y ^= T; X ^= T;
            X = ((X << 1) | (X >> 31)) & 0xFFFFFFFF;
        }


        void DES_ROUND(uint[] SK, ref uint X, ref uint Y)
        {
            uint T;

            T = SK[x++] ^ X;
            Y ^= SB8[(T) & 0x3F] ^
            SB6[(T >> 8) & 0x3F] ^
            SB4[(T >> 16) & 0x3F] ^
            SB2[(T >> 24) & 0x3F];

            T = SK[x++] ^ ((X << 28) | (X >> 4));
            Y ^= SB7[(T) & 0x3F] ^
            SB5[(T >> 8) & 0x3F] ^
            SB3[(T >> 16) & 0x3F] ^
            SB1[(T >> 24) & 0x3F];
        }

        public void des3_crypt(uint[] SK, byte[] input, ref byte[] output)
        {
            uint X = 0, Y = 0, T = 0;

            GET_UINT32(ref X, input, 0);
            GET_UINT32(ref Y, input, 4);

            DES_IP(ref X, ref Y);

            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);

            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);
            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);
            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);
            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);
            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);
            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);
            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);
            DES_ROUND(SK, ref X, ref Y); DES_ROUND(SK, ref Y, ref X);

            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);
            DES_ROUND(SK, ref Y, ref X); DES_ROUND(SK, ref X, ref Y);

            x = 0;
            DES_FP(ref Y, ref X);

            PUT_UINT32(Y, output, 0);
            PUT_UINT32(X, output, 4);

        }

        void DES_FP(ref uint X, ref uint Y)
        {
            uint T;
            X = ((X << 31) | (X >> 1)) & 0xFFFFFFFF;
            T = (X ^ Y) & 0xAAAAAAAA; X ^= T; Y ^= T;
            Y = ((Y << 31) | (Y >> 1)) & 0xFFFFFFFF;
            T = ((Y >> 8) ^ X) & 0x00FF00FF; X ^= T; Y ^= (T << 8);
            T = ((Y >> 2) ^ X) & 0x33333333; X ^= T; Y ^= (T << 2);
            T = ((X >> 16) ^ Y) & 0x0000FFFF; Y ^= T; X ^= (T << 16);
            T = ((X >> 4) ^ Y) & 0x0F0F0F0F; Y ^= T; X ^= (T << 4);
        }

        void PUT_UINT32(uint n, byte[] b, int i)
        {
            (b)[(i)] = (byte)((n) >> 24);
            (b)[(i) + 1] = (byte)((n) >> 16);
            (b)[(i) + 2] = (byte)((n) >> 8);
            (b)[(i) + 3] = (byte)((n));
        }


    }

    public static class _AES
    {
        private static string GetKey()
        {
            string MyKey = "";
            try
            {
                //best to have 32bytes key
                //1234567890 1234567890 1234567890 1234567890
                //AabtrTUJgi 1463618309 wxZyDTwfti35g
                MyKey = "AabtrTUJgi1463618309wxZyDTwfti35g";
            }
            catch (Exception ex)
            {
                string sMessage = ex.Message;
                Log.WriteLogFile("Function: setKey", Enums.LogType.Error);
                Log.WriteLogFile("Message : " + ex.Message, Enums.LogType.Error);
                Log.WriteLogFile("StackTrace : " + ex.StackTrace, Enums.LogType.Error);
            }
            return MyKey;
        }


        public static string HashSHA512(this string value)
        {
            using (var sha = SHA512.Create())
            {
                return Convert.ToBase64String(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value)));
            }
        }

        //private static byte[] CreateKey(string password, int keyBytes = 32)
        private static byte[] CreateKey()
        {
            string password = GetKey();
            int keyBytes = 32;

            //this salt value can be changed in any random numbers as seed into new KEY
            byte[] salt = new byte[] { 70, 80, 70, 40, 20, 80, 30, 30 };
            int iterations = 300;
            var keyGenerator = new Rfc2898DeriveBytes(password, salt, iterations);
            return keyGenerator.GetBytes(keyBytes);
        }

        //public static string Encrypt(this string clearValue, string encryptionKey)
        public static string Encrypt(this string clearValue)
        {
            using (Aes aes = Aes.Create())
            {
                //aes.Key = CreateKey(encryptionKey);
                aes.Key = CreateKey();

                byte[] encrypted = AesEncryptStringToBytes(clearValue, aes.Key, aes.IV);
                return Convert.ToBase64String(encrypted) + ";" + Convert.ToBase64String(aes.IV);
            }
        }

        private static byte[] AesEncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException($"{nameof(plainText)}");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException($"{nameof(key)}");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException($"{nameof(iv)}");

            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                    {
                        streamWriter.Write(plainText);
                    }
                    encrypted = memoryStream.ToArray();
                }
            }
            return encrypted;
        }

        //public static string Decrypt(this string encryptedValue, string encryptionKey)
        public static string Decrypt(this string encryptedValue)
        {
            string encryptionKey = GetKey();
            string iv = encryptedValue.Substring(encryptedValue.IndexOf(';') + 1, encryptedValue.Length - encryptedValue.IndexOf(';') - 1);
            encryptedValue = encryptedValue.Substring(0, encryptedValue.IndexOf(';'));

            //return AesDecryptStringFromBytes(Convert.FromBase64String(encryptedValue), CreateKey(encryptionKey), Convert.FromBase64String(iv));
            return AesDecryptStringFromBytes(Convert.FromBase64String(encryptedValue), CreateKey(), Convert.FromBase64String(iv));
        }

        private static string AesDecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException($"{nameof(cipherText)}");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException($"{nameof(key)}");
            if (iv == null || iv.Length <= 0)
                throw new ArgumentNullException($"{nameof(iv)}");

            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                    plaintext = streamReader.ReadToEnd();
            }
            return plaintext;
        }
    }
}