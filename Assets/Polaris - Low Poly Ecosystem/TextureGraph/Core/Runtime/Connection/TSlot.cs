using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    public class TSlot
    {
        private int id;
        public int Id
        {
            get
            {
                return id;
            }
        }

        private TSlotType slotType;
        public TSlotType SlotType
        {
            get
            {
                return slotType;
            }
        }

        private TSlotDataType dataType;
        public TSlotDataType DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name ?? string.Empty;
            }
        }

        public TSlot(string name, TSlotType slotType, TSlotDataType dataType, int id)
        {
            this.name = name;
            this.slotType = slotType;
            this.dataType = dataType;
            this.id = id;
        }
    }
}
