using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;

using System.IO;
using OpenWLS.Server.LogDataFile;
using OpenWLS.Server.LogDataFile.Models;
using OpenWLS.Server.DBase;
using OpenWLS.Server.WebSocket;
using OpenWLS.Server.LogDataFile.Models.NMRecord;

namespace OpenWLS.Server.LogDataFile.DLIS;

public enum DlisTimeZone { LocalStandardTime = 0, LocalDaylightSavingsTime = 1, UniversalCoordinatedTime = 2 }
public class DlisFile
{
    protected string fileName;
    protected bool validFile;
    protected FileStream fileStream;
    protected StorageUnitLabel storageUnitLabel;

    protected SetComponents sets;
    protected DlisFrames frames;

    /*
    public bool isTemplate()
    {
        return this.sets[this.sets.Count - 1].isTemplate();
    }*/

    public static DataFileInfor? ImportDLIS(string fn, ISyslogRepository syslog)
    {
        try
        {
            FileStream fs = new FileStream(fn, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            DataReader r = new DataReader(fs, 82);
            r.SetByteOrder(false);
            StorageUnitLabel sul = new StorageUnitLabel();
            sul.ReadStorageUnitLabel(r);
            syslog.AddMessage($"Import {fn}\n {sul.VersionDLIS}");
            DlisFile dlis_file = sul.VersionDLIS == "V1.00" ? new V1.DlisFileV1() : new V2.DlisFileV2();
            dlis_file.storageUnitLabel = sul;
            dlis_file.fileName = fn;
            dlis_file.fileStream = fs;
            return dlis_file.ImportDLIS(r.ReadUInt16(), syslog);
        }
        catch (Exception e1)
        {
            return null;
        }
    }

    void CountFrame(DlisIndirectFormatLogicalRecord iflr)
    {
        SetComponent s = sets.GetSetComponent("FRAME");
        if(s != null)
        {
            ObjectComponent o = s.Objects.GetObjectComponent(iflr.Indentifier);
            if (o != null)
                o.Samples++;
        }
    }

    #region Properties
 
    public SetComponents SetComponents
    {
        get
        {
            return sets;
        }
    }

    #endregion

    public DlisFile()
    {
        sets = new SetComponents();
        frames = new DlisFrames();
        storageUnitLabel = new StorageUnitLabel();  
        validFile = true;
    }

    protected void SaveMeasurments(DataFile df)
    {
        int mid = 0;
        foreach (DlisFrame f in frames)
        {
            foreach (Measurement m in f.Measurements)
            {
                m.DataFile = df;
              //  m.Head.VMax = m.SampleBuffer.ValueMax;
              //  m.Head.VMin = m.SampleBuffer.ValueMin;
              //  m.Head.Samples = f.Samples;
               ((MHead)m.Head).AddHeadToDb(df);
               ((DlisChannel)m).AddMVBlock();
            }
        }
    }

   
        
    protected void SaveParameters(DataFile df)
    {
        List<AttrObjRecord> dlisOrigins = AttrObjRecord.CreateAttrObjRecords(sets.GetSetComponent("ORIGIN"));
        foreach (AttrObjRecord origin in dlisOrigins)
            df.NMRecords.Add(origin);

        SetComponent ps = sets.GetSetComponent("PARAMETER");
        if (ps == null)
            return;
        int cdim = ps.Template.GetComponentIndex("DIMENSION");
        int cv = ps.Template.GetComponentIndex("VALUES");
        int cz = ps.Template.GetComponentIndex("ZONES");
        int cn = ps.Template.GetComponentIndex("LONG-NAME");
        Parameters aps = new Parameters();

     //   string str = "";
        foreach (ObjectComponent o in ps.Objects)
        {
            Parameter p = new Parameter();
            p.Name = o.Name;
            if (cz > 0)
            {
                if(o[cz].Value != null)
                    p.Zone = (string)o[cz].Value;
            }
            if (cn > 0)
            {
                if(o[cn].Value != null)
                    p.Description = (string)o[cn].Value;
            }

            if (cv> 0)
            {
                if (o[cv].Value != null){
                    SparkPlugDataType dt = DataType.GetSparkPlugDataType(o[cv].RepCode);
                    p.Val = o[cv].ToStrings();
                    p.DataType = dt;
                }

            }
            aps.Add(p);
        }
        ParametersRecord pr = new ParametersRecord()
        { 
            Parameters = aps,
            Name = "DLIS Para"
        };
       // pr.SaveExtension();
        df.NMRecords.Add(pr);
    }

    public virtual DataFileInfor? ImportDLIS(int nxtRLength, ISyslogRepository syslog)
    {
        return null;
    }
  
    public bool CreateDLIS(string strFN)
    {
        try
        {
            fileStream = new FileStream(strFN, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.None);
            return true;
        }
        catch (Exception e)
        {
        }
        return false;
    }

    protected virtual void InitFrameChannelSet(SetComponents sets)
    {

    }




    public void BeforeNewComponent(EFLRComponent sender)
    {
        if (sets.Count > 0)
            this.sets[sets.Count - 1].BeforeAddNewComponent((EFLRComponent)sender);
    }


    public void AfterNewComponent(EFLRComponent sender)
    {
        if (sender is SetComponent)
            sets.Add((SetComponent)sender);
        else
            sets[sets.Count - 1].AfterAddNewComponent((EFLRComponent)sender);
    }





    public void ReadIndirectFormatLogicalRecord(DlisIndirectFormatLogicalRecord iflr, bool scan)
    {
         if (scan)
                CountFrame(iflr);
            else
            {
                DlisFrame df = this.frames.GetFrame(iflr.Indentifier);
                if (df != null)
                    df.ReadFrameData(iflr.DataReader, iflr.BufferDataLength);
            }

    }

    protected int CountMeasurement(string name)
    {
        int count = 0;
        foreach (DlisFrame f in frames)
        {
            foreach (Measurement m in f.Measurements)
            {
                if (m.Head.Name == name) count++;
            }
        }
        return count;
    }

    public void CreateComponet(int descriptor,
    string label, int count, DlisDataType type, object unit, object value)
    {
        EFLRComponent newtemplateComponent;

        newtemplateComponent = EFLRComponent.CreateComponent(descriptor);
        BeforeNewComponent(newtemplateComponent);
        newtemplateComponent.WriteComponent(descriptor, label, count, type, unit, value);
        AfterNewComponent(newtemplateComponent);
    }

    public void CreateComponet(Measurement curve, int descriptor)
    {
        EFLRComponent newTemplateComponent;

        newTemplateComponent = EFLRComponent.CreateComponent(descriptor);
        BeforeNewComponent(newTemplateComponent);
        newTemplateComponent.WriteComponent(curve, descriptor);
        AfterNewComponent(newTemplateComponent);
    }

    public void CreateComponet(int descriptor, string name)
    {
        EFLRComponent newtemplateComponent;

        newtemplateComponent = EFLRComponent.CreateComponent(descriptor);
        BeforeNewComponent(newtemplateComponent);
        newtemplateComponent.WriteComponent(descriptor, name);
        AfterNewComponent(newtemplateComponent);
    }

}
