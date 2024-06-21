using System;

public class JavaRandom
{
    private long seed;
    private const long multiplier = 0x5DEECE66DL;
    private const long addend = 0xBL;
    private const long mask = (1L << 48) - 1;

    public JavaRandom(long seed)
    {
        this.seed = (seed ^ multiplier) & mask;
    }

    protected int Next(int bits)
    {
        seed = (seed * multiplier + addend) & mask;
        return (int)(seed >> (48 - bits));
    }

    public int NextInt()
    {
        return Next(32);
    }

    public int NextInt(int bound)
    {
        if (bound <= 0)
            throw new ArgumentException("bound must be positive");

        if ((bound & -bound) == bound)  // i.e., bound is a power of 2
            return (int)((bound * (long)Next(31)) >> 31);

        int bits, val;
        do
        {
            bits = Next(31);
            val = bits % bound;
        } while (bits - val + (bound - 1) < 0);
        return val;
    }

    public long NextLong()
    {
        return ((long)(Next(32)) << 32) + Next(32);
    }

    public double NextDouble()
    {
        return (((long)Next(26) << 27) + Next(27)) / (double)(1L << 53);
    }

    public void NextBytes(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length;)
        {
            for (int rnd = NextInt(), n = Math.Min(bytes.Length - i, 4); n-- > 0; rnd >>= 8)
                bytes[i++] = (byte)rnd;
        }
    }
}
