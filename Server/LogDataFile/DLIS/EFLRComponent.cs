using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenWLS.Server.Base;

using System.IO;
using OpenWLS.Server.LogDataFile;
using System.Reflection.Metadata.Ecma335;
using OpenWLS.Server.LogDataFile.Models;

namespace OpenWLS.Server.LogDataFile.DLIS
{
    public enum ComponentRole { ABSATR = 0, ATTRIB = 0x20, INVATR = 0x40, OBJECT = 0x60, Unused = 0x80, RedundantSet = 0xa0, ReplacementSet = 0xc0, NormalSet = 0xe0 };


    /// <summary>
    /// Summary description for AttributeComponent.
    /// </summary>
    public class AttributeComponent : EFLRComponent
    {
        [FlagsAttribute]
        public enum AttributeComponents { Value = 1, Unit = 2, RepresentationCode = 4, Count = 8, Label = 16 };
        string label;
        int count;
        int countBytes;
        DlisDataType representationCode;


        string units;
        object theValue;

        public DlisDataType RepCode { get { return representationCode; } }
        public int Count { get { return count; } }

        public object Value
        {
            get
            {
                return theValue;
            }
        }

        public string Label
        {
            get
            {
                return label;
            }
        }

        public string Units
        {
            get
            {
                return units;
            }
        }

        public string[] ToStrings()
        {
            if (count == 0) return null;
            if (count == 1) return new string[] { theValue.ToString() };
            string[] ss = new string[count];
            object[] obs = (object[])theValue;
            for (int i = 0; i < count; i++)
                ss[i] = obs[i].ToString();

            return ss;
        }


        public override string ToString()
        {
            if (count == 0) return null;
            if (count == 1) return theValue.ToString();
            object[] obs = (object[])theValue;
            string str = obs[0].ToString();
            for (int i = 1; i < count; i++)
                str = str + ", " + obs[i].ToString();
            return str;
        }

        public AttributeComponent(int iDescriptor)
        {
            descriptor = iDescriptor;
            label = "";
            count = 1;
            representationCode = DlisDataType.IDENT;
            units = "";
            theValue = null;
        }

        
        protected override void WriteAttributeComponent(int iDescriptor,
            string ilabel,
            int icount,
            DlisDataType itype,
            object iunits,
            object ithevalue)
        {
            descriptor = iDescriptor;
            label = ilabel;
            count = icount;
            representationCode = itype;
            units = (string)iunits;
            theValue = ithevalue;
        }

        public void InitAttributeComponent(AttributeComponent ac)
        {
            label = ac.label;
            count = ac.count;
            representationCode = ac.representationCode;
            units = ac.units;
            theValue = ac.theValue;
        }


        protected override void OnReadComponent(DataReader r)
        {
            if (ComponentRole != ComponentRole.ABSATR)
            {

                if ((descriptor & (int)AttributeComponents.Label) != 0)
                    label = (string)r.ReadDataObject(DlisDataType.IDENT);
                countBytes = r.Position;
                if ((descriptor & (int)AttributeComponents.Count) != 0)
                    count = (int)r.ReadDataObject(DlisDataType.UVARI);
                countBytes = r.Position - countBytes;
                if ((descriptor & (int)AttributeComponents.RepresentationCode) != 0)
                    representationCode = (DlisDataType)r.ReadByte();
                if ((descriptor & (int)AttributeComponents.Unit) != 0)
                    units = (string)r.ReadDataObject(DlisDataType.UNITS);
                if ((descriptor & (int)AttributeComponents.Value) != 0 && count > 0)
                {
                    if (count == 1)
                        theValue = r.ReadDataObject(representationCode);
                    else
                    {
                        object[] obs = new object[count];
                        for (int i = 0; i < count; i++)
                            obs[i] = r.ReadDataObject(representationCode);
                        theValue = obs;
                    }
                }
            }
        //    else
         //       count = count;
        }
        
        public void WriteFileComponent(DataWriter w)
        {
            int i;
            byte tempval = (byte)descriptor;

            w.WriteDataObject(tempval, DlisDataType.USHORT);
            if (ComponentRole != ComponentRole.ABSATR)
            {
                if ((descriptor & (int)AttributeComponents.Label) != 0)
                    w.WriteDataObject(Label, DlisDataType.IDENT);
                if ((descriptor & (int)AttributeComponents.Count) != 0)
                    w.WriteDataObject(count, DlisDataType.UVARI);
                if ((descriptor & (int)AttributeComponents.RepresentationCode) != 0)
                {
                    tempval = (byte)representationCode;
                    w.WriteDataObject(tempval, DlisDataType.USHORT);
                }
                if ((descriptor & (int)AttributeComponents.Unit) != 0)
                    w.WriteDataObject(units, DlisDataType.UNITS);
                if ((descriptor & (int)AttributeComponents.Value) != 0 && count > 0)
                {
                    switch (representationCode)
                    {
                        case DlisDataType.OBNAME:
                            if (count == 1)
                            {
                                DLISObjectName on;
                                on.origin = 0;
                                on.copy = 0;
                                on.Indenifier = (string)theValue;
                                w.WriteDataObject(on, DlisDataType.OBNAME);
                            }
                            else
                            {
                                for (i = 0; i < count; i++)
                                {
                                    DLISObjectName on;
                                    object[] tempobj = (object[])theValue;

                                    on.origin = 0;
                                    on.copy = 0;
                                    on.Indenifier = (string)tempobj[i];

                                    w.WriteDataObject(on, DlisDataType.OBNAME);
                                }
                            }
                            break;

                        default:
                            if (count == 1)
                            {
                                w.WriteDataObject(theValue, representationCode);
                            }
                            else
                            {
                                int[] num = GetIntArray(theValue);
                                for (i = 0; i < count; i++)
                                    w.WriteDataObject(num[i], representationCode);
                            }
                            break;

                    }
                }
            }
        }

        
        protected override void WriteAttributeComponent(int descriptor, string name)
        {
            return;
        }

        
        protected override void WriteAttributeComponent(Measurement curve, int descriptor)
        {
            label = curve.Head.Name;
            count = 1;

            return;
        }

    }

    public class ObjectComponent : EFLRComponent
    {
        public static int index;
        [FlagsAttribute]
        public enum ObjectComponents { Name = 16 };
        int origin;
        int copy;
        string name;
        AttributeComponent[] values;
        object tag;
        public int Samples { get; set; }

        public string Name
        {
            get
            {
                return name;
            }
        }


        public AttributeComponent this[int index]
        {
            get
            {
                if (index >= 0 && index < values.Length)
                    return values[index];
                return null;
            }
        }


        public object Tag
        {
            get
            {
                return tag;
            }
            set
            {
                tag = value;
            }
        }

        public override int BufferSize
        {
            get
            {
                int l = buffer.Length;
                foreach (AttributeComponent ac in values)
                    l += ac.BufferSize;
                return l;
            }
        }


        public ObjectComponent(int iDescriptor)
        {
            descriptor = iDescriptor;
            index = 0;  Samples = 0;
        }



        public override string ToString()
        {
            string str = origin.ToString() + ":" + copy.ToString();
            if (name != null)
                str = ":" + name;
            return str;
        }




        public void InitObjectComponent(int attributes)
        {
            values = new AttributeComponent[attributes];
        }


        protected override void OnReadComponent(DataReader r)
        {
            origin = (int)r.ReadDataObject(DlisDataType.ORIGIN);
            copy = (byte)r.ReadByte();
            if ((descriptor & (int)ObjectComponents.Name) != 0)
                name = (string)r.ReadDataObject(DlisDataType.IDENT);
        }


        protected override void WriteAttributeComponent(int idescriptor, string iname)
        {
            origin = 0;
            copy = 0;
            name = iname;
        }


        protected override void WriteAttributeComponent(Measurement curve,
            int descriptor)
        {
            origin = 0;
            copy = 0;
            name = curve.Head.Name;

            return;
        }
        
        public int Count
        {
            get
            {
                return values.Length;
            }
        }

         
        public void WriteFileComponent(DataWriter w)
        {
            foreach (AttributeComponent ac in values)
                ac.WriteFileComponent(w);
        }

        public override void WriteComponent(DataWriter w)
        {
            w.WriteData(buffer);
            foreach (AttributeComponent ac in values)
                ac.WriteComponent(w);
        }

        public void AddValue(AttributeComponent newValue)
        {
            values[index] = newValue;
            index++;
        }

    }


    public class SetComponent : EFLRComponent
    {
        int index;
        [FlagsAttribute]
        public enum SetComponents { Count = 4, Name = 8, Type = 16 };
        string type;
        string name;
        bool selected;
        AttributeComponents template;
        ObjectComponents objects;

        public bool Selected
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
            }
        }

        public string Type
        {
            get
            {
                return type;
            }
        }

        public override int BufferSize
        {
            get
            {
                int l = buffer.Length;
                foreach (AttributeComponent ac in template)
                    l += ac.BufferSize;
                foreach (ObjectComponent oc in objects)
                    l += oc.BufferSize;
                return l;
            }
        }


        public ObjectComponents Objects
        {
            get
            {
                return objects;
            }
        }

        public AttributeComponents Template
        {
            get
            {
                return template;
            }
        }
        public SetComponent(int iDescriptor)
        {
            descriptor = iDescriptor;
            template = new AttributeComponents();
            objects = new ObjectComponents();
        }


        public override string ToString()
        {
            string str = type == null ? "" : type;
            if (name != null)
                str = str + ":" + name;
            return str;
        }


        public int GetTemplateColumnIndex(string strName)
        {
            return template.GetComponentIndex(strName);
        }


    

        public void BeforeAddNewComponent(EFLRComponent newComponent)
        {
            try
            {
                if (newComponent is AttributeComponent && objects.Count > 0)
                    ((AttributeComponent)newComponent).InitAttributeComponent(template[index++]);
            }
            catch (Exception e)
            {
                int i = 0;
            }
        }

        public void AfterAddNewComponent(EFLRComponent newComponent)
        {
            try
            {
                if (newComponent is ObjectComponent)
                {
                    ((ObjectComponent)newComponent).InitObjectComponent(template.VariantAttributes);
                    objects.Add((ObjectComponent)newComponent);
                    index = 0;
                }
                else
                {
                    //tempate
                    if (objects.Count == 0)
                        template.AddComponent((AttributeComponent)newComponent);
                    //objects
                    else
                        objects[objects.Count - 1].AddValue((AttributeComponent)newComponent);
                }

            }
            catch (Exception e)
            {

            }
        }


        protected override void OnReadComponent(DataReader r)
        {
            if ((descriptor & (int)SetComponents.Type) != 0)
                type = (string)r.ReadDataObject(DlisDataType.IDENT);
            if ((descriptor & (int)SetComponents.Name) != 0)
                name = (string)r.ReadDataObject(DlisDataType.IDENT);
            else
                name = null;
        }

        
        public void WriteFileComponent(DataWriter w)
        {
            byte tempval = (byte)descriptor;
            w.WriteDataObject(tempval, DlisDataType.USHORT);
            if ((descriptor & (int)SetComponents.Type) != 0)
                w.WriteDataObject(type, DlisDataType.IDENT);
            if ((descriptor & (int)SetComponents.Name) != 0)
                w.WriteDataObject(name, DlisDataType.IDENT);
        }

        
        protected override void WriteAttributeComponent(Measurement curve,
            int idescriptor)
        {
            /*
            if ((descriptor & (int)SetComponents.Type) != 0)
                type = (string)r.ReadDataObject(DlisDataType.IDENT);
            if ((descriptor & (int)SetComponents.Name) != 0)
                name = (string)r.ReadDataObject(DlisDataType.IDENT);
            else
                name = null;
             * */

            descriptor = idescriptor;
        }

        
        protected override void WriteAttributeComponent(int idescriptor, string name)
        {
            descriptor = idescriptor;
            type = name;
        }

        
        public bool isTemplate()
        {
            if (objects.Count == 0)
                return true;

            return false;
        }

        
        public bool isObjectHead()
        {
            if (objects.Count == 0)
                return true;

            return false;
        }
/*
        public OpenLS.LogFile.PlfFile.PlfEmbededObject CreatePlfEmbededObject(PlfFile.PlfFile PlfFile)
        {
            PlfFile.PlfEmbededObject PlfEO;
            if (name == null)
                PlfEO = new PlfEmbededObject("", type, null, false);
            else
                PlfEO = new PlfEmbededObject(name, type, null, false);
            MyDataWriter w = new MyDataWriter(false, BufferSize);
            WriteComponent(w);
            PlfFile.AddEmbededObject(w.GetBuffer(), PlfEO, true);
            return PlfEO;
        }
*/
        public override void WriteComponent(DataWriter w)
        {
            w.WriteData(buffer);
            foreach (AttributeComponent ac in template)
                ac.WriteComponent(w);
            foreach (ObjectComponent oc in objects)
                oc.WriteComponent(w);
        }

    }


    public class EFLRComponent
    {
        protected byte[] buffer;
        protected int descriptor;
        public static EFLRComponent CreateComponent(int iDescriptor)
        {
            ComponentRole cr = (ComponentRole)(iDescriptor & 0xe0);
            EFLRComponent newComponent = null;
            switch (cr)
            {
                case ComponentRole.ABSATR:
                case ComponentRole.ATTRIB:
                case ComponentRole.INVATR:
                    newComponent = new AttributeComponent(iDescriptor);
                    break;
                case ComponentRole.OBJECT:
                    newComponent = new ObjectComponent(iDescriptor);
                    break;
                case ComponentRole.RedundantSet:
                case ComponentRole.ReplacementSet:
                case ComponentRole.NormalSet:
                    newComponent = new SetComponent(iDescriptor);
                    break;
                default:
                    newComponent = null;
                    break;
            }
            return newComponent;
        }


        public static int[] GetIntArray(object ob)
        {
            if (ob is int)
            {
                int[] ia = new int[1];
                ia[0] = (int)ob;
                return ia;
            }
            if (ob is int[])
                return (int[])ob;
            return null;
        }

        public ComponentRole ComponentRole
        {
            get
            {
                return (ComponentRole)(descriptor & 0xe0);
            }
        }

        public virtual int BufferSize
        {
            get
            {
                return buffer.Length;
            }
        }

        public void ReadComponent(DataReader r)
        {
            int s = r.Position - 1;//one byte for descriptor
            OnReadComponent(r);
            int l = r.Position - s;
            buffer = new byte[l];
            byte[] from = r.GetBuffer();
            for (int i = 0; i < l; i++)
                buffer[i] = from[s++];

        }

        
        public void WriteComponent(int iDescriptor,
            string ilabel,
            int icount,
            DlisDataType itype,
            object iunits,
            object ithevalue)
        {
            WriteAttributeComponent(iDescriptor, ilabel, icount, itype, iunits, ithevalue);
        }

        
        protected virtual void WriteAttributeComponent(int iDescriptor,
            string ilabel,
            int icount,
            DlisDataType itype,
            object iunits,
            object ithevalue)
        {
        }

        
        public void WriteComponent(int descriptor, string name)
        {
            WriteAttributeComponent(descriptor, name);
        }

        
        protected virtual void WriteAttributeComponent(int descriptor, string name)
        {
        }

        
        public void WriteComponent(Measurement curve,
            int descriptor)
        {
            WriteAttributeComponent(curve, descriptor);
        }

        
        protected virtual void WriteAttributeComponent(Measurement curve,
            int iDescriptor)
        {
        }

        protected virtual void OnReadComponent(DataReader r)
        {

        }

        public virtual void WriteComponent(DataWriter w)
        {
            w.WriteData(buffer);
        }

    }


    public class AttributeComponents : List<AttributeComponent>
    {
        int variantAttributes;
        public int VariantAttributes
        {
            get
            {
                return variantAttributes;
            }
        }
        public AttributeComponent? GetComponent(string strName)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Label == strName)
                    return this[i];
            }
            return null;
        }
        public int GetComponentIndex(string strName)
        {
            for (int i = 0; i < Count; i++)
            {
                if (this[i].Label == strName)
                    return i;
            }
            return -1;
        }
        /// <summary>
        /// Adds an attributeComponent object to the collection
        /// </summary>
        /// <param name="attributeComponent"></param>
        public void AddComponent(AttributeComponent attributeComponent)
        {
            Add(attributeComponent);
            if (attributeComponent.ComponentRole != ComponentRole.INVATR)
                variantAttributes++;

        }
	
        /// <summary>
        /// Removes an attributeComponent object from the collection
        /// </summary>
        /// <param name="attributeComponent"></param>
        public void RemoveComponent(AttributeComponent attributeComponent)
        {
            if (Count <= 1)
                return;
            Remove(attributeComponent);
            if (attributeComponent.ComponentRole != ComponentRole.INVATR)
                variantAttributes--;
        }
    }


    public class ObjectComponents: List<ObjectComponent>
    {
        public ObjectComponent GetObjectComponent(string name)
        {
            foreach (ObjectComponent sc in this)
            {
                if (sc.Name == name)
                    return sc;
            }
            return null;
        }

    }


    public class SetComponents : List<SetComponent>
    {
        public SetComponent GetSetComponent(string type)
        {
            foreach (SetComponent sc in this)
            {
                if (sc.Type == type)
                    return sc;
            }
            return null;
        }

    }
}
