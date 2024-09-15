using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenWLS.Server.Base;
using OpenWLS.Server.DBase;
using Microsoft.EntityFrameworkCore.Storage;
using OpenWLS.Server.WebSocket;
using System.Text.Json.Serialization;
using OpenWLS.Server.LogInstance;

namespace OpenWLS.Server.GView.Models
{
    public class GvSection : IGvStreamObject
    {
   //     public event EventHandler SaveEvent;       
  //      public int Id { get; set; }
        public int IId { get; set; }  //item id
        public float Top { get; set; }
        public float Bot { get; set; }


        [JsonIgnore]
        public virtual bool Full { get { return false; } }
        protected bool yOffseted;
        public float Height
        {
            get { return Bot - Top; }
            set
            {
                Bot = Top + value;
            }
        }

        public GvSection()
        {
         //   Id = -1;
            Bot = float.NegativeInfinity;
            Top = float.PositiveInfinity;
  
        }    

        public bool Inside(float top, float bot)
        {
            return top <= Bot && bot >= Top;
        }



       public void Restore(DataReader r)
       {
         //   IId = r.ReadInt32();                //4 
        //    Id = r.ReadInt32();                 //4           
            Top = r.ReadSingle();
            Bot = r.ReadSingle();     
            if(!r.END)   
                RestoreVal(r.ReadByteArrayToEnd());                
        }

        public byte[] GetBytes(GvDocument doc)
        {   
            byte[] vals =  GetValBytes();
            int s = 14;
            if (vals != null)
                s += vals.Length;
            DataWriter w = doc.GetDataWriter(s);
            w.WriteData(WsService.response_gv_section);   //2
            w.WriteData(IId);                           //4
      //      w.WriteData(Id);
            w.WriteData(Top);                           //4
            w.WriteData(Bot);                           //4
            if(vals != null)w.WriteData( vals );
            return w.GetBuffer();
        }

        public virtual void YOffsetSection(float offset)
        {
           Top += offset;
           Bot += offset;
           yOffseted = true;
        }

        public virtual byte[] GetValBytes()
        {
            return null;
        } 
        public virtual void RestoreVal(byte[] bs) {        } 

        public virtual void Restore(DataRow dr)
        {
         //   Id = Convert.ToInt32(dr["Id"]);
            IId = Convert.ToInt32(dr["IId"]);
            Top = Convert.ToSingle(dr["Top"]);
            Bot = Convert.ToSingle(dr["Bot"]);
         //   Left = Convert.ToSingle(dr["Left"]);
         //   Right = Convert.ToSingle(dr["Right"]);
        }


        public void AddToDb(SqliteDataBase db)
        {
            db.ExecuteNonQuery($"INSERT INTO Sections ( IId, Top, Bot ) VALUES ( {IId}, {Top}, {Bot})");
            byte[] val = GetValBytes();
            if(val != null)
            {
               // int id = db.Get;

            }

        }
        public virtual void  ConvertToView(float dpiX, float dpiY)
        {
            Top = Top * dpiY;
            Bot = Bot * dpiY;
        }

    }

    public class GvSections : List<GvSection>
    {

    }
}