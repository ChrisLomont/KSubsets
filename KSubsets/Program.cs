// Chris Lomont Aug 2024
// choose all size k subsets of n >= k items

using System.Diagnostics;

// test all small values
for (int n = 0; n <= 20; ++n)
for (int k = 0; k <= n; ++k)
{
    Trace.Assert(Binomial(n, k) == CountKSubsets(n, k));
}

// check all breaks at 1 bit shorter than int size
Console.WriteLine($"(62,3): {Binomial(62, 3)} = {CountKSubsets(62, 3)}");
Console.WriteLine($"(63,2): {Binomial(63, 2)} != {CountKSubsets(63,2)}");


Console.WriteLine("Done");

// given integers 0<=k<=n<=62, count how many integers
// 0 <= x < 2^n have exactly k bits set
int CountKSubsets(int n, int k)
{
    Trace.Assert(n >= k);
    Trace.Assert(k >= 0);
    if (k == 0) return 1;
    //Trace.Assert(64 - 2 >= n); // max size 2 bits workspace
    // iterate over all size k subsets of n items, checking things, counting
    int count = 0;
    Int64 x = (1L << k) - 1; // smallest one
    do
    {
        ++count;
        // use x here to make k-subset as needed
        // ... 

        Trace.Assert(Bitcount(x)==k);
        var y = Next(x);
        Trace.Assert(y > x);
        var z = Prev(y);
        Trace.Assert(z == x);
        x = y;
    } while (x < (1L << n));

    return count;
}
// return  number of bits set in x
int Bitcount(Int64 x)
{
    int count = 0;
    while (x > 0)
    {
        count += (int)(x & 1);
        x >>= 1;
    }
    return count;
}

// compute binomial coefficient n Choose k 
int Binomial(int n, int k)
{
    var p = 1; // falling powers trick
    for (var i = 1; i <= k; ++i)
    {
        p *= (n + 1 - i);
        p /= i;
    }
    return p;
}

// Gosper's Hack for enumerating size K subsets
// ints must be represented as two's complement
// return the NEXT x with the same number of bits set
Int64 Next(Int64 x)
{
    Int64 u = x & -x;
    Int64 v = x + u;
    return v + (((v ^ x) / u) >> 2);
}

// Gosper's Hack for enumerating size K subsets
// ints must be represented as two's complement
// return the PREVIOUS x with the same number of bits set
Int64 Prev(Int64 y)
{
    Int64 t = y + 1;
    Int64 u = t ^ y;
    Int64 v = t & y;
    return v - (v & -v) / (u + 1);
}