using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
//using Index = OpenWLS.Server.LogDataFile.Index;

namespace OpenWLS.Server.LogInstance.RtDataFile;
/// <summary>
/// Measurement Value Read By Index
/// 
/// </summary>
public class MVReadByIndexRt
{
 //  Instrument.Measurement measurement_val;
 //   IndexMeasurement measurement_index;
    protected MVReaderRt mvReaderRt;
    double[] index_vals;

    int end_pos;
    int rdPos;

    int element;

    public bool EOR         //End Of Reader 
    {
        get { return rdPos == end_pos; }
    }

    protected double indexCoeff;
  /*  double from;

    public double From
    {
        get { return from; }
    }
  */


    /// <summary>
    /// 
    /// </summary>
    /// <param name="m">Measurement to read</param>
    /// <param name="indexType"></param>
    /// <param name="indexUnit"></param>
    /// <param name="element"></param>
    /// <returns></returns>
    public bool InitReaderBI(Instrument.Measurement m, LogIndexType? indexType, string indexUnit)
    {
        if (indexType == null)
            indexType = LogDataFile.Index.FromUnit(indexUnit);
        IndexMeasurement? index_m = m.GetIndexMeasurement((LogIndexType)indexType);
        if (index_m != null)
        {

            indexCoeff = LogDataFile.Index.GetIndexCoeff(indexUnit, m.UOM, (LogIndexType)indexType);
            index_vals = index_m.IndexVals; 
            mvReaderRt = new MVReaderRt(m);
            rdPos = index_m.WritePosition;
            end_pos = rdPos;
            return true;
        }
        return false;
    }

    
    public void StartRead( int elements, int element_offset)
    {
        mvReaderRt.StartRead(rdPos, elements, element_offset);
    }
    
    public double ReadDouble(out double index)
    {
        if(rdPos >= index_vals.Length)
            rdPos = 0;
        if (rdPos == end_pos)
        {
            index = double.NaN; 
            return double.NaN;
        }
        else
        {
            index = indexCoeff * index_vals[rdPos++];
            return mvReaderRt.ReadDouble();
        }

    }

    public double[]? ReadDoubles(out double index)
    {
        if (rdPos >= index_vals.Length)
            rdPos = 0;
        if (rdPos == end_pos)
        {
            index = double.NaN;
            return null;
        }
        else
        {
            index = indexCoeff * index_vals[rdPos++];
            return mvReaderRt.ReadDoubles();
        }
    }

}

