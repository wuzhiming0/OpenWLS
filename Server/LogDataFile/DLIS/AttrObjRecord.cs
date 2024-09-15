using OpenWLS.Server.Base;
using OpenWLS.Server.LogDataFile.Models.NMRecord;

namespace OpenWLS.Server.LogDataFile.DLIS
{
    public class AttrObjRecord : ParametersRecord
    {
        public static List<AttrObjRecord> CreateAttrObjRecords(SetComponent set)
        {
            List<AttrObjRecord> ars = new List<AttrObjRecord>();
            foreach (ObjectComponent ob in set.Objects)
                ars.Add(new AttrObjRecord(ob, set.Template)
                {
                    Name = ob.Name
                });
            return ars;
        }
        public AttrObjRecord() { }

        public AttrObjRecord(ObjectComponent ob, AttributeComponents t) {
            Parameters = new Parameters();
            for( int i = 0; i < t.Count; i++)
                Parameters.Add( new Parameter()
                {
                    Name = t[i].Label,
                    Val = ob[i].ToStrings(),
                });
        }

    }
}
