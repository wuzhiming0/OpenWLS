using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models;
using System.Diagnostics.Metrics;
using System.Xml.Linq;
using OpenWLS.Server.DBase;
using OpenWLS.Server.GView.ViewDefinition;
using System.Drawing;
using OpenWLS.Server.LogDataFile;
using System.Configuration;

namespace OpenWLS.Server.LogInstance.RtDataFile;

public class MVReaderRt
{
    int total_samples;
    DataReader.ReadObject readObject;
    DataReader.ReadObjectDouble readDouble;
    Instrument.Measurement measurement;
    DataReader r;
    public DataReader DataReader { get { return r; } }
 //   public DataReader.ReadObjectDouble ReadDouble { get { return readDouble; } }
 //   public DataReader.ReadObject ReadObject { get { return readObject; } }

    int total_elements;
    int sample_elements;
    int sample_bytes;
    int element_bytes;

    int rd_pos;
    int rd_element;
    int rd_seek_size;
    public int ReadPos { get { return rd_pos; } }
    public MVReaderRt(Instrument.Measurement m)
    {
        measurement = m;
        sample_elements = m.MeasurementDf.Head.SampleElements;
        element_bytes = m.MeasurementDf.Head.GetElementBytes();
        sample_bytes = m.MeasurementDf.Head.GetSampleBytes();
      //  total_elements = m.Samples * m.Head.SampleElements;

    }

    public void CreateDataReader(SampleBuffer sb)
    {
        if (sb.Bytes != null)
        {
            total_samples = sb.Bytes.Length / sample_bytes;
            r = new DataReader(sb.Bytes);
            readObject = DataReader.GetObjectReader(r, measurement.DType);
            readDouble = DataReader.GetDoubleReader(r, measurement.DType);
        }
    }

    public void StartRead(int pos, int elements, int element_offset)
    {
        rd_pos = pos;
        rd_element = element_offset;
        rd_seek_size = (sample_elements - elements ) * element_bytes;
        r.Seek(rd_pos * sample_bytes + rd_element * element_bytes, SeekOrigin.Begin);
    }
    
    public double ReadDouble()
    {
        if (rd_pos >= total_samples)
        {
            rd_pos = 0;
            r.Seek(0, SeekOrigin.Begin);
        }
        rd_pos++;
        double d = readDouble();
        if (rd_seek_size > 0)
            r.Seek(rd_seek_size, SeekOrigin.Current);
        return d;
    }

    public double[] ReadDoubles()
    {
        if (rd_pos >= total_samples)
        {
            rd_pos = rd_element * element_bytes;
            r.Seek(0, SeekOrigin.Begin);
        }
        rd_pos++;
        double[] ds = new double[sample_elements];
        for (int i = 0; i < sample_elements; i++)
            ds[i] = readDouble();
        if (rd_seek_size > 0)
            r.Seek(rd_seek_size, SeekOrigin.Current);
        return ds; 
    }

}
