using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSM.BusinessObjects
{
    public class SupportQuestion
    {
        /*
       [SupportQuestionId]
       INT NOT NULL PRIMARY KEY, 
   [SupportQuestion1] NVARCHAR(50) NULL, 
   [SupportQuestion2] NVARCHAR(50) NULL, 
   [SupportQuestion3] NVARCHAR(50) NULL, 
   [SupportQuestion4] NVARCHAR(50) NULL, 
   [SupportQuestion5] NVARCHAR(50) NULL, 
   [CompanyId]
       INT NULL
       */

        public SupportQuestion() { }

        private int supportQuestionId;
        public int SupportQuestionId
        {
            get { return supportQuestionId; }
            set { supportQuestionId = value; }
        }
        private string supportQuestion1;
        public string SupportQuestion1
        {
            get { return supportQuestion1; }
            set { supportQuestion1 = value; }
        }

        private string supportQuestion2;
        public string SupportQuestion2
        {
            get { return supportQuestion2; }
            set { supportQuestion2 = value; }
        }

        private string supportQuestion3;
        public string SupportQuestion3
        {
            get { return supportQuestion3; }
            set { supportQuestion3 = value; }
        }

        private string supportQuestion4;
        public string SupportQuestion4
        {
            get { return supportQuestion4; }
            set { supportQuestion4 = value; }
        }

        private string supportQuestion5;
        public string SupportQuestion5
        {
            get { return supportQuestion5; }
            set { supportQuestion5 = value; }
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
            sb.Append(this.SupportQuestion1);
            sb.Append(", ");
            sb.Append(this.SupportQuestion2);
            sb.Append(", ");
            sb.Append(this.SupportQuestion3);
            sb.Append(", ");
            sb.Append(this.SupportQuestion4);
            sb.Append(", ");
            sb.Append(this.SupportQuestion5);
            return sb.ToString();
        }

    }
}
