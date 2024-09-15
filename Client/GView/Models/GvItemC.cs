using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

using System.Drawing;
using OpenWLS.Server.Base;
using OpenWLS.Server.GView.Models;

namespace OpenWLS.Client.GView.Models
{

    public interface IGvItemC
    {
        void DrawItem(Graphics g, float top, float bot);
        float DrawScrollbar(Graphics g, float leftMargin, float w, float sh);
        void Init(float dpiX, float dpiY, GvItemCs intems);
      //  void OffsetElementY(float offset, float dpiY);
        void OffsetControl(double scrollX, double scrollY);
    }



    public class GvBOSC : GvBOS, IGvItemC
    {
        public void DrawItem(Graphics g, float top, float bot){      }


        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void Init(float dpiX, float dpiY, GvItemCs intems){      }
        public void OffsetElementY(float offset, float dpiY){     }
        public void OffsetControl(double scrollX, double scrollY)
        {

        }
    }
    public class GvEOSC : GvEOS, IGvItemC
    {
        public void DrawItem(Graphics g, float top, float bot) {      }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void Init(float dpiX, float dpiY, GvItemCs intems)
        {

        }
        public void OffsetElementY(float offset, float dpiY)
        {

        }


        public void  OffsetControl( double scrollX, double scrollY)
        {

        }
    }

    public class GvBOAC : GvBOA, IGvItemC
    {
        public void DrawItem(Graphics g, float top, float bot){  }


        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void Init(float dpiX, float dpiY, GvItemCs intems)
        {

        }
        public void OffsetElementY(float offset, float dpiY)
        {

        }
        public void OffsetControl(double scrollX, double scrollY)
        {

        }
    }
    public class GvEOAC : GvEOA, IGvItemC
    {
        public void DrawItem(Graphics g, float top, float bot){   }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void Init(float dpiX, float dpiY, GvItemCs intems)
        {

        }
        public void OffsetElementY(float offset, float dpiY)
        {

        }


        public void OffsetControl(double scrollX, double scrollY)
        {

        }
    }

    public class GvBOUC : GvBOU, IGvItemC
    {
        public void DrawItem(Graphics g, float top, float bot){ }


        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void Init(float dpiX, float dpiY, GvItemCs intems)
        {

        }
        public void OffsetElementY(float offset, float dpiY)
        {

        }
        public void OffsetControl(double scrollX, double scrollY)
        {

        }
    }


    public class GvYOffsetC : GvYOffset, IGvItemC
    {
        public void DrawItem(Graphics g, float top, float bot) {   }

        public float DrawScrollbar(Graphics g, float leftMargin, float w, float sh)
        {
            return leftMargin;
        }

        public void Init(float dpiX, float dpiY, GvItemCs intems)
        {

        }

        public void OffsetElementY(float offset, float dpiY)
        {

        }


        public void OffsetControl( double scrollX, double scrollY)
        {

        }
    }



    public class GvItemCs : List<IGvItemC>
    {
        public event PropertyChangedEventHandler PropertyChanged;

        List<GvCurveSectionC> curveSections;
        public GvParameterSectionC SelectedPara { get; set; }
        public List<GvCurveSectionC> CurveSections { get { return curveSections; } }

        
        public void CreateFromDoc(GvDocument doc)
        {
            Clear();
      /*      foreach (GeBlockInfor infor in doc.Infors.InforTbl)
            {
                DataReader r = doc.ReadElement(infor);
                IGvItemC ni = GvItemCs.CreateItem(r, doc);
                if (ni != null)
                    Add(ni);
            }
      */
        }

        public void CreateFromItems(GvItems es)
        {
            Clear();
            foreach (GvItem  e in es)
            {
                IGvItemC ni = GvItemCs.CreateItem(e);
                if (ni != null)
                    Add(ni);
            }
        }
/*
        public static IGvItemC CreateItem(DataReader r, GvDocument doc)
        {
            GvType t = (GvType)r.ReadUInt16();
            IGvItemC new_item = null;
            switch (t)
            {
                case GvType.Font:
                    new_item = new GvFontC(); break;

                case GvType.Text:
                    new_item = new GvTextC(); break;
                case GvType.Symbol:
                    new_item = new GvSymbolC(); break;
   
        //        case GvType.CurveES:
        //            new_item = new GvCurveEsC(); break;
                case GvType.CurveVS:
                    new_item = new GvCurveVsC(); break;  
   
                case GvType.Line:
                    new_item = new GvLineC(); break;                
                case GvType.BmpHead:
                    new_item = new GvBmpHeadC(); break;
        //        case GvType.Pixels:
        //            new_item = new GvPixelsC(); break;
                case GvType.Rect:
                    new_item = new GvRectC(); 
                    break;
               case GvType.FillingImage:
                    new_item = new GvFillImageC(); break;

                case GvType.Fill:
                    new_item = new GvFillC(); break;

                case GvType.BOA:
                    new_item = new GvBoaC(); break;
                case GvType.EOA:
                    new_item = new GvEoaC(); break;

    //            case GvType.BOS:
    //                new_item = new GvBosC(); break;
    //            case GvType.EOS:
    //                new_item = new GvEosC(); break;

     //           case GvType.NumberValueFormat:
     //               new_item = new  GvNumberValueFormatC(); break;
     //           case GvType.NumberValue:
     //               new_item = new  GvNumberC(); break;
     //           case GvType.Parameter:
     //               new_item = new GvParameterC(); break;
            }
            if (new_item == null)
                return null;

            ((GvItem)new_item).Doc = doc;
//            ((GvItem)new_item).Infor = infor;
 //           ((GvItem)new_item).RestoreElementBody(r);
            return new_item;
        }
        */

        public static IGvItemC CreateItem(GvItem gvItem)
        {
            IGvItemC new_item = null;
            switch (gvItem.EType)
            {
                case GvType.Font:
                    new_item = new GvFontC(); break;

                case GvType.Text:
                    new_item = new GvTextC(); break;
                case GvType.Symbol:
                    new_item = new GvSymbolC(); break;

        //        case GvType.CurveES:
        //            new_item = new GvCurveEsC(); break;
                case GvType.CurveVS:
                    new_item = new GvCurveVsC(); break;

                case GvType.Line:
                    new_item = new GvLineC(); break;
        //        case GvType.BmpHead:
        //            new_item = new GvBmpHeadC(); break;
         //       case GvType.Pixels:
         //           new_item = new GvPixelsC(); break;
                case GvType.Rect:
                    new_item = new GvRectC();
                    break;
                case GvType.FillingImage:
                    new_item = new GvFillImageC(); break;

                case GvType.Fill:
                    new_item = new GvFillC(); break;
                case GvType.BOS:
                    new_item = new GvBOSC(); break;
                case GvType.EOS:
                    new_item = new GvEOSC(); break;
        //        case GvType.NumberValueFormat:
        //            new_item = new GvNumberValueFormatC(); break;
          //      case GvType.NumberValue:
          //          new_item = new GvNumber(); break;
          //      case GvType.Parameter:
          //          new_item = new GvParameterC(); break;
            }
            if (new_item == null)
                return null;

    //        ((GvItem)new_item).Doc = .Doc;
    //        ((GvItem)new_item).Infor = infor;

      /*      DataWriter w = new DataWriter( (int)new_item.GetBodySize() );
            w.SetByteOrder(false);
            new_item.WriteBody(w);
            DataReader r = new DataReader( w.GetBuffer());
            r.SetByteOrder(false);
            ((GvItem)new_item).RestoreElementBody(r);
      */
            return new_item;
        }

/*
        public GvParameterCs Paras
        {
            get
            {
                GvParameterCs ps = new GvParameterCs();
                foreach (IGvItemC e in this)
                {
                    if (e is GvParameterC)
                        ps.Add((GvParameterC)e);
                }

                return ps;
            }
        }
*/
        public GvNumberSection GetValueSection(string name)
        {
            foreach (IGvItemC e in this)
            {
                if( ((GvItem)e).EType == GvType.NumberValue)
                {
                    GvNumber v = (GvNumber)e;
                    foreach(GvNumberSection s in v.Sections)
                    if (s.Name == name)
                        return s;
                }

            }
            return null;
        }

        public IGvItemC GetFirstItem(int eid, GvType t)
        {
            foreach (IGvItemC e in this)
            {
                if (((GvItem)e).EType == t && ((GvItem)e).Id == eid)
                    return e;
            }
            return null;
        }

        public IGvItemC GetLastItem(int eid, GvType t)
        {
            
            for (int i = Count -1; i >=0; i--)
            {
                IGvItemC e = this[i];
                if (((GvItem)e).EType == t && ((GvItem)e).Id == eid)
                    return e;
            }
            return null;
        }


        public GvCurveSectionCs GetCurveSections(int id)
        {
            GvCurveSectionCs gvCurveSections = new GvCurveSectionCs();
            foreach (GvCurveSectionC s in curveSections)
                if(s.IId == id)
                     gvCurveSections.Add(s);
            return gvCurveSections;
        }


        public void DrawItems(Graphics g, double top, double bottom)
        {
            curveSections = new List<GvCurveSectionC>();
            foreach (IGvItemC e in this)
            {
                if (((GvItem)e).EType != GvType.Fill)
                    e.DrawItem(g, (float)top, (float)bottom);
            }
            foreach (IGvItemC e in this)
            {
                if (((GvItem)e).EType == GvType.Fill)
                    e.DrawItem(g, (float)top, (float)bottom);
            }
            curveSections = null;
        }
        public  void Init(float dpiX, float dpiY, GvItemCs items)
        {
            lock (this)
            {
                foreach (IGvItemC e in this)
    //                if (!(e is GvParameterC))
                        e.Init(dpiX, dpiY, items);
            }
        }

        public void AddRealTimeItem(IGvItemC e)
        {
     /*       GvItem ge = (GvItem)e;
            GvItem ge1 = (GvItem)GetLastItem(ge.Id, ge.EType);
            if (ge1 == null || ge1.YOffseted)
                Add(e);
            else
                ge1.Merge(ge);
     */
        }

        public void OffsetY(float offset, float dpiY)
        {
     //       foreach (IGvItemC e in this)
     //           e.OffsetY(offset, dpiY);

        }
        private void NotifyPropertyChanged(String PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

    /*    GvParameterC sp;
        public GvParameterC SelectedPara
        {
            get
            {
                return sp;
            }

            set
            {
                sp = value;
                foreach (IGvItemC e in this)
                {
                    if (e is GvParameterC)
                    {
                        GvParameterC p = (GvParameterC)e;
                        p.Selected = p == sp;
                    }

                }
                NotifyPropertyChanged("SelectedPara");
            }
        }
    */
    }

   

}
