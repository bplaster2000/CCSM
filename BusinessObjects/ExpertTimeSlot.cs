using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for ExpertTimeSlot
/// /// </summary>
/// 
namespace CCSM.BusinessObjects
{
    public class ExpertTimeSlot
    {
        public ExpertTimeSlot()
        {   }

        private int expSlotId;
        public int ExpSlotId
        {
            get { return expSlotId; }
            set { expSlotId = value; }
        }
        private string searchKeywords;
        public string SearchKeywords
        {
            get { return searchKeywords; }
            set { searchKeywords = value; }
        }
        private string expSlotDesc;
        public string ExpSlotDesc
        {
            get { return expSlotDesc; }
            set { expSlotDesc = value; }
        }

        private string expSlotName;
        public string ExpSlotName
        {
            get { return expSlotName; }
            set { expSlotName = value; }
        }
        private string expSlotType;
        public string ExpSlotType
        {
            get { return expSlotType; }
            set { expSlotType = value; }
        }
        private DateTime expSlotDate;
        public DateTime ExpSlotDate
        {
            get { return expSlotDate; }
            set { expSlotDate = value; }
        }
        private int companyId;
        public int CompanyId
        {
            get { return companyId; }
            set { companyId = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.ExpSlotDate.ToString()).Append(" - ");
            sb.Append(this.ExpSlotName);
            sb.Append(", ");
            sb.Append(this.ExpSlotDesc);
            sb.Append(", ");
            sb.Append(this.ExpSlotType);
            return sb.ToString();

        }


    }
}