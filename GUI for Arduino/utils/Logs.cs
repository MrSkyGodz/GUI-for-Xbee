using System;
using System.Collections.Generic;

struct Logs
{
    public
        int time;
        int b;
        int c;

    public Logs(int time) : this()
    {
        this.time = time;
    }

    public Logs(string time, string b, string c)
        {
            this.time = int.Parse(time);
            this.b = int.Parse(b);
            this.c = int.Parse(c);
        }

}

class Cmp : IComparer<Logs>
{
    public int Compare(Logs x, Logs y)
    {
        return (x.time).CompareTo(y.time);
    }


}
