using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// SUbscription info for a comapany
/// </summary>
/// 
namespace CCSM.BusinessObjects
{
    [Serializable]
    public class Subscription
    {
        
        //only make the subscription by the user
        public Subscription(CUser inuser)
        {
            this.User = inuser;
        }
        //only make the subscription by the user who owns it
        private Subscription() { }

        private CUser user;
        public CUser User
        {
            get { return user; }
            set { user = value; }
        }

        private int subscId;
        public int SubscId
        {
            get { return subscId; }
            set { subscId = value; }
        }

        private DateTime subscStart;
        public DateTime SubscStart
        {
            get { return subscStart; }
            set { subscStart = value; }
        }

        private DateTime subscEnd;
        public DateTime SubscEnd
        {
            get { return subscEnd; }
            set { subscEnd = value; }
        }
        private string subscrProductMsg;
        public string SubscrProductMsg
        {
            get { return subscrProductMsg; }
            set { subscrProductMsg = value; }
        }
        private string currentStage;
        public string CurrentStage
        {
            get { return currentStage; }
            set { currentStage = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.CurrentStage);
            sb.Append(", ");
            sb.Append(this.SubscStart.ToString());
            sb.Append(", ");
            sb.Append(this.SubscStart.ToString());
            sb.Append(", ");
            sb.Append(this.SubscrProductMsg);
            sb.Append(", ");
            sb.Append(this.SubscId);
            return sb.ToString();
        }


    }
}