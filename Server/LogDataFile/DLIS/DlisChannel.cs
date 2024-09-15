using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;
using System.IO;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.LogDataFile;

namespace OpenWLS.Server.LogDataFile.DLIS;

public class DlisChannel : Measurement
{
	DlisDataType dataDlisType;
    public static DlisChannel CreateDlisChannel(ObjectComponent oc, DlisFrame frame, AttributeComponents template)
    {
        DlisChannel newCurve = new DlisChannel();
        if (newCurve.InitDlisChannel(oc, frame, template))
            return newCurve;
        else
            return null;
    }


    public bool InitDlisChannel(ObjectComponent oc, DlisFrame frame, AttributeComponents template)
    {
        try
        {
            Frame = frame;
            Head.Name = oc.Name;
            Head.Dimensions = EFLRComponent.GetIntArray(oc[template.GetComponentIndex("DIMENSION")].Value);
            Head.UOM = (string)oc[template.GetComponentIndex("UNITS")].Value;
            dataDlisType = (DlisDataType)((byte)oc[template.GetComponentIndex("REPRESENTATION-CODE")].Value);
            Head.DType = OpenWLS.Server.Base.DataType.GetSparkPlugDataType(dataDlisType);

            if (frame.LevelSpacing != null)
            { 
                if (frame.LevelSpacing < 0)
                {
                    StartIndex = (double)frame.IndexMax;
                    StopIndex = (double)frame.IndexMin;
                }
                else
                {
                    StartIndex = (double)frame.IndexMin;
                    StopIndex = (double)frame.IndexMax;
                }

                Head.Spacing = frame.LevelSpacing;
                Head.UOI = frame.UOI;
            }
            /*
            if(Head.Name == "TDEP")
            {
                Head.IType = LogIndexType.BOREHOLE_DEPTH;
                Head.IndexM = true;
            }
            if (Head.Name == "TIME")
            {
                Head.IType = LogIndexType.TIME;
                Head.IndexM = true;
            }
            */
            CreateMVWriter(frame.Samples);
            mVWriter.ResetBuffer();
            return true;
        }
        catch (Exception e1)
        {
            return false;
        }

    }




    //return ture when complete all data
    public bool ReadCurveData(DataReader r, int bufferDataLength)
		{
       // Console.WriteLine(Head.Name + ":" + mVWriter.Postion);
            r.Seek( mVWriter.WriteSampleBigEndian(r.GetBuffer(), r.Position), SeekOrigin.Current);


            return true;
		}	

	}
