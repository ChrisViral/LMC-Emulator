namespace LMC_Virtual_Machine
{
    //Signed number (-500 to 499)
    public struct Num
    {
        #region Constants
        public const short minValue = -500;
        public const short maxValue = 499;
        #endregion

        #region Fields
        private readonly short value;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new signed number from a short value
        /// </summary>
        /// <param name="value">Short value</param>
        private Num(short value)
        {
            while (value > maxValue)
            {
                value -= 1000;
            }

            while (value < minValue)
            {
                value += 1000;
            }

            this.value = value;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implicit int to Num cast
        /// </summary>
        public static implicit operator Num(int value)
        {
            return new Num((short)value);
        }

        /// <summary>
        /// Explicit UNum to Num cast
        /// </summary>
        public static explicit operator Num(UNum value)
        {
            return new Num((short)value);
        }

        /// <summary>
        /// Implicit Num to string cast
        /// </summary>
        public static implicit operator string(Num value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Explicit Num to short cast
        /// </summary>
        public static explicit operator short(Num value)
        {
            return value.value;
        }

        /// <summary>
        /// Signed addition
        /// </summary>
        public static Num operator +(Num n1, Num n2)
        {
            return Clamp(n1.value + n2.value);
        }

        /// <summary>
        /// Signed substraction
        /// </summary>
        public static Num operator -(Num n1, Num n2)
        {
            return Clamp(n1.value - n2.value);
        }

        /// <summary>
        /// Signed/int addition
        /// </summary>
        public static Num operator +(Num n1, int n2)
        {
            return n1.value + n2;
        }

        /// <summary>
        /// Signed/int substraction
        /// </summary>
        public static Num operator -(Num n1, int n2)
        {
            return n1.value - n2;
        }
        #endregion

        #region Overloads
        /// <summary>
        /// Returns the value's string representation
        /// </summary>
        public override string ToString()
        {
            return this.value.ToString();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Clamps the value between its min and max
        /// </summary>
        /// <param name="n">Value to clamp</param>
        private static Num Clamp(Num n)
        {
            if (n.value > maxValue)
            {
                return n - 1000;
            }
            if (n.value < minValue)
            {
                return n + 1000;
            }
            return n;
        }

        /// <summary>
        /// Tries to parse a string to a signed number and returns if it worked
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="n">Value to store the result into</param>
        public static bool TryParse(string s, ref Num n)
        {
            short result;
            bool success = short.TryParse(s, out result) && result >= minValue && result <= maxValue;
            if (success) { n = result; }
            return success;
        }
        #endregion
    }

    //Unsigned number (0 to 999)
    public struct UNum
    {
        #region Constants
        public const short minValue = 0;
        public const short maxValue = 999;
        #endregion

        #region Fields
        private readonly short value;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new unsigned number from a given value
        /// </summary>
        /// <param name="value">Short value</param>
        private UNum(short value)
        {
            while (value > maxValue)
            {
                value -= 1000;
            }

            while (value < minValue)
            {
                value += 1000;
            }

            this.value = value;
        }
        #endregion

        #region Operators
        /// <summary>
        /// Implicit int to UNum cast
        /// </summary>
        public static implicit operator UNum(int value)
        {
            return new UNum((short)value);
        }

        /// <summary>
        /// Explicit Num to UNum cast
        /// </summary>
        public static explicit operator UNum(Num value)
        {
            return new UNum((short)value);
        }
        
        /// <summary>
        /// Explicit UNum to Instruction cast
        /// </summary>
        public static explicit operator Instruction(UNum value)
        {
            return (Instruction)value.value;
        }

        /// <summary>
        /// Implicit UNum to string cast
        /// </summary>
        public static implicit operator string(UNum value)
        {
            return value.ToString();
        }

        /// <summary>
        /// Explicit UNum to short cast
        /// </summary>
        public static explicit operator short(UNum value)
        {
            return value.value;
        }

        /// <summary>
        /// Unsigned addition
        /// </summary>
        public static UNum operator +(UNum n1, UNum n2)
        {
            return (UNum)((Num)n1 + (Num)n2);
        }

        /// <summary>
        /// Unsigned substraction
        /// </summary>
        public static UNum operator -(UNum n1, UNum n2)
        {
            return (UNum)((Num)n1 - (Num)n2);
        }

        /// <summary>
        /// Unsigned/int addition
        /// </summary>
        public static UNum operator +(UNum n1, int n2)
        {
            return n1.value + n2;
        }

        /// <summary>
        /// Unsigned/int substraction
        /// </summary>
        public static UNum operator -(UNum n1, int n2)
        {
            return n1.value - n2;
        }

        /// <summary>
        /// Unsigned incrementation
        /// </summary>
        public static UNum operator ++(UNum n)
        {
            return n.value + 1;
        }

        /// <summary>
        /// Value smaller than
        /// </summary>
        public static bool operator <(UNum n1, UNum n2)
        {
            return n1.value < n2.value;
        }

        /// <summary>
        /// Value greater than
        /// </summary>
        public static bool operator >(UNum n1, UNum n2)
        {
            return n1.value > n2.value;
        }

        /// <summary>
        /// Value smaller than or equal
        /// </summary>
        public static bool operator <=(UNum n1, UNum n2)
        {
            return n1.value < n2.value;
        }

        /// <summary>
        /// Value greater than or equald
        /// </summary>
        public static bool operator >=(UNum n1, UNum n2)
        {
            return n1.value >= n2.value;
        }

        /// <summary>
        /// Value equals
        /// </summary>
        public static bool operator ==(UNum n1, UNum n2)
        {
            return n1.value == n2.value;
        }

        /// <summary>
        /// Value not equald
        /// </summary>
        public static bool operator !=(UNum n1, UNum n2)
        {
            return n1.value != n2.value;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Returns value string representation
        /// </summary>
        public override string ToString()
        {
            return this.value.ToString();
        }

        /// <summary>
        /// Returns if the instance and given object are equal
        /// </summary>
        /// <param name="obj">Object to test</param>
        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (obj is UNum) { return Equals((UNum)obj); }
            return false;
        }

        /// <summary>
        /// Verifies if the two unsigned numbers are equal
        /// </summary>
        /// <param name="n">Number to verify</param>
        public bool Equals(UNum n)
        {
            return this.value.Equals(n.value);
        }

        /// <summary>
        /// Returns the value's hashcode
        /// </summary>
        public override int GetHashCode()
        {
            return this.value.GetHashCode();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Tries to parse the given string into an unsigned numbers and returns if it worked
        /// </summary>
        /// <param name="s">String to parse</param>
        /// <param name="n">Value to store the result into</param>
        public static bool TryParse(string s, ref UNum n)
        {
            short result;
            bool success = short.TryParse(s, out result) && result >= minValue && result <= maxValue;
            if (success) { n = result; }
            return success;
        }
        #endregion
    }
}
