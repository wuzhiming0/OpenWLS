using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;


namespace OpenWLS.Server.LogDataFile;
/// <summary>
/// Measurement Value Read By Index
/// 
/// </summary>
public class MVReadByIndex
{
    Measurement measurement;
    Index mIndex;               
    protected MVReader mvReader;

    protected double[] buffer_1d;
    protected double[][] buffer_xd;
    protected byte[][] buffer_vs;
    protected double[] indexes;
    int samples;
    int rdPos;
    double indexShift;
    int element;

    public bool EOR         //End Of Reader 
    {
        get  { return rdPos >= samples;   }
    }

    protected double indexCoeff;
    double from;

    public double From
    {
        get { return from;  }
    }
 
    double spacing;
    public double Spacing
    {
        get  { return spacing; }
    }
    
    public bool EqualSpacing { get { return mIndex.EqualSpacing; } }

/// <summary>
/// 
/// </summary>
/// <param name="m">Measurement to read</param>
/// <param name="indexType"></param>
/// <param name="indexUnit"></param>
/// <param name="element"></param>
/// <returns></returns>
    public bool InitReaderBI(Measurement m, LogIndexType? indexType, string indexUnit, int element)
    {
        measurement = m;
        if (indexType == null)
            indexType = Index.FromUnit(indexUnit);
        mIndex = m.Frame.GetIndex((LogIndexType)indexType);
        if (mIndex == null)
            return false;
        mIndex.LoadIndexBuffer();
        indexShift = measurement.Head.IndexShift == null ? 0 : (double)measurement.Head.IndexShift;
        this.element = element; 
        indexCoeff = Index.GetIndexCoeff( indexUnit,  mIndex.UOM, mIndex.Type );
        if (indexType == LogIndexType.BOREHOLE_DEPTH)
            mIndex.IndexShift = indexShift;

        return true;
    }
    
/// <summary>
/// Calculate read indexes
/// </summary>
/// <param name="indexRd"></param>
    void ConvertIndexesUnit(ReadIndex indexRd)
    {
        from = indexRd.From * indexCoeff;
        spacing = indexRd.Spacing * indexCoeff;
        indexes = indexRd.Values;
        samples = indexRd.Samples;
        if (indexCoeff != 1)
        {
            for (int i = 0; i < samples; i++)
                indexes[i] = indexes[i] * indexCoeff;
        }
    }

    public void Load1dData(double from, double to)
    {
        ReadIndex indexRd = new ReadIndex(mIndex, from / indexCoeff, to / indexCoeff, 0, indexShift);
        indexRd.GenerateRdIndex(measurement.Id);
        ConvertIndexesUnit(indexRd);
        mvReader = new MVReader(measurement);

        buffer_1d = mvReader.Read1DDoubles( indexRd.PoistionBegin, samples, 0, indexRd.Backward);
//        if (indexRd.IndexDecreasing)
//            Array.Reverse(buffer_1d);
    }

    public void Load1dData(double from, double to, double spacing)
    {
        ReadIndex indexRd = new ReadIndex(mIndex, from / indexCoeff, to / indexCoeff, spacing / indexCoeff, indexShift);
        indexRd.GenerateRdIndex(measurement.Id);
        ConvertIndexesUnit(indexRd);
        mvReader = new MVReader(measurement);
        buffer_1d = mvReader.Read1DDoubles( indexRd.PoistionBegin, samples, 0, indexRd.Backward);
//        if (indexRd.IndexDecreasing)
//            Array.Reverse(buffer_1d);
    }

    public void LoadXdData(double from, double to, int elements)
    {
        ReadIndex indexRd = new ReadIndex(mIndex, from / indexCoeff, to / indexCoeff, 0, indexShift);
        indexRd.GenerateRdIndex(measurement.Id);
        ConvertIndexesUnit(indexRd);
        mvReader = new MVReader(measurement);

        buffer_xd = mvReader.ReadXdDoubles(indexRd.PoistionBegin, samples, elements, 0, indexRd.Backward);
   //     if (indexRd.IndexDecreasing)
   //         Array.Reverse(buffer_1d);
    }
    public void LoadXdData(double from, double to, double spacing)
    {
        LoadXdData(from, to, spacing, measurement.Head.Get1DElements());
    }
    public void LoadXdData(double from, double to, double spacing, int elements)
    {
        ReadIndex indexRd = new ReadIndex(mIndex, from / indexCoeff, to / indexCoeff, spacing / indexCoeff, indexShift);
        indexRd.GenerateRdIndex(measurement.Id);
        ConvertIndexesUnit(indexRd);
        mvReader = new MVReader(measurement);
        if (indexRd.Positions == null)
            buffer_xd = mvReader.ReadXdDoubles( indexRd.PoistionBegin, samples, elements, 0, indexRd.Backward);
        else
            buffer_xd = mvReader.ReadXdDoubles( indexRd.Positions, elements, 0);

   //     if (indexRd.IndexDecreasing)
    //        Array.Reverse(buffer_1d);
    }

    public virtual double ReadData(out double index)
    {
        index = indexes[rdPos];
        return buffer_1d[rdPos++];
    }

    public virtual double[] ReadDoubles(out double index)
    {
        index = indexes[rdPos];
        return buffer_xd[rdPos++];
    }

    public void Skip()
    {
        rdPos++;
    }
}

