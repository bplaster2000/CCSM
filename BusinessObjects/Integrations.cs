using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCSM.BusinessObjects
{
    public class Integrations
    {

        private int integrationId;
        public int IntegrationId
        {
            get { return integrationId; }
            set { integrationId = value; }
        }

        private int companyId;
        public int CompanyId
        {
            get { return companyId; }
            set { companyId = value; }
        }
        private string supportSignupLink;
        public string SupportSignupLink
        {
            get { return supportSignupLink; }
            set { supportSignupLink = value; }
        }
        private string supportSignupLinkInst;
        public string SupportSignupLinkInst
        {
            get { return supportSignupLinkInst; }
            set { supportSignupLinkInst = value; }
        }
        private string emailNotifications;
        public string EmailNotifications
        {
            get { return emailNotifications; }
            set { emailNotifications = value; }
        }
        private string sfuser;
        public string SFuser
        {
            get { return sfuser; }
            set { sfuser = value; }
        }

        private string sfpwd;
        public string SFpwd
        {
            get { return sfpwd; }
            set { sfpwd = value; }
        }
        private string sfToken;
        public string SFToken
        {
            get { return sfToken; }
            set { sfToken = value; }
        }
        private string sfAssignUser;
        public string SfAssignUser
        {
            get { return sfAssignUser; }
            set { sfAssignUser = value; }
        }

        private string gainsightCTAPlaybook;
        public string GainsightCTAPlaybook
        {
            get { return gainsightCTAPlaybook; }
            set { gainsightCTAPlaybook = value; }
        }

        private string gainsightCTAPriority;
        public string GainsightCTAPriority
        {
            get { return gainsightCTAPriority; }
            set { gainsightCTAPriority = value; }
        }
        private string gainsightCTAStatus;
        public string GainsightCTAStatus
        {
            get { return gainsightCTAStatus; }
            set { gainsightCTAStatus = value; }
        }

        private string gainsightCTAReason;
        public string GainsightCTAReason
        {
            get { return gainsightCTAReason; }
            set { gainsightCTAReason = value; }
        }

        private string gainsightCTAType;
        public string GainsightCTAType
        {
            get { return gainsightCTAType; }
            set { gainsightCTAType = value; }
        }

        private string gainsightCTAUserId;
        public string GainsightCTAUserId
        {
            get { return gainsightCTAUserId; }
            set { gainsightCTAUserId = value; }
        }



    }
}
