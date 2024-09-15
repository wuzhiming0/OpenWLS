using OpenWLS.Server.Base;

namespace OpenWLS.Server.LogDataFile.DLIS
{
    public class StorageUnitLabel
    {
        public int StorageUnitSequenceNumber { get; set; }
        public string VersionDLIS { get; set; }
        public string StorageUintStruct { get; set; }
        public int MaxRecordLength { get; set; }
        public string StorageSetIdentifier { get; set; }

        public StorageUnitLabel()
        {
            StorageUnitSequenceNumber = 1;
            VersionDLIS = "V1.00";
            StorageUintStruct = "RECORD";
            MaxRecordLength = 16384;
            StorageSetIdentifier = "CUSTOMER";
        }

        public void WriteStorageUnitLabel(DataWriter w)
        {
            w.WriteString(StorageUnitSequenceNumber.ToString(), 4);
            w.WriteString(VersionDLIS, 5);
            w.WriteString(StorageUintStruct, 6);
            w.WriteString(MaxRecordLength.ToString(), 5);
            w.WriteString(StorageSetIdentifier, 60);
        }

        public void ReadStorageUnitLabel(DataReader r)
        {
            StorageUnitSequenceNumber = Convert.ToInt32(r.ReadString(4));
            VersionDLIS = r.ReadString(5);
            StorageUintStruct = r.ReadString(6);
            MaxRecordLength = Convert.ToInt32(r.ReadString(5));
            StorageSetIdentifier = r.ReadString(60);
        }
    }
}
