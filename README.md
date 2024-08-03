# Generating all subsets of size k

Chris Lomont, Aug 2024

There's a really nice method to generate all subsets of $N$ items of size $k$ that I have had to look up or derive enough times I decided to write a post on my website so I can find it there. Along the way I'll post some other items I've found useful.

Sometimes you want to generate all subsets of $N$ things, say all subsets of the integers $\{0,1,...,N-1\}$. A nice way to do this is to use the binary representation of a counter $i$ going from $0$ through $2^N-1$, and for each value of $i$, make a subset where te $jth$ item is in the set if and only if bit $j$ is set in $i$. In code:

```
max = 1<<N  # this is power 2^N
for i = 0; i < max ; ++i
   subset = {}
   for j = 0 to N-1
      if bit j set in i
         subset = subset add j
```

This generates all $2^N$ subsets of $N$ items, where $i=0$ is the empty set, and $i=2^N-1$ is all items.

Other times you need to get all subsets with exactly $k$ elements, where $0<=k<=N$. When the value of $N$ is small enough to count from $0$ to $2^N$ quick enough, you can remember the simple method

```
max = 1<<N  # this is power 2^N
for i = 0; i < max; ++i
   c = bitcount(i)  # counts number of 1 bits set
   if c == k then 
       make subset and use it....
```

This works fine for things like $N<20$ (more or less, depending on hardware).

But, if you need to find all ways to pick size 5 subsets of 50 things, you cannot loop over $2^{50}$ items. Here is a really nice method called Gosper's Hack [1].

Given an integer $x>0$ with $k$ bits set, then the following returns the next larger integer with $k$ bits set:

```
int Next(int x)
{
    int u = x & -x;
    int v = x + u;
    return v + (((v ^ x) / u) >> 2);
}
```

Here, `&` is bitwise AND, `^` if bitwise XOR, integers must be stored as 2's complement, integer division truncates, and `>>` is shift right. I'll leave it to you to figure out how this works (it's pretty straightforward, and interesting to check).

The integer must have at least $N+2$ bits to enumerate $N$ items. So 64-bit integers can enumerate k-subsets up to N=$62$. Without larger native integer support, to go larger you'd have to emulate larger bitsize integers.

To enumerate the $\binom{N}{k}$ subsets of size $k$, you can start with the smallest integer with $k$ bits set, $x=2^k-1$, keep iterating `x=Next(x)`, and stopping when it hits the last such integer $(2^k-1)2^{n-k}$ (or goes one past it to something greater than $2^n$).

Surprisingly, there is also a similar way to get the previous integer

```
int Prev(int y)
{
    int t = y + 1;
    int u = t ^ y;
    int v = t & y;
    return v - (v & -v) / (u + 1);
}
```

An efficient way to compute the number of size $k$ subsets of $N$ items is the binomial coefficient $\binom{N}{k}$ is 

```
int Binomial(int n, int k)
{
    int p = 1; // falling powers trick
    for (int i = 1; i <= k; ++i)
    {
        p *= (n + 1 - i);
        p /= i;
    }
    return p;
}
```

You can put all these into a little test program and see how it all works. Here's a C# program to do just that:

```c#
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
```

[1] Donald Knuth, "The Art Of Computer Programming," Vol 4A, Part I, problem 7.1.3 - 20, 21.

