using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSM.BusinessObjects
{
    public class SupportLink
    {
        public SupportLink() { }

        private int supportlinkId;
        public int SupportlinkId
        {
            get { return supportlinkId; }
            set { supportlinkId = value; }
        }
        private string supportlinkName;
        public string SupportlinkName
        {
            get { return supportlinkName; }
            set { supportlinkName = value; }
        }
        private string supportlink;
        public string Supportlink
        {
            get { return supportlink; }
            set { supportlink = value; }
        }
        private string supportlinkImage;
        public string SupportlinkImage
        {
            get { return supportlinkImage; }
            set { supportlinkImage = value; }
        }
        private string searchKeywords;
        public string SearchKeywords
        {
            get { return searchKeywords; }
            set { searchKeywords = value; }
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
            sb.Append(this.CompanyId);
            sb.Append(", ");
            sb.Append(this.Supportlink);
            sb.Append(", ");
            sb.Append(this.SupportlinkId);
            sb.Append(", ");
            sb.Append(this.SupportlinkName);
            sb.Append(", ");
            sb.Append(this.SearchKeywords);
            return sb.ToString();
        }


    }
}
