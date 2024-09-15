using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Drawing;


using System.Reflection;

using System.IO;
using System.Drawing;
using OpenWLS.Server.DBase;
using OpenWLS.Server.Base;
using System.Text.RegularExpressions;
using OpenWLS.Server.LogInstance;
//using OpenWLS.Display.ViewDefinition;


namespace OpenWLS.Server.GView.Models
{
   /* public interface IGDocProducer{
        SizeF Drawitems(GvDocumentS geDoc, float yOffset);
    }
*/
    public interface IGvStream
    {
        void SendObject( IGvStreamObject ob, GvDocument doc);
    }
    public class GvDocument  
    {
        protected GvGroups groups;

        public int? Id {  get; set; }    
        public GvGroups Groups { get { return groups; } }    

        protected GvItems items;
        public GvItems Items { get { return items; } }     
        protected string fileName;
        public SizeF Size { get; set; }
  //      public bool RealTime { get; set; }
        int blockId;
        public int BlockNxtID { get{ blockId++; return blockId; } }  
//        int groupId;
//        public int GroupNxtId { get{ groupId++; return groupId; } }  
        int itemId;
        public int ItemNxtId { get{ itemId++; return itemId; }} 
        int sectionId;
        public int SectionNxtId { get{ sectionId++; return sectionId; } }
        SqliteDataBase db;
        IGvStream gvStream;

//        public bool BufferItem { get; set; }

        public GvDocument(int block_id, SqliteDataBase gv_file, IGvStream gv_stream)
        {
            items = new GvItems();       groups = new GvGroups();
            itemId = 0; 
            blockId = block_id;
            db = gv_file;
            gvStream = gv_stream;
//            BufferItem =  
        }
        public GvDocument()
        { 
            items = new GvItems(); groups = new GvGroups();
            itemId = 0; blockId = 0;
        }

        public static GvDocument CreateMemoryDoc()
        {
            GvDocument doc = new GvDocument();
          //  doc.OpenDbInMemory();
            return doc;
        }
        public static GvDocument CreateDbDoc(string fn )
        {
            GvDocument doc = new GvDocument();
            if (File.Exists(fn))
                    fn = StringConverter.GetNxtFileName(fn, ".gef");
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream("OpenWLS.Server.GView.ge.db"))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                byte[] bs = new byte[stream.Length];
                reader.Read(bs, 0, bs.Length);
                using (FileStream fs = new FileStream(fn, FileMode.Create))
                using (BinaryWriter outputFile = new BinaryWriter(fs))
                {
                    outputFile.Write(bs);
                }
            }
        //    Globals.sysLog.AddMessage($"created {fn}.");
            return OpenDocument(fn);
        }

        public static GvDocument OpenDocument(string fn)
        {
            if (File.Exists(fn))
            {
                GvDocument doc = new GvDocument();
                doc.db = new SqliteDataBase();
                doc.db.OpenDb(fn);
                return doc;
            }
            else
            {
               // Globals.sysLog.AddMessage("{fn} does not exist, open failed.");
                return null;
            }
        }

        public  DataWriter GetDataWriter(int size )
        {
            if (Id != null)
                size += 6;
            DataWriter w = new DataWriter(size);
            if (Id != null)
            {
                w.WriteData((ushort)LiWsMsg.Gv);
                w.WriteData(Id);
            }
            return w;
        }

        public GvFont AddNewFont(string name, float size, ushort style)
        {
            GvFont f = new GvFont(name,size, (GvFontStyle)style );
            AddItem(f);
            return f;
        }

        public  void AddSection(GvItem item, GvSection s)
        {
            if (gvStream != null)
                gvStream.SendObject(s, this);
            else
            {
                if (db != null)
                    AddToDb(s);
                else
                    item.Sections.Add(s);
            }
        }

        public void AddItem(GvItem e)
        { 
            e.Doc = this;
            e.BId = blockId;
        //    if(e.Id < 0)
                e.Id = ItemNxtId;
            if(gvStream != null)
                gvStream.SendObject(e, this);
            else{
                if( db != null)
                    AddToDb(e); 
                else
                    items.Add(e);
            }
        }

        /*
        public void AddSection(GvSection s)
        { 
           // s.Doc = this;
          //  if(s.Id < 0)
          //      s.Id = SectionNxtId;
            if(gvStream != null)
                gvStream.SendObject(s);
            else{
                if( db != null)
                    AddToDb(s);   
                else 
                    items.Where(e => e.Id == s.IId).FirstOrDefault().Sections.Add(s);
            }   
        }
        */

        void AddToDb(GvItem item)
        {

        }
        void AddToDb(GvSection section)
        {
            
        }
        
        public SizeF AddItems(GvDocument gvDoc, float yOffset)
        {
            int id_offset = ItemNxtId;
            foreach (GvItem item in gvDoc.items)
            {
             //  if (item.EType >= GvType.Font && item.EType <= GvType.Fill)
                {
               
                    item.OffsetID(id_offset);
                    item.OffsetY(yOffset);
                    AddItem(item);
                }
            }

            return Size;
        }


    }


}
