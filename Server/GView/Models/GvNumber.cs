using OpenWLS.Server.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenWLS.Server.GView.Models
{
    public class GvNumber : GvItem
    {
        public int ValueFId { get; set; }
        public int RangeFId { get; set; }
        public byte ValueDecimals { get; set; }


        GvNumberSection sectionCur;
        public GvNumber()
        {
            EType = GvType.NumberValue;
        }

        protected override void RestoreExt(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            ValueFId = r.ReadInt32();
            RangeFId = r.ReadInt32();
            ValueDecimals = r.ReadByte();
        }

        public override byte[]? GetItemExtBytes()
        {
           DataWriter w = new DataWriter(12);
           w.WriteData(ValueFId);
           w.WriteData(RangeFId);
           w.WriteData(ValueDecimals);
           return w.GetBuffer();
        }
        public GvNumber(  byte format, GvFont valueFont,   GvFont rangeFont)
        {
            EType = GvType.NumberValueFormat;
            ValueFId = valueFont.Id;
            RangeFId = rangeFont.Id;
            ValueDecimals = format;
        }

    }

    public enum ValueLimitStatus{Unknown = 0, Inside, Outside};
    public class GvNumberSection :  GvSection
    {
        public string Name { get; set; }
        public float Value { get; set; }
        public float HighLimit { get; set; }
        public float LowLimit { get; set; }
        public bool Active { get; set; }       
 
        public GvNumberSection()
        {
            //EType = GvType.NumberValue;
        }

        protected  void Restore(byte[] bs)
        {
            DataReader r = new DataReader(bs);
            Value = r.ReadSingle();
            HighLimit = r.ReadSingle();
            LowLimit = r.ReadSingle();
            Name = Encoding.UTF8.GetString(r.ReadByteArrayToEnd());
        }

        protected  byte[] GetBytes()
        {
            byte[] bs = Encoding.UTF8.GetBytes(Name);
            DataWriter w = new DataWriter(16 + bs.Length);
            w.WriteData(Value);
            w.WriteData(HighLimit);
            w.WriteData(LowLimit);
            w.WriteData(bs);
            return bs;
        }
        public ValueLimitStatus GetStatus()
        {
            if (HighLimit == LowLimit)
                return ValueLimitStatus.Unknown;
            ValueLimitStatus hs = ValueLimitStatus.Unknown;
            if ((!float.IsNaN(HighLimit)) && (!float.IsNaN(Value)))
            {
                if (Value > HighLimit)
                    hs = ValueLimitStatus.Outside;
                else
                    hs = ValueLimitStatus.Inside;
            }
            ValueLimitStatus ls = ValueLimitStatus.Unknown;
            if ((!float.IsNaN(LowLimit)) && (!float.IsNaN(Value)))
            {
                if (Value < LowLimit)
                    ls = ValueLimitStatus.Outside;
                else
                    ls = ValueLimitStatus.Inside;
            }
            if (ls == ValueLimitStatus.Outside || hs == ValueLimitStatus.Outside)
                return ValueLimitStatus.Outside;
            if (ls == ValueLimitStatus.Inside || hs == ValueLimitStatus.Inside)
                return ValueLimitStatus.Inside;
            return ValueLimitStatus.Unknown;
        }
    }
}
