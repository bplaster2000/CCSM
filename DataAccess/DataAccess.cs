using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CCSM.BusinessObjects;
using System.Collections;
//using Microsoft.Extensions.Logging;

/// <summary>
/// Summary description for DataAccess
/// </summary>
namespace CCSM.DataAccess
{
    [Serializable]
    public class DataAccess
    {
        private DataAccess()
        {
            //always create DataAccess with a company in mind, and sometimes a user
        }

        /****************************************************************************************************************************
         * Constructors
         * *************************************************************************************************************************/
        private BaseCompany company;
        private CUser user;
        private DatabaseUtils db = new DatabaseUtils();
        //private readonly ILogger logger;

        public DataAccess(BaseCompany company)
        {
            //always create DataAccess with a company in mind  
            this.company = getCompanyInfo(company);
            //logger.LogError("test");
        }
        //access the data by company and user
        public DataAccess(BaseCompany company, CUser user)
        {
            //always create DataAccess with a company in mind
            this.company = getCompanyInfo(company);
            this.user = getUserInfo(user);
        }

        //get info for constructor
        //need an id to play
        //private so that it is internal to this constructor
        private BaseCompany getCompanyInfo(BaseCompany company)
        {
            if(String.IsNullOrEmpty(company.CompanyGUID) & company.CompanyId == 0)
            {
                throw new Exception("We need a GUID or a companyId to load the company info");
            }
            return db.getCompanyByGUID(company);
            

            
        }

        //need an id to play
        //private so that it is internal to this constructor
        private CUser getUserInfo(CUser user)
        {
            if (user.UserId != 0)
                return db.getUserById(user);
            else
                return db.getUserByEmail(user);
        }

        /* what to impletement the ************************************************************************************************
         * Step 1: Confirm Subscription
         * 
         *  for Onboarding
         * *************************************************************************************************************************/

      
        /// <summary>
        /// get the user by the email that they tell use
        /// </summary>
        /// <param name="email">the email address to find the user</param>
        /// <returns>a populated user object</returns>
        public CUser getUserByEmail(string email)
        {
            if (user == null)
                user = new CUser(company, email);

            return db.getUserByEmail(user);
        }


        /// <summary>
        /// get the user by the internal ID -used internally
        /// </summary>
        /// <param name="id">internal DB id</param>
        /// <returns>a populated user object</returns>
        public CUser getUserById(int id)
        {
            if (user == null)
                user = new CUser(company, id);
            return db.getUserById(user);
        }        
        
        /// <summary>
        /// Get the subscription for the current user in this DataAccess object 
        /// </summary>
        /// <returns>the users subscription </returns>
        public Subscription getSubscription()
        {
            if (user == null) throw new Exception("Need a user to find the subscription");
            return this.user.Subscription;
        }
        

        ///<summary>Create a CTA with handling for the Bot</summary>
        ///<param name="userinput">comments from the user through the bot</param>
        ///<param name="title">Title of the case or the CTA </param>
        ///<returns>Case or CTA number for the customer reference</returns>
        private string createCase(string userinput, string title )
        {
            //decide in here if we want to make a support CASE or a Call To Action

            //local variables
            ArrayList messages = new ArrayList();
            bool success;
            string caseNum = String.Empty;           

            //basic error handling
            if (user == null) throw new Exception("Need a user and/or email to create a case");

            //create the case
            SalesforceUtils su = new SalesforceUtils(company);
            caseNum =  su.createCase(title, userinput, user, out messages, out success);

            //something went wrong! 
           if(!success) emailHelpMe(messages, userinput, user);

            return caseNum;
        }

        ///<summary>Create a CTA with handling for the Bot </summary>
        ///<param name="userinput">comments from the user through the bot</param>
        ///<param name="title">Title of the CTA </param>
        /// <returns>Case or CTA number for the customer reference</returns>
        private string createCTA(string userinput, string title)
        {
            //decide in here if we want to make a support CASE or a Call To Action

            //local variables
            ArrayList messages = new ArrayList();
            bool success;
            string caseNum = String.Empty;

            //basic error handling
            if (user == null) throw new Exception("Need a user and/or email to create a CTA");

            //create the case
            SalesforceUtils su = new SalesforceUtils(company);
            caseNum = su.createCTA(title, userinput, user, out messages, out success);

            //something went wrong! 
            if (!success) emailHelpMe(messages, userinput, user);

            return caseNum;
        }

        /// <summary>
        /// if we can't create a case, we'll need to email someone
        /// </summary>
        /// <param name="messages">these should be the error messages</param>
        /// <param name="userinput">this is what the user asked us</param>
        /// <param name="user">this is the user object</param>
        private void emailHelpMe(ArrayList messages, string userinput, CUser user)
        {
            //if case fails, we still need to send an email to someone, and let the bot move one
        }
        /// <summary>
        /// Call this from the Bot.    Create a Case for no subscription
        /// 1.1.2 - Corresponds to this section of the design doc 
        /// </summary>
        /// <param name="userinput">this is what the user asks</param>
        /// <returns>a reference number to give back to the user</returns>
        public string createNoSubscCase(string userinput)
        {
            return createCase(userinput, "VCSM: Engage CSM to search why the user subscription is not setup.");
        }
        /// <summary>
        /// Create a case to make a new subscription for the user
        /// </summary>
        /// <param name="userinput">this is what the user asks</param>
        /// <returns>a reference number to give back to the user</returns>
        public string createNewSubscCase(string userinput)
        {
            return createCase(userinput, "VCSM: User request to CREATE a subscription.");
        }
        /// <summary>
        /// Create a case to renew thier subscription for the user
        /// </summary>
        /// <param name="userinput">this is what the user asks</param>
        /// <returns>a reference number to give back to the user</returns>
        public string createReNewSubscCase(string userinput)
        {
            return createCase(userinput, "VCSM: User request to RENEW a subscription.");
        }
        /// <summary>
        /// Create a case to ADD to thier subscription for the user
        /// </summary>
        /// <param name="userinput">this is what the user asks</param>
        /// <returns>a reference number to give back to the user</returns>
        public string createAddToSubscCase(string userinput)
        {
            return createCase(userinput, "VCSM: User request to ADD TO a subscription.");
        }


        /* what to impletement the *************************************************************************************************
         * Step 2: Register for Tech Support
         * 
         *  for Onboarding
         * *************************************************************************************************************************/

        /// <summary>
        /// Create a case for adding users to access support
        /// </summary>
        /// <param name="userinput">this is what the user asks</param>
        /// <returns>a reference number to give back to the user</returns>
        public string createAddSuppUsersCase(String userinput)
        {
           return createCase(userinput,"VCSM: Please enlist users in support");            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userinput"></param>
        /// <returns></returns>
        public string createTechnicalSupportCase(String userinput, SupportQuestion questions)
        {
           return createCase(userinput + questions.ToString(),"VCSM: User requested a Technical Support case");
        }
        /// <summary>
        /// Get the options for the Webscreens to create a CTA in Gainsight
        /// </summary>
        /// <param name="priorities">Dictionary of key/value pairs</param>
        /// <param name="stages">Dictionary of key/value pairs</param>
        /// <param name="reasons">Dictionary of key/value pairs</param>
        /// <param name="users">Dictionary of key/value pairs</param>
        /// <param name="CTATypes">Dictionary of key/value pairs</param>
        /// <param name="playbooks">Dictionary of key/value pairs</param>
        /// <param name="messages">if this doesn't work, these are messages why to handle</param>
        public void GetCTAOptions(
            out Dictionary<string, string> priorities,
            out Dictionary<string, string> stages,
            out Dictionary<string, string> reasons,
            out Dictionary<string, string> users,
            out Dictionary<string, string> CTATypes,
            out Dictionary<string, string> playbooks,
            out ArrayList messages)
        {
            SalesforceUtils su = new SalesforceUtils(company);
            su.GetCTAOptions(out priorities, out stages, out reasons,out users, out CTATypes, out playbooks, out messages);
        }

        /// <summary>
        /// Based on the email, look in salesforce for a selection of account records that match. 
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="messages">if this doesn't work, these are messages why to handle</param>
        /// <returns>Dictionary of key/value pairs</returns>
        public Dictionary<string, string> SFGetAccountIDFromEmail(string email, out ArrayList messages)
        {
            SalesforceUtils su = new SalesforceUtils(company);
            return su.SFGetAccountsFromEmail(email, out messages);            
        }
        /// <summary>
        /// get the link to the support signup sight
        /// </summary>
        /// <returns>the link</returns>
        public string getSuppSignupLink()
        {
            if (company.Intgr == null)
                throw new Exception("Integrations are not configured yet!");
            else
                return company.Intgr.SupportSignupLink;           
        }

        /// <summary>
        /// get the link to the support signup sight
        /// </summary>
        /// <returns>the link</returns>
        public string getSuppSignupLinkInst()
        {
            if (company.Intgr == null)
                throw new Exception("Integrations are not configured yet!");
            else
                return company.Intgr.SupportSignupLinkInst;
        }

     /* what to impletement the *************************************************************************************************
     * Step 3: Recommend Training
     * 
     *  for Onboarding
     * *************************************************************************************************************************/

        /// <summary>
        /// Get the introductory training links for the user
        /// </summary>
        /// <returns>names and links</returns>
        public TrainingLink[] getIntroTraining()
        {
            return db.getInitialTrainingLinks(company);
        }

        /// <summary>
        /// get the first training link that a user shoudl get started with
        /// </summary>
        /// <returns>a link or null if thier is not one setup</returns>        
        public TrainingLink getFirstIntroTraining()
        {
            TrainingLink tl =  db.getFirstTrainingLink(company);
            if (tl != null)
                return tl;
            else
                return new TrainingLink();
        }

        /// <summary>
        /// find training links based on the keywords
        /// </summary>
        /// <param name="keywordsByToken">key words to search for separated by a comma</param>
        /// <returns></returns>
        public TrainingLink[] getUserSearchedTrainingLinks(string[] keywordsByToken)
        {
            return db.getUserSearchedTrainingLinks(company, keywordsByToken);
        }

        /// <summary>
        /// get keywords to search for training by so they can find stuff
        /// </summary>
        /// <returns>a string array of the keywords</returns>
        public string[] getTrainingKeywords()
        {
            return db.getTrainingKeywords(company);
        }


        public ExpertTimeSlot[] getInstructorLedTraining()
        {
            return db.getInstructorLedTraining(company);
        }

        /// <summary>
        /// Create a case to ADD to thier subscription for the user
        /// </summary>
        /// <param name="userinput">this is what the user asks</param>
        /// <returns>a reference number to give back to the user</returns>
        public string createSignupTrainingCase(string userinput, ExpertTimeSlot trainingCourse)
        {
            return createCase(userinput + "  " +trainingCourse.ToString(), "VCSM: The user request to sign them up a training course");
        }


        /* what to impletement the *************************************************************************************************
      * Step 4: Ask a support question
      * 
      *  for Onboarding
      * *************************************************************************************************************************/


        /// <summary>
        /// send in Keywords using the token to search both top 10 list and knowledge base
        /// </summary>
        /// <param name="keywordsByToken">keywords to search for</param>
        /// <returns></returns>
        public SupportLink[] getUserSearchedSupport(string [] keywordsByToken)
        {
            ArrayList links = new ArrayList();

            SupportLink[] sl = db.getUserSearchedSupportLinks(company, keywordsByToken);

            if(sl.Length > 0)
                links.AddRange(sl);
            
            //links.AddRange(  ---salesforce KB search )
            return (SupportLink[])links.ToArray(typeof(SupportLink));            
        }
        /// <summary>
        /// Get the keywords to find support links
        /// </summary>
        /// <returns></returns>
        public string[] getSupportKeywords()
        {
            return db.getSupportKeywords(company);
        }

        /// <summary>
        /// get all of the support links
        /// </summary>
        /// <returns></returns>
        public SupportLink[] getSupportLinks()
        {
            return db.getSupportLinks(company);
        }


        /// <summary>
        /// get the questions to ask the user if they want to open a support ticket
        /// </summary>
        /// <returns></returns>
        public SupportQuestion getSupportQuestions()
        {
            return db.getSupportQuestions(company);
        }


        /// <summary>
        /// send in Keywords using the token of search knowledge base
        /// </summary>
        /// <param name="keywordsByToken"></param>
        /// <returns></returns>
        private SupportLink[] searchFromTop10Issues(string[] keywordsByToken)
        {
            return db.getUserSearchedSupportLinks(company, keywordsByToken);
        }
        /// <summary>
        /// send in Keywords using the token of search knowledge base
        /// </summary>
        /// <param name="keywordsByToken"></param>
        /// <returns></returns>
        private SupportLink[] searchFromKB(string[] keywordsByToken)
        {
            //this one should search from Salesforce Knowledge ARticles
            return null; // db.getUserSearchedSupportLinks(company, keywordsByToken);
        }

        /* what to implement the **********************************************************************************************
    * Step 5: Ask an Expert
    * 
    *  for Onboarding
    * *************************************************************************************************************************/

        //
        /// <summary>
        /// get the service offerings to pick from
        /// </summary>
        /// <returns></returns>
        public ServiceOffLink[] getServiceOfferings()
        {
            return db.getServiceOfferings(company);
        }

        /// <summary>
        /// create a ticket for a service offering
        /// </summary>
        /// <param name="userinput"></param>
        /// <returns></returns>
        public string createServiceOfferingCase(string userinput, ServiceOffLink servoffer)
        {
            return createCase(userinput + servoffer.ToString(), "VCSM: User request information on a service offering.");
        }

        /*
         * //send in Keywords using the token of ,
        public ServiceOffLink[] getUserSearchedServiceOffLink(string[] keywordsByToken)
        {
            return db.getUserSearchedServiceOffLink(company, keywordsByToken);
        }
        */

        /// <summary>
        /// get the expert timeslots from the database that they can sign up for
        /// </summary>
        /// <returns></returns>
        public ExpertTimeSlot[] getExpertTimeSlots()
        {
            return db.getExpertTimeSlots(company);
        }

        /// <summary>
        /// get the expert timeslots from the database that they can sign up for by keywords
        /// </summary>
        /// <param name="keywordsByToken"></param>
        /// <returns></returns>
        public ExpertTimeSlot[] getUserSearchedExpertSlot(string[] keywordsByToken)
        {
            return db.getUserSearchedExpertSlot(company, keywordsByToken);
        }


        /// <summary>
        /// create a case to assign the slot
        /// </summary>
        /// <param name="userinput"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public string createAssignExpertSlotCase(string userinput, ExpertTimeSlot slot)
        {
            return createCase(userinput + slot.ToString(), "VCSM: User request sign up for on a expert slot.");
        }

    }

}