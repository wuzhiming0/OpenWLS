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
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.DBase;
using OpenWLS.Server.GView.ViewDefinition;
using System.Drawing;

namespace OpenWLS.Server.LogDataFile;

public class MVReader  
{
    int total_samples;
    DataReader.ReadObject readObject;
    DataReader.ReadObjectDouble readDouble;
    Measurement measurement;
    DataReader r;
    public DataReader DataReader { get { return r; } }
    public DataReader.ReadObjectDouble ReadDouble { get { return readDouble; } }
    public DataReader.ReadObject ReadObject { get { return readObject; } }
 
    int total_elements; 
    int sample_elements;
    int sample_bytes;
    int element_bytes;
    public MVReader(Measurement m)
    {
        measurement = m;
        sample_elements = m.Head.SampleElements;
        element_bytes = m.Head.GetElementBytes();
        sample_bytes = m.Head.GetSampleBytes();
        total_elements = m.Samples * m.Head.SampleElements;
     
        if (m.MVBlocks == null)
            m.MVBlocks = new MVBlocks(m);

       // if (measurement.MVBlocks.ValBytes == null)
        //    measurement.MVBlocks.LoadAllValue(m.DataFile);

       // if (measurement.MVBlocks.ValBytes != null)
       //     CreateDataReader();
    }

    void CreateDataReader(MVBlock b)
    {
        b.LoadValueBuffer(measurement.DataFile);
        if (b.VBuffer != null)
        {
            r = new DataReader(b.VBuffer);
            readObject = DataReader.GetObjectReader(r, measurement.Head.DType);
            readDouble = DataReader.GetDoubleReader(r, measurement.Head.DType);
        }
    }

    /*
    public void LoadAllValues()
    {
        measurement.MVBlocks.LoadAllValue();
        CreateDataReader();
    }
    */
    public double[] ReadAllDoubles()
    {
        double[] ds = new double[total_elements];
        int wr_index = 0;
 //       int wr_index = reverse ? total_elements - 1 : 0;
        foreach (MVBlock b in measurement.MVBlocks)
        {
            CreateDataReader(b);
            while ((!r.END) && wr_index >= 0)
            {              
                //if (reverse) ds[wr_index--] = readDouble();
               // else
                    ds[wr_index++] = readDouble();
            }
        } 
        return ds;
    }




 /// <summary>
 /// 
 /// </summary>
 /// <param name="rd_start">read start sample index</param>
 /// <param name="rd_samples">read samples count</param>
 /// <param name="element">element if xd data</param>
 /// <returns></returns>
    public double[] Read1DDoubles(int rd_start, int rd_samples, int element, bool backward )
    {
        double[] ds = new double[rd_samples];
        int b_start = 0;
        int rd_end = rd_start + rd_samples;
        int wr_index = backward? rd_samples - 1: 0;
        int seek_size = sample_bytes - element_bytes;
        int e_byte_offset = element * element_bytes;
        foreach (MVBlock b in measurement.MVBlocks)
        {
            int b_end = b_start + b.Samples;

            if (rd_end > b_start && b_end > rd_start)  // b_end is not the block
            {
                int k = rd_start - b_start;
                if (k < 0) k = 0;
                k = k * sample_bytes + e_byte_offset;

                CreateDataReader(b);
                r.Seek(k, SeekOrigin.Begin);

                if (sample_elements == 1)
                {
                    if (backward)
                    {
                        while ((!r.END) && wr_index >= 0)
                            ds[wr_index--] = readDouble();
                    }
                    else { 
                        while ((!r.END) && wr_index < rd_samples)
                            ds[wr_index++] = readDouble();                    
                    }
                }
                else
                {
                    if (backward)
                    {
                        while ((!r.END) && wr_index >= 0)
                        {
                            ds[wr_index--] = readDouble();
                            r.Seek(seek_size, SeekOrigin.Current);
                        }
                    }
                    else
                    {
                        while ( (!r.END) && wr_index < rd_samples )
                        {
                            ds[wr_index++] = readDouble();
                            r.Seek(seek_size, SeekOrigin.Current);
                        }
                    }
                }
            }
            b_start = b_end;
        }
        return ds;
    }


    public double[][] ReadAllXdDoubles( )
    {
        double[][] ds1 = new double[total_samples][];
        int wr_index = 0;
        foreach (MVBlock b in measurement.MVBlocks)
        {
            CreateDataReader(b);

            while ((!r.END) && wr_index >= 0)
            {
                double[] ds2 = new double[sample_elements];
                for (int j = 0; j < sample_elements; j++)
                    ds2[j] = readDouble();                
               ds1[wr_index++] = ds2;
            }

        }      
        return ds1;
       // return ReadXdDoubles(0, measurement.Head.Samples, measurement.Head.SampleElements, 0);
    }

    public string[] LoadStringAll()
    {
        string[] ss = new string[total_samples];
        return ss;
    }

/// <summary>
/// 
/// </summary>
/// <param name="rd_start"></param>
/// <param name="rd_samples"></param>
/// <param name="elements_rd">For example, read 256 elements from 8*256 data, elements_rd = 256  </param>
/// <param name="element"></param>
/// <returns></returns>
    public double[][] ReadXdDoubles(int rd_start, int rd_samples, int elements_rd, int element, bool backward)
    {       
        double[][] ds1 = new double[rd_samples][];
        int b_start = 0;
        int rd_end = rd_start + rd_samples;
        int wr_index = backward ? rd_samples - 1 : 0;
        int seek_size = sample_bytes - element_bytes;
        int e_byte_offset = element * element_bytes;
        foreach (MVBlock b in measurement.MVBlocks)
        {
            int b_end = b_start + b.Samples;

            if (rd_end > b_start && b_end > rd_start)  // b_end is not the block
            {
                int k = rd_start - b_start;
                if (k < 0) k = 0;
                k = k * sample_bytes + e_byte_offset;

                CreateDataReader(b);
                r.Seek(k, SeekOrigin.Begin);
                if (backward)
                {
                    while ((!r.END) && wr_index >= 0)
                    {
                        double[] ds2 = new double[elements_rd];
                        for (int j = 0; j < elements_rd; j++)
                            ds2[j] = readDouble();

                        ds1[wr_index--] = ds2;
                        r.Seek(seek_size, SeekOrigin.Current);
                    }
                }
                else
                {
                    while ((!r.END) && wr_index < rd_samples)
                    {
                        double[] ds2 = new double[elements_rd];
                        for (int j = 0; j < elements_rd; j++)
                            ds2[j] = readDouble();

                        ds1[wr_index++] = ds2;
                        r.Seek(seek_size, SeekOrigin.Current);
                    }
                }
            }
            b_start = b_end;
        }
        return ds1;
    }

    double[] Read1DDoublesReverse(int[] positions, int element)
    {
        int rd_samples = positions.Length;
        double[] ds = new double[rd_samples];
        int[] r_pos = positions.Reverse().ToArray();

        int rd_index = 0;
        int rd_pos = r_pos[rd_index++];
        int wr_index = rd_samples - 1;
        int b_start = 0;
        int e_byte_offset = element * element_bytes;
        foreach (MVBlock b in measurement.MVBlocks)
        {
            int b_end = b_start + b.Samples;
            if (rd_pos >= b_start && rd_pos < b_end)
            {
                CreateDataReader(b);
                while ((!r.END) && rd_pos < b_end)
                {
                    int k = rd_pos - b_start;
                    k = k * sample_bytes + e_byte_offset;
                    r.Seek(k, SeekOrigin.Begin);
                    ds[wr_index--] = readDouble();
                    if (rd_index < rd_samples)
                        rd_pos = r_pos[rd_index++];
                    else break;
                }
            }
        }
        return ds;
    }

    public double[] Read1DDoubles(int[] positions, int element )
    {
        int rd_samples = positions.Length;
        if( positions[0] > positions[rd_samples - 1])
            return Read1DDoublesReverse(positions, element);

        double[] ds = new double[rd_samples];
        int rd_index = 0;
        int rd_pos = positions[rd_index++];
        int wr_index = 0;
        int b_start = 0;
        int e_byte_offset = element * element_bytes;
        foreach (MVBlock b in measurement.MVBlocks)
        {
            int b_end = b_start + b.Samples;
            if (rd_pos >= b_start && rd_pos < b_end)
            {
                CreateDataReader(b);
                while ((!r.END) && rd_pos < b_end)
                {
                    int k = rd_pos - b_start;
                    k = k * sample_bytes + e_byte_offset;
                    r.Seek(k, SeekOrigin.Begin);                 
                    ds[wr_index++] = readDouble();
                    if (rd_index < rd_samples)
                        rd_pos = positions[rd_index++];
                    else break;
                }
            }
        }
        return ds;		
	}
	        
	public double[][] ReadXdDoubles( int[] positions, int elements_rd,  int element)
    {

        int rd_samples = positions.Length;
        double[][] ds = new double[rd_samples][];
        bool reverse = positions[0] > positions[rd_samples - 1];
        if (reverse) positions.Reverse();

        int rd_index = 0;
        int rd_pos = positions[rd_index++];
        int wr_index = reverse ? rd_samples - 1 : 0;
        int b_start = 0;
        int e_byte_offset = element * element_bytes;
        foreach (MVBlock b in measurement.MVBlocks)
        {
            int b_end = b_start + b.Samples;
            if (rd_pos >= b_start && rd_pos < b_end)
            {
                CreateDataReader(b);
                while ((!r.END) && rd_pos < b_end)
                {
                    int k = rd_pos - b_start;
                    k = k * sample_bytes + e_byte_offset;
                    r.Seek(k, SeekOrigin.Begin);
                    double[] ds2 = new double[elements_rd];
                    for (int j = 0; j < elements_rd; j++)
                        ds2[j] = readDouble();
                    if (reverse) ds[wr_index--] = ds2;
                    else ds[wr_index++] = ds2;
                    if (rd_index < rd_samples)
                        rd_pos = positions[rd_index++];
                    else break;
                }
            }
        }
        return ds;

	}
}
