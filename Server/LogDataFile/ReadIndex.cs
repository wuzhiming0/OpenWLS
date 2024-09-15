using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.LogDataFile;

/// <summary>
/// 
/// </summary>
public class ReadIndex
{
    //  bool indexDecreasing;
    double spacing;
    public int PoistionBegin { get { return pos_begin_r; } }
    public double From { get { return from_r; } }
    public bool Backward { get; set;  }
    public double Spacing { get { return spacing; } }
    public int Samples { get { return samples_r; } }
    public int[] Positions { get { return positions; } }
   // public double Bottom { get; set; }
    public double[] Values { get { return values; } }
 //   public bool IndexDecreasing { get { return indexDecreasing; } }


    int[]? positions;
    double[]? values;

    Index index;
    double from_r, to_r, spacing_r;
    double begin_r, end_r, range_r;
    double abs_d_spacing;
    double index_start, index_stop;
    double index_shift;
    int  pos_begin_r, pos_end_r;
    // int samples_total;
    int samples_r;


    /// <summary>
    /// 
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="top_r"></param>
    /// <param name="bofrom_r"></param>
    /// <param name="spacing_r">0: read with data spacing</param>
    /// <param name="shift"></param>
    public ReadIndex(Index index, double from_rd, double to_rd, double spacing_rd, double shift)
    {
        from_r = from_rd;
        to_r = to_rd;

        abs_d_spacing = index.Spacing == null? 0 : Math.Abs((double)index.Spacing);
        this.index = index;
        spacing_r = spacing_rd;
        index_shift = shift;
        index_start = index.Start + shift;
        index_stop = index.Stop + shift;

        positions = null;
        values = null;
        spacing = 0;

    }

    public bool Same(double from, double to, double spacing)
    {
        return from == from_r && to == to_r && spacing == Math.Abs(spacing_r);
    }

    void SetReadRange()
    {
        if (index_start < index_stop)  // index inc
        {
            if (from_r < to_r)       // read inc
            {
                begin_r = Math.Max(from_r, index_start);
                end_r = Math.Min(to_r, index_stop);
                Backward = false;
            }
            else                   //read dec 
            {
                begin_r = Math.Max(to_r, index_start);
                end_r = Math.Min(from_r, index_stop);
                Backward = true;
            }
            range_r = end_r - begin_r;
            if(abs_d_spacing != 0)  
                pos_begin_r = (int)(( begin_r - index_start) / abs_d_spacing);
        }
        else                        //index dec
        {
            if (from_r < to_r)       // read inc
            {
                begin_r = Math.Max(from_r, index_stop);
                end_r = Math.Min(to_r, index_start);
                Backward = true;
            }
            else                   //read dec 
            {
                begin_r = Math.Max(to_r, index_stop);
                end_r = Math.Min(from_r, index_start);
                Backward = false;
            }
            range_r = begin_r - end_r;
            if (abs_d_spacing != 0)
                pos_begin_r = (int)(( index_start - begin_r) / abs_d_spacing);
        }
    }

    bool SetESpacingIndexRange()
    {
        SetReadRange();
        if (range_r < 0) return false;

        return true;
    }

    void GenerateWithDataESpacing(int mid)    {

        spacing = (double)index.Spacing;
        samples_r = (int)( range_r / spacing) + 1;
        values = new double[samples_r];
        double d = begin_r;
        for (int i = 0; i < samples_r; i++)
        {
            values[i] = d;
            d += spacing;
        }
    }

    void GenerateWithRdESpacing(int mid)
    {
        spacing = spacing_r;
        samples_r = (int)( range_r / spacing) + 1;
        positions = new int[samples_r];
        values = new double[samples_r];
        double d = begin_r;

        if (index.IndexDecrease)
        {
            for (int i = 0; i < samples_r; i++)
            {
                values[i] = d;
                positions[i] = (int)((index_start - 2) / abs_d_spacing);
                d -= spacing;
            }
        }
        else
        {
            for (int i = 0; i < samples_r; i++)
            {
                values[i] = d;
                positions[i] = (int)((d - index_start) / abs_d_spacing);
                d += spacing;
            }
        }
    }

    double[] SetVSpacingIndexRange(int mid)
    {
        SetReadRange();
        double[] ds = index.GetMeasurmentIndexes(mid);
        if (index_shift != 0)
        {
            double[] ds1 = new double[ds.Length];
            for (int i = 0; i < ds.Length; i++)
                ds1[i] = ds[i] + index_shift;
            ds = ds1;
        }
        pos_begin_r = Math.Abs(Array.BinarySearch(ds, begin_r));
        pos_end_r = Math.Abs(Array.BinarySearch(ds, end_r));
        return ds;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    bool GenerateWithDataVSpacing(int mid)
    {
        double[] ds = SetVSpacingIndexRange(mid);
        if (ds == null)
            return false;
        samples_r = pos_end_r - pos_begin_r + 1;
        values = new double[samples_r];
        int y = pos_begin_r;
        for (int i = 0; i < samples_r; i++)
            values[i] = ds[y++];

        return true;
    }

    bool GenerateWithReadVSpacing(int mid)
    {
        double[] ds = SetVSpacingIndexRange(mid);
        if (ds == null)
            return false;
        samples_r = pos_end_r - pos_begin_r + 1;
        values = new double[samples_r];
        int p = pos_begin_r;
        int p1 = pos_begin_r;
        double d = ds[pos_begin_r];

        List<int> ps = new List<int>();
        if (index.IndexDecrease)
        {
            while (p > pos_end_r)
            {
                ps.Add(p);
                d -= spacing_r;
                while (ds[p] < d)
                    p++;
            }
        }
        else
        {
            while (p < pos_end_r)
            {
                ps.Add(p);
                d += spacing_r;
                while (ds[p] < d)
                    p++;
            }
        }
        ps.Add(p);
        samples_r = ps.Count;
        positions = new int[samples_r];
        values = new double[samples_r];
        for (int i = 0; i < samples_r; i++)
        {
            positions[i] = ps[i];
            values[i] = ds[ps[i]];
        }
        return true;
    }

    bool GenerateESpacing(int mid)
    {
        if (!SetESpacingIndexRange())
            return false;

        if (spacing_r < abs_d_spacing)
            GenerateWithDataESpacing(mid);
        else
            GenerateWithRdESpacing(mid);
        return true;
    }

    void GenerateVSpacing(int mid)
    {
   //     indexDecreasing = index.IndexDecrease;
        if (spacing_r == 0)
            GenerateWithDataVSpacing(mid);
        else
            GenerateWithReadVSpacing(mid);
    }

    public void GenerateRdIndex(int mid)
    {
        if (index.EqualSpacing )
            GenerateESpacing( mid);
        else
            GenerateVSpacing( mid);
    }

}
