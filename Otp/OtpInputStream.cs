/*``The contents of this file are subject to the Erlang Public License,
* Version 1.1, (the "License"); you may not use this file except in
* compliance with the License. You should have received a copy of the
* Erlang Public License along with this software. If not, it can be
* retrieved via the world wide web at http://www.erlang.org/.
* 
* Software distributed under the License is distributed on an "AS IS"
* basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
* the License for the specific language governing rights and limitations
* under the License.
* 
* The Initial Developer of the Original Code is Ericsson Utvecklings AB.
* Portions created by Ericsson are Copyright 1999, Ericsson Utvecklings
* AB. All Rights Reserved.''
* 
 * Converted from Java to C# by Vlad Dumitrescu (vlad_Dumitrescu@hotmail.com)
*/
namespace Otp
{
    using System;

    /*
    * Provides a stream for decoding Erlang terms from external format.
    *
    * <p> Note that this class is not synchronized, if you need
    * synchronization you must provide it yourself.
    **/
    public class OtpInputStream : System.IO.MemoryStream
    {
        byte[] m_buf2;
        byte[] m_buf4;
        byte[] m_buf8;
        private int m_Origin;

        /*
        * Create a stream from a buffer containing encoded Erlang terms.
        **/
        public OtpInputStream(byte[] buf)
            : this(buf, 0, buf.Length)
        {
        }

        /*
        * Create a stream from a buffer containing encoded
        * Erlang terms at the given offset and length.
        **/
        public OtpInputStream(byte[] buf, int offset, int length)
            : base(buf, offset, length, false, true)
        {
            m_buf2 = new byte[2];
            m_buf4 = new byte[4];
            m_buf8 = new byte[8];
            m_Origin = offset;
        }

        public int BufferPosition
        {
            get { return m_Origin + (int)Position; }
        }

        ///*
        //* Get the current position in the stream.
        //*
        //* @return the current position in the stream.
        //**/
        //public int getPos()
        //{
        //    return (int) base.Position;
        //}
        ///*
        //* Set the current position in the stream.
        //*
        //* @param pos the position to move to in the stream. If pos
        //* indicates a position beyond the end of the stream, the position
        //* is move to the end of the stream instead. If pos is negative, the
        //* position is moved to the beginning of the stream instead.
        //*
        //* @return the previous position in the stream.
        //**/
        //public int setPos(int pos)
        //{
        //    int oldpos = (int) base.Position;
        //    if (pos > (int) base.Length)
        //        pos = (int) base.Length;
        //    else if (pos < 0)
        //        pos = 0;

        //    base.Position = (System.Int64) pos;

        //    return oldpos;
        //}

        /*
        * Read an array of bytes from the stream. The method reads at most
        * buf.length bytes from the input stream.
        *
        * @return the number of bytes read.
        *
        * @exception OtpErlangDecodeException if the next byte cannot be
        * read.
        **/
        public int readN(byte[] buf)
        {
            try
            {
                return base.Read(buf, 0, buf.Length);
            }
            catch (System.IO.IOException)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }
        }

        /*
        * Look ahead one position in the stream without consuming the byte
        * found there.
        *
        * @return the next byte in the stream, as an integer.
        *
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public int peek()
        {
            int i;
            try
            {
                i = base.ReadByte();
                base.Seek(-1, System.IO.SeekOrigin.Current);
                if (i < 0)
                    i += 256;

                return i;
            }
            catch (System.Exception)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }
        }

        /*
        * Read a one byte integer from the stream.
        *
        * @return the byte read, as an integer.
        * 
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public int read1()
        {
            int i;
            i = base.ReadByte();

            if (i < 0)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }

            return i;
        }

        /*
        * Read a two byte big endian integer from the stream.
        *
        * @return the bytes read, converted from big endian to an integer.
        * 
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public int read2BE()
        {
            try
            {
                base.Read(m_buf2, 0, m_buf2.Length);
            }
            catch (System.IO.IOException)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }
            return ((((int)m_buf2[0] << 8) & 0xff00) + (((int)m_buf2[1]) & 0xff));
        }

        /*
        * Read a four byte big endian integer from the stream.
        *
        * @return the bytes read, converted from big endian to an integer.
        * 
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public int read4BE()
        {
            try
            {
                base.Read(m_buf4, 0, m_buf4.Length);
            }
            catch (System.IO.IOException)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }
            return read4BE(m_buf4);
        }

        public static int read4BE(byte[] b)
        {
            System.Diagnostics.Debug.Assert(b.Length == 4);
            return (int)((((int)b[0] << 24) & 0xff000000) + (((int)b[1] << 16) & 0xff0000) + (((int)b[2] << 8) & 0xff00) + (((int)b[3]) & 0xff));
        }

        /*
        * Read an eight byte big endian integer from the stream.
        *
        * @return the bytes read, converted from big endian to an integer.
        * 
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public System.UInt64 read8BE()
        {
            try
            {
                base.Read(m_buf8, 0, m_buf8.Length);
            }
            catch (System.IO.IOException)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }
            System.UInt64 i1 = (System.UInt64)((((int)m_buf8[0] << 24) & 0xff000000)
                             + (((int)m_buf8[1] << 16) & 0xff0000)
                             + (((int)m_buf8[2] << 8) & 0xff00)
                             + (((int)m_buf8[3]) & 0xff));
            System.UInt64 i2 = (i1 << 32) & 0xffffffff00000000
                             + (System.UInt64)((((int)m_buf8[4] << 24) & 0xff000000)
                             + (((int)m_buf8[5] << 16) & 0xff0000)
                             + (((int)m_buf8[6] << 8) & 0xff00)
                             + (((int)m_buf8[7]) & 0xff));
            return i2;
        }

        /*
        * Read a two byte little endian integer from the stream.
        *
        * @return the bytes read, converted from little endian to an
        * integer.
        * 
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public int read2LE()
        {
            try
            {
                base.Read(m_buf2, 0, m_buf2.Length);
            }
            catch (System.IO.IOException)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }
            return ((((int)m_buf2[1] << 8) & 0xff00) + (((int)m_buf2[0]) & 0xff));
        }

        /*
        * Read a four byte little endian integer from the stream.
        *
        * @return the bytes read, converted from little endian to an
        * integer.
        * 
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public int read4LE()
        {
            try
            {
                base.Read(m_buf4, 0, m_buf4.Length);
            }
            catch (System.IO.IOException)
            {
                throw new Erlang.Exception("Cannot read from input stream");
            }
            return (int)((((int)m_buf4[3] << 24) & 0xff000000) + (((int)m_buf4[2] << 16) & 0xff0000)
                + (((int)m_buf4[1] << 8) & 0xff00) + (((int)m_buf4[0]) & 0xff));
        }

        /*
        * Read an Erlang atom from the stream and interpret the value as a
        * boolean.
        *
        * @return true if the atom at the current position in the stream
        * contains the value 'true' (ignoring case), false otherwise.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not an atom.
        **/
        public bool read_boolean()
        {
            return System.Boolean.Parse(this.read_atom());
        }

        /*
        * Read an Erlang atom from the stream.
        *
        * @return a String containing the value of the atom.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not an atom.
        **/
        public string read_atom()
        {
            int tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            if (tag != OtpExternal.atomTag)
            {
                throw new Erlang.Exception("wrong tag encountered, expected " + OtpExternal.atomTag + ", got " + tag);
            }

            int len = this.read2BE();
            int n = len > OtpExternal.maxAtomLength ? OtpExternal.maxAtomLength : len;
            string s = System.Text.Encoding.ASCII.GetString(base.GetBuffer(), BufferPosition, len);
            base.Position += len;
            if (n != len)
                s = s.Substring(0, n);
            return s;
        }

        /*
        * Read an Erlang binary from the stream.
        *
        * @return a byte array containing the value of the binary.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not a binary.
        **/
        public byte[] read_binary()
        {
            int tag;
            int len;
            byte[] bin;

            tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            if (tag != OtpExternal.binTag)
            {
                throw new Erlang.Exception("Wrong tag encountered, expected " + OtpExternal.binTag + ", got " + tag);
            }

            len = this.read4BE();

            bin = new byte[len];
            this.readN(bin);

            return bin;
        }

        /*
        * Read an Erlang float from the stream.
        *
        * @return the float value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not a float.
        **/
        public float read_float()
        {
            double d = this.read_double();
            float f = (float)d;
            if (System.Math.Abs(d - f) >= 1.0E-20)
                throw new Erlang.Exception("Value cannot be represented as float: " + d);
            return f;
        }

        /*
        * Read an Erlang float from the stream.
        *
        * @return the float value, as a double.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not a float.
        *
        **/
        public double read_double()
        {
            return getFloatOrDouble();
        }

        private double getFloatOrDouble()
        {

            // parse the stream
            int tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            byte[] strbuf;
            double parsedValue = 0.0;

            if (tag == OtpExternal.floatTag)
            {
                // get the string
                strbuf = new byte[31];
                this.readN(strbuf);

                char[] tmpChar = new char[strbuf.Length];
                strbuf.CopyTo(tmpChar, 0);
                System.String str = new System.String(tmpChar);
                //System.Diagnostics.Debug.WriteLine("getFloatOrDouble: str = " + str);

                try
                {
                    // Easier than the java version.
                    parsedValue = System.Double.Parse(str);
                    return parsedValue;
                }
                catch
                {
                    throw new Erlang.Exception("Error parsing float format: '" + str + "'");
                }
            }
            else if (tag == OtpExternal.newFloatTag)
            {
                this.readN(m_buf8);
                // IEEE 754 decoder
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(m_buf8);
                }
                return BitConverter.ToDouble(m_buf8, 0);
            }
            else
            {
                throw new Erlang.Exception("Wrong tag encountered, expected " + OtpExternal.floatTag + ", got " + tag);
            }
        }

        /*
        * Read one byte from the stream.
        *
        * @return the byte read.
        * 
        * @exception Erlang.DecodeException if the next byte cannot be
        * read.
        **/
        public byte read_byte()
        {
            long l = this.read_long();
            byte i = (byte)l;

            if (l != i)
            {
                throw new Erlang.Exception("Value too large for byte: " + l);
            }

            return i;
        }

        /*
        * Read a character from the stream.
        *
        * @return the character value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not an integer that can be represented as a char.
        **/
        public char read_char()
        {
            long l = this.read_long();
            char i = (char)l;

            if (l != i)
            {
                throw new Erlang.Exception("Value too large for byte: " + l);
            }

            return i;
        }

        /*
        * Read an unsigned integer from the stream.
        *
        * @return the integer value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream can not be represented as a positive integer.
        **/
        public int read_uint()
        {
            long l = this.read_long();
            int i = (int)l;

            if (l != i)
            {
                throw new Erlang.Exception("Value too large for integer: " + l);
            }
            else if (l < 0)
            {
                throw new Erlang.Exception("Value not unsigned: " + l);
            }

            return i;
        }

        /*
        * Read an integer from the stream.
        *
        * @return the integer value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream can not be represented as an integer.
        **/
        public int read_int()
        {
            long l = this.read_long();
            int i = (int)l;

            if (l != i)
            {
                throw new Erlang.Exception("Value too large for byte: " + l);
            }

            return i;
        }

        /*
        * Read an unsigned short from the stream.
        *
        * @return the short value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream can not be represented as a positive short.
        **/
        public short read_ushort()
        {
            long l = this.read_long();
            short i = (short)l;

            if (l != i)
            {
                throw new Erlang.Exception("Value too large for byte: " + l);
            }
            else if (l < 0)
            {
                throw new Erlang.Exception("Value not unsigned: " + l);
            }

            return i;
        }

        /*
        * Read a short from the stream.
        *
        * @return the short value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream can not be represented as a short.
        **/
        public short read_short()
        {
            long l = this.read_long();
            short i = (short)l;

            if (l != i)
            {
                throw new Erlang.Exception("Value too large for byte: " + l);
            }

            return i;
        }

        /*
        * Read an unsigned long from the stream.
        *
        * @return the long value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream can not be represented as a positive long.
        **/
        public ulong read_ulong()
        {
            return (ulong)read_long(false);
        }

        /*
        * Read a long from the stream.
        *
        * @return the long value.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream can not be represented as a long.
        **/
        public long read_long() { return read_long(true); }

        private long read_long(bool signed)
        {
            int tag;
            int sign;
            int arity;
            long val;

            tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            switch (tag)
            {
                case OtpExternal.smallIntTag:
                    val = this.read1();
                    break;
                case OtpExternal.intTag:
                    val = this.read4BE();
                    break;
                case OtpExternal.smallBigTag:
                    {
                        arity = this.read1();
                        sign = this.read1();

                        byte[] nb = new byte[arity];
                        if (arity != this.readN(nb))
                        {
                            throw new Erlang.Exception("Cannot read from input stream. Expected smallBigTag arity " + arity);
                        }
                        if (arity > 8)
                            throw new Erlang.Exception("Value too large for long type (arity=" + arity + ")");

                        val = 0;
                        for (int i = 0; i < arity; i++)
                        {
                            val |= (long)nb[i] << (i * 8);
                        }

                        val = (sign == 0 ? val : -val); // should deal with overflow

                        if (sign == 1 && !signed)
                            throw new Erlang.Exception("Requested unsigned, but read signed long value: " + val.ToString());

                        break;
                    }
                case OtpExternal.largeBigTag:
                default:
                    throw new Erlang.Exception("Not valid integer tag: " + tag);
            }

            return val;
        }

        /*
        * Read a list header from the stream.
        *
        * @return the arity of the list.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not a list.
        **/
        public int read_list_head()
        {
            int arity = 0;
            int tag = this.read1();

            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            switch (tag)
            {
                case OtpExternal.nilTag:
                    arity = 0;
                    break;
                case OtpExternal.stringTag:
                    arity = this.read2BE();
                    break;
                case OtpExternal.listTag:
                    arity = this.read4BE();
                    break;
                default:
                    throw new Erlang.Exception("Not valid list tag: " + tag);
            }

            return arity;
        }

        /*
        * Read a tuple header from the stream.
        *
        * @return the arity of the tuple.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not a tuple.
        **/
        public int read_tuple_head()
        {
            int arity = 0;
            int tag = this.read1();

            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            // decode the tuple header and get arity
            switch (tag)
            {
                case OtpExternal.smallTupleTag:
                    arity = this.read1();
                    break;
                case OtpExternal.largeTupleTag:
                    arity = this.read4BE();
                    break;
                default:
                    throw new Erlang.Exception("Not valid tuple tag: " + tag);

            }

            return arity;
        }

        /*
        * Read an empty list from the stream.
        *
        * @return zero (the arity of the list).
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not an empty list.
        **/
        public int read_nil()
        {
            int arity = 0;
            int tag = this.read1();

            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            switch (tag)
            {
                case OtpExternal.nilTag:
                    arity = 0;
                    break;
                default:
                    throw new Erlang.Exception("Not valid nil tag: " + tag);
            }

            return arity;
        }

        /*
        * Read an Erlang PID from the stream.
        *
        * @return the value of the PID.
        * 
        * @exception Erlang.DecodeException if the next term in the
        * stream is not an Erlang PID.
        **/
        public Erlang.Pid read_pid()
        {
            System.String node;
            int id;
            int serial;
            int creation;
            int tag;

            tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            if (tag != OtpExternal.pidTag)
            {
                throw new Erlang.Exception("Wrong tag encountered, expected " + OtpExternal.pidTag + ", got " + tag);
            }

            node = this.read_atom();
            id = this.read4BE() & 0x7fff; // 15 bits
            serial = this.read4BE() & 0x07; // 3 bits
            creation = this.read1() & 0x03; // 2 bits

            return new Erlang.Pid(node, id, serial, creation);
        }

        /*
        * Read an Erlang port from the stream.
        *
        * @return the value of the port.
        * 
        * @exception DecodeException if the next term in the
        * stream is not an Erlang port.
        **/
        public Erlang.Port read_port()
        {
            System.String node;
            int id;
            int creation;
            int tag;

            tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            if (tag != OtpExternal.portTag)
            {
                throw new Erlang.Exception("Wrong tag encountered, expected " + OtpExternal.portTag + ", got " + tag);
            }

            node = this.read_atom();
            id = this.read4BE() & 0x3ffff; // 18 bits
            creation = this.read1() & 0x03; // 2 bits

            return new Erlang.Port(node, id, creation);
        }

        /*
        * Read an Erlang reference from the stream.
        *
        * @return the value of the reference
        * 
        * @exception DecodeException if the next term in the
        * stream is not an Erlang reference.
        **/
        public Erlang.Ref read_ref()
        {
            System.String node;
            int id;
            int creation;
            int tag;

            tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            switch (tag)
            {
                case OtpExternal.refTag:
                    node = this.read_atom();
                    id = this.read4BE() & 0x3ffff; // 18 bits
                    creation = this.read1() & 0x03; // 2 bits
                    return new Erlang.Ref(node, id, creation);



                case OtpExternal.newRefTag:
                    int arity = this.read2BE();
                    node = this.read_atom();
                    creation = this.read1() & 0x03; // 2 bits

                    int[] ids = new int[arity];
                    for (int i = 0; i < arity; i++)
                    {
                        ids[i] = this.read4BE();
                    }
                    ids[0] &= 0x3ffff; // first id gets truncated to 18 bits
                    return new Erlang.Ref(node, ids, creation);



                default:
                    throw new Erlang.Exception("Wrong tag encountered, expected ref, got " + tag);

            }
        }

        /*
        * Read a string from the stream.
        *
        * @return the value of the string.
        * 
        * @exception DecodeException if the next term in the
        * stream is not a string.
        **/
        public System.String read_string()
        {
            int tag;
            int len;
            char[] charbuf;

            tag = this.read1();
            if (tag == OtpExternal.versionTag)
            {
                tag = this.read1();
            }

            switch (tag)
            {

                case OtpExternal.stringTag:
                    len = this.read2BE();
                    string s = System.Text.Encoding.ASCII.GetString(base.GetBuffer(), BufferPosition, len);
                    base.Position += len;
                    return s;

                case OtpExternal.nilTag:
                    return "";

                case OtpExternal.listTag:
                    // List when unicode +
                    len = this.read4BE();
                    charbuf = new char[len];

                    for (int i = 0; i < len; i++)
                        charbuf[i] = this.read_char();

                    this.read_nil();
                    return new string(charbuf);

                default:
                    throw new Erlang.Exception("Wrong tag encountered, expected " + OtpExternal.stringTag + " or " + OtpExternal.listTag + ", got " + tag);

            }
        }

        /*
        * Read an arbitrary Erlang term from the stream.
        *
        * @return the Erlang term.
        *
        * @exception DecodeException if the stream does not
        * contain a known Erlang type at the next position.
        **/
        public Erlang.Object read_any()
        {
            // calls one of the above functions, depending on o
            int tag = this.peek();
            if (tag == OtpExternal.versionTag)
            {
                this.read1();
                tag = this.peek();
            }

            //System.Diagnostics.Debug.WriteLine("read_any: tag = " + tag);

            switch (tag)
            {
                case OtpExternal.smallIntTag:
                case OtpExternal.intTag:
                case OtpExternal.smallBigTag:
                    return new Erlang.Long(this);

                case OtpExternal.atomTag:
                    string s = read_atom();
                    if (s == "true")
                        return new Erlang.Boolean(true);
                    else if (s == "false")
                        return new Erlang.Boolean(false);
                    else
                        return new Erlang.Atom(s);

                case OtpExternal.floatTag:
                case OtpExternal.newFloatTag:
                    return new Erlang.Double(this);

                case OtpExternal.refTag:
                case OtpExternal.newRefTag:
                    return new Erlang.Ref(this);

                case OtpExternal.portTag:
                    return new Erlang.Port(this);

                case OtpExternal.pidTag:
                    return new Erlang.Pid(this);

                case OtpExternal.stringTag:
                    return new Erlang.String(this);

                case OtpExternal.listTag:
                case OtpExternal.nilTag:
                    return new Erlang.List(this);

                case OtpExternal.smallTupleTag:
                case OtpExternal.largeTupleTag:
                    return new Erlang.Tuple(this);

                case OtpExternal.binTag:
                    return new Erlang.Binary(this);

                case OtpExternal.largeBigTag:
                default:
                    throw new Erlang.Exception("Uknown data type: " + tag);

            }
        }
    }
}