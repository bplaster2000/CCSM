using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for BaseUser
/// </summary>
namespace CCSM.BusinessObjects
{
    public class CUser
    {
        private CUser()
        {

        }
       // private BaseCompany company;
        public CUser(BaseCompany Company, string sEmail)
        {
            company = Company;
            email = sEmail;
        }
        public CUser(BaseCompany Company, int userId)
        {
            this.company = Company;
            this.userId = userId;

        }

       
        private string targetSegment;
        public string TargetSegment
        {
            get { return targetSegment; }
            set { targetSegment = value; }
        }

        private string externalSystemId;
        public string ExternalSystemId
        {
            get { return externalSystemId; }
            set { externalSystemId = value; }
        }
        private BaseCompany company;
        public BaseCompany Company
        {
            get { return company; }
            set { company = value; }
        }
        private int userId;
        public int UserId
        {
            get { return userId; }
            set { userId = value; }
        }
        private string textPhone;
        public string TextPhone
        {
            get { return textPhone; }
            set { textPhone = value; }
        }

        private string email;
        public string Email
        {
            get { return email; }
            set { email = value; }
        }
        private string username;
        public string Username
        {
            get { return username; }
            set { username = value; }
        }

        private string officePhone;
        public string OfficePhone
        {
            get { return officePhone; }
            set { officePhone = value; }
        }
        private string businessName;
        public string BusinessName
        {
            get { return businessName; }
            set { businessName = value; }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.Company.ToString());
            sb.Append(", ");
            sb.Append(this.Email);
            sb.Append(", ");
            sb.Append(this.OfficePhone);
            sb.Append(", ");
            sb.Append(this.TextPhone);
            sb.Append(", ");
            sb.Append(this.UserId);
            sb.Append(", ");
            sb.Append(this.Username);
            sb.Append(", ");
            return sb.ToString();
        }


    }
}