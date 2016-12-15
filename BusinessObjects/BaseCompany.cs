using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// For companies that want a customization, 
/// they would also extend this class BaseCompany with the company name
/// </summary>
/// 
namespace CCSM.BusinessObjects
{
    public abstract class BaseCompany
    {

        private string companyGUID;
        private BaseCompany()
        {
            // should not create a BaseCompany with out a Company GUID.   
            //Also create a specialization of BaseCompany is how we would extend to customized clients

        }

        //MainContactPhone
        //MainContactEmailId
        // SupportContactPhone
        // CSMContactPhone
        private string mainContactPhone;
        public string MainContactPhone
        {
            get { return mainContactPhone; }
            set { mainContactPhone = value; }
        }


        public BaseCompany(int companyId)
        {
            this.companyId = companyId; 
        }

        public BaseCompany(string CompanyGUID)
        {
            companyGUID = CompanyGUID;
        }

        public string CompanyGUID
        {
            get { return companyGUID; }
            set { companyGUID = value; }
        }

        private int companyId;
        public int CompanyId
        {
            get { return companyId; }
            set { companyId = value; }
        }

        private string companyName;
        public string CompanyName
        {
            get { return companyName; }
            set { companyName = value; }
        }

        private string mainContactEmail;
        public string MainContactEmail
        {
            get { return mainContactEmail; }
            set { mainContactEmail = value; }
        }
        private string csmContactEmail;
        public string CsmContactEmail
        {
            get { return csmContactEmail; }
            set { csmContactEmail = value; }
        }
        private string supportContactPhone;
        public string SupportContactPhone
        {
            get { return supportContactPhone; }
            set { supportContactPhone = value; }
        }
        private string supportContactEmail;
        public string SupportContactEmail
        {
            get { return supportContactEmail; }
            set { supportContactEmail = value; }
        }
        private string productsSoldDesc;
        public string ProductsSoldDesc
        {
            get { return productsSoldDesc; }
            set { productsSoldDesc = value; }
        }

        private Integrations intgr;
        public Integrations Intgr
        {
            get { return intgr; }
            set { intgr = value; }
        }



        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.companyGUID);
            sb.Append(", ");
            sb.Append(this.companyId);
            sb.Append(", ");
            sb.Append(this.companyName);
           
            return sb.ToString();

        }

       

    }
}