using System;
using System.Net;
using System.Web.Services.Protocols;
using CCSM.BusinessObjects;
using CCSM.SFAccess;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Summary description for SalesforceUtils
/// </summary>

namespace CCSM.DataAccess
{
    public class SalesforceUtils
    {
       private SforceService binding;
        string username;
        string password;
        Integrations intgr;
               
        private BaseCompany company;
        public SalesforceUtils(BaseCompany company)
        {

            //set this to use the new updated security protocol to make salesforce happy
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls; // comparable to modern browsers
            
            this.company = company;
            this.intgr = company.Intgr;

            //if the company doesn't have basic SF access info
            if (intgr == null || String.IsNullOrEmpty(intgr.SFuser))
                throw new Exception("You need to Setup SF access!");

            username = intgr.SFuser;
            password = intgr.SFpwd + intgr.SFToken;
        }
               

        public void GetCTAOptions(
            out Dictionary<string,string> priorities,
            out Dictionary<string, string> stages,
            out Dictionary<string, string> reasons,
            out Dictionary<string, string> users,
            out Dictionary<string, string> CTATypes,
            out Dictionary<string, string> playbooks,
            out ArrayList messages)
        {
            messages = new ArrayList();


            if (!login(out messages))
            {
                //failed to login
                priorities = null;
                stages = null;
                reasons = null;
                users = null;
                CTATypes = null;
                playbooks = null;
                return;
            }

            priorities = SFQueryGSPicklist("Alert Severity", out messages);
            stages = SFQueryGSPicklist("Alert Status", out messages);
            reasons = SFQueryGSPicklist("Alert Reason", out messages);
            users = SFQueryUsers(out messages);
            CTATypes = SFQueryGSTypes(out messages);
            playbooks = SFQueryGSPlaybooks(out messages);

            logout();
        }

        private Dictionary<string, string> SFQueryGSPicklist(string pickListCategory, out ArrayList messages)
        {
            return SFQueryPairs("SELECT Id, Name FROM JBCXM__PickList__c WHERE JBCXM__Category__c = '" + pickListCategory + "' ORDER BY Name ASC NULLS FIRST", out messages);
        }
        private Dictionary<string, string> SFQueryUsers(out ArrayList messages)
        {
            return SFQueryPairs("SELECT Id,Name FROM User ORDER BY Name ASC NULLS FIRST", out messages);
        }
        private Dictionary<string, string> SFQueryGSTypes(out ArrayList messages)
        {            
            return SFQueryPairs("SELECT Id, Name FROM JBCXM__CTATypes__c ORDER BY Name ASC NULLS FIRST", out messages);
        }
        private Dictionary<string, string> SFQueryGSPlaybooks(out ArrayList messages)
        {            
            return SFQueryPairs("SELECT Id,Name FROM JBCXM__Playbook__c WHERE JBCXM__Tasks_Count__c != null ORDER BY Name ASC NULLS FIRST", out messages);
        }

        /// <summary>
        /// get the Salesforce Accounts from the contact email address
        /// </summary>
        /// <param name="email">email address in a Contact record to find the AccountIds to pick from</param>
        /// <param name="messages"></param>
        /// <returns>a dict of id, name of the accounts</returns>
        public Dictionary<string, string> SFGetAccountsFromEmail(string email, out ArrayList messages)
        {
            if (!login(out messages)) return null;

            ArrayList AcctIds = SFQueryIds("SELECT AccountId FROM Contact WHERE Email = '" + email + "'", out messages);
            String AcctIdStr = buildAccounts(AcctIds);

            //now search for accounts from the contacdt email to select
            Dictionary<string, string> Accounts = SFQueryPairs("SELECT Id,Name FROM Account where Id in (" + AcctIdStr + ") ORDER BY Name ASC NULLS FIRST", out messages);

            logout();
            return Accounts;
        }
        /// <summary>
        /// build the account list to query later
        /// </summary>
        /// <param name="AcctIds"></param>
        /// <returns></returns>
        private string buildAccounts(ArrayList AcctIds)
        {
            StringBuilder sb = new StringBuilder(" "); //add one in case it is empty
            foreach(string s in AcctIds)
            {
                sb.Append("'").Append(s).Append("'").Append(",");
            }
            return sb.ToString().Remove(sb.ToString().Length - 1); //remove last comma

        }

        private Dictionary<string, string> SFQueryNew_VCSM_CTA(out ArrayList messages)
        {
            return SFQueryPairs("SELECT Id,Name FROM JBCXM__Playbook__c WHERE JBCXM__Tasks_Count__c != null ORDER BY Name ASC NULLS FIRST", out messages);
        }


        private ArrayList SFQueryIds(string SFQuery, out ArrayList messages)
        {

            messages = new ArrayList();
            ArrayList ids = new ArrayList();

            try
            {
                QueryResult qr = null;
                binding.QueryOptionsValue = new QueryOptions();
                binding.QueryOptionsValue.batchSize = 250;
                binding.QueryOptionsValue.batchSizeSpecified = true;

                qr = binding.query(SFQuery);

                bool done = false;
                int loopCount = 0;
                while (!done)
                {
                    Console.WriteLine("\nRecords in results set " +
                        Convert.ToString(loopCount++)
                            + " - ");
                    // Process the query results
                    if (qr.records == null) return ids; //return empty

                    for (int i = 0; i < qr.records.Length; i++)
                    {
                        sObject con = qr.records[i];
                        ids.Add(con.Any[0].InnerText);
                    }

                    if (qr.done)
                        done = true;
                    else
                        qr = binding.queryMore(qr.queryLocator);
                }
            }
            catch (SoapException e)
            {
                messages.Add("An unexpected error has occurred: " + e.Message +
                    " Stack trace: " + e.StackTrace);
            }
            Console.WriteLine("\nQuery execution completed.");

            return ids;

        }
        private Dictionary<string, string> SFQueryPairs(string SFQuery, out ArrayList messages)
        {         

            messages = new ArrayList();
            Dictionary<string, string> dic = new Dictionary<string, string>();

            try
              {
                QueryResult qr = null;
                binding.QueryOptionsValue = new QueryOptions();
                binding.QueryOptionsValue.batchSize = 250;
                binding.QueryOptionsValue.batchSizeSpecified = true;

                qr = binding.query(SFQuery);

                bool done = false;
                int loopCount = 0;
                while (!done)
                {
                    Console.WriteLine("\nRecords in results set " +
                        Convert.ToString(loopCount++)
                            + " - ");
                    // Process the query results
                    if (qr.records == null) return dic; ; //return empty
                    for (int i = 0; i < qr.records.Length; i++)
                    {
                        sObject con = qr.records[i];
                        dic.Add(con.Any[0].InnerText, con.Any[1].InnerText);
                       
                    }

                    if (qr.done)
                        done = true;
                    else
                        qr = binding.queryMore(qr.queryLocator);
                }
            }
            catch (SoapException e)
            {
                messages.Add("An unexpected error has occurred: " + e.Message +
                    " Stack trace: " + e.StackTrace);
            }
            Console.WriteLine("\nQuery execution completed.");

            return dic;

        }

        


        public string createCTA(string name,
           string comments,
           CUser user, out ArrayList messages, out bool success)
        {
            if (user.Company == null) throw new Exception("Can't create a CTA without a company and an Integration");
            if(user.Company.Intgr == null) throw new Exception("Can't create a CTA without a company and an Integration");
            if(user.Company.Intgr.GainsightCTAStatus == null) throw new Exception("Can't create a CTA without a Gainsight Info setup!");
            if (user== null || user.ExternalSystemId == null) throw new Exception("Can't create a CTA without a salesforce Account Id on the User record!");

            Integrations integrations = user.Company.Intgr;

            //set up returns
            success = false;
            messages = new ArrayList();
            
            //the id to return
            string idcreated = "";

            //first login
            if (!login(out messages))
            {
                success = false;

                //throw new Exception("Could not login to Salesforce");
            }
                       
            try
            {
                // Create a new sObject of type Contact
                // and fill out its fields.
                sObject CTA = new SFAccess.sObject();
                System.Xml.XmlElement[] CTAFields = new System.Xml.XmlElement[12];

                // Create the ticket's fields
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                CTAFields[0] = doc.CreateElement("JBCXM__Account__c");
                CTAFields[0].InnerText = user.ExternalSystemId;
                CTAFields[1] = doc.CreateElement("JBCXM__Assignee__c");
                CTAFields[1].InnerText = integrations.GainsightCTAUserId;// "00541000001LJxYAAW"; //------------------------------------me
                CTAFields[2] = doc.CreateElement("JBCXM__DueDate__c");
                CTAFields[2].InnerText = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
                CTAFields[3] = doc.CreateElement("JBCXM__Priority__c"); //Alert Severity
                CTAFields[3].InnerText = integrations.GainsightCTAPriority;// "a0D41000001qZtYEAU"; //-------------------------------------high priority
                CTAFields[4] = doc.CreateElement("JBCXM__Stage__c");//Alert STatus
                CTAFields[4].InnerText = integrations.GainsightCTAStatus;// "a0D41000001qZsCEAU";//---------------- stands for Open - this is like a status
                CTAFields[5] = doc.CreateElement("JBCXM__Type__c");
                CTAFields[5].InnerText = integrations.GainsightCTAType;// "a1141000000cVEcAAM";//--------------------------------------what is type id ; 
                CTAFields[7] = doc.CreateElement("JBCXM__Reason__c");
                CTAFields[7].InnerText = integrations.GainsightCTAReason;// "a0D41000001qZsIEAU";
                CTAFields[8] = doc.CreateElement("JBCXM__Comments__c");
                CTAFields[8].InnerText = comments;//--------------------------------------
                CTAFields[9] = doc.CreateElement("Name");
                CTAFields[9].InnerText = name;
                CTAFields[10] = doc.CreateElement("JBCXM__CreatedDate__c");
                CTAFields[10].InnerText = DateTime.Now.ToString("yyyy-MM-dd"); 
                CTAFields[11] = doc.CreateElement("JBCXM__Playbook__c");
                CTAFields[11].InnerText = integrations.GainsightCTAPlaybook;// "a0d41000000inGeAAI";
                
                //field to set          picklist Category      other  
                //JBCXM__Priority__c    Alert Severity
                //JBCXM__Stage__c       Alert Status
                //JBCXM__Reason__c      Alert Reason
                //JBCXM__TypeName__c                           ? different object - type of CTA?  event or trial
                //JBCXM__Account__c                            from account object
                //JBCXM__Assignee__c                           this is from a user
                //JBCXM__DueDate__c                            user input
                //JBCXM__Comments__c                           user input


                CTA.type = "JBCXM__CTA__c";
                CTA.Any = CTAFields;

                // Add this sObject to an array
                sObject[] CTAList = new sObject[1];
                CTAList[0] = CTA;

                // Make a create call and pass it the array of sObjects 
                SaveResult[] results = binding.create(CTAList);
                // Iterate through the results list
                // and write the ID of the new sObject
                // or the errors if the object creation failed.
                // In this case, we only have one result
                // since we created one ticket.
                for (int j = 0; j < results.Length; j++)
                {
                    if (results[j].success)
                    {
                        messages.Add("\nA CTA was created with an ID of: "
                                        + results[j].id);
                        idcreated = results[j].id;
                    }
                    else
                    {
                        // There were errors during the create call,
                        // go through the errors array and write
                        // them to the console
                        for (int i = 0; i < results[j].errors.Length; i++)
                        {
                            Error err = results[j].errors[i];
                            messages.Add("Errors were found on item " + j.ToString());
                            messages.Add("Error code is: " + err.statusCode.ToString());
                            messages.Add("Error message: " + err.message);
                        }
                    }
                }
            }
            catch (SoapException e)
            {
                messages.Add("An unexpected error has occurred: " + e.Message +
                    " Stack trace: " + e.StackTrace);
            }
            //now logout
            logout();
            return idcreated;



        }

        public string createCase(string Subject,
           string Description,
           CUser user, out ArrayList messages, out bool success)
        {
            

            string ph = user.TextPhone; //check which phone to use
            if (String.IsNullOrEmpty(ph))  ph = user.OfficePhone;
            
            return createCase(Subject, Description, user.BusinessName, user.Email, user.Username, ph, user.ExternalSystemId, out messages, out success);           
        }

        private string createCase(string Subject,
           string Description,
           string SuppliedCompany,
           string SuppliedEmail,
           string SuppliedName,
           string SuppliedPhone, 
           string accountId, out ArrayList messages, out bool success)
        {

            string idcreated = "";
            messages = new ArrayList();
            success = false;

            //first login
            if (!login(out messages))
            {            
                return "";
            }

            int hasAcctId = 0;
            if (!String.IsNullOrEmpty(accountId)) hasAcctId = 1;

            //Subject Description	SuppliedCompany	SuppliedEmail	SuppliedName	SuppliedPhone Status Origin 
            try
            {
                // Create a new sObject of type Contact
                // and fill out its fields.
                sObject ticket = new SFAccess.sObject();
                System.Xml.XmlElement[] ticketFields = new System.Xml.XmlElement[7 + hasAcctId];

                // Create the ticket's fields
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                ticketFields[0] = doc.CreateElement("Subject");
                ticketFields[0].InnerText = Subject;
                ticketFields[1] = doc.CreateElement("Description");
                ticketFields[1].InnerText = Description;
                ticketFields[2] = doc.CreateElement("SuppliedCompany");
                ticketFields[2].InnerText = SuppliedCompany;
                ticketFields[3] = doc.CreateElement("SuppliedEmail");
                ticketFields[3].InnerText = SuppliedEmail;
                ticketFields[4] = doc.CreateElement("SuppliedName");
                ticketFields[4].InnerText = SuppliedName;
                ticketFields[5] = doc.CreateElement("SuppliedPhone");
                ticketFields[5].InnerText = SuppliedPhone;
                ticketFields[6] = doc.CreateElement("Origin");
                ticketFields[6].InnerText = "Web";

                //if we have an account id, add it to the mix to assign it to an account
                if (hasAcctId == 1)
                {
                    ticketFields[7] = doc.CreateElement("AccountId");
                    ticketFields[7].InnerText = accountId;
                }



                    ticket.type = "Case";
                ticket.Any = ticketFields;

                // Add this sObject to an array
                sObject[] ticketList = new sObject[1];
                ticketList[0] = ticket;

                // Make a create call and pass it the array of sObjects 
                SaveResult[] results = binding.create(ticketList);
                // Iterate through the results list
                // and write the ID of the new sObject
                // or the errors if the object creation failed.
                // In this case, we only have one result
                // since we created one ticket.
                for (int j = 0; j < results.Length; j++)
                {
                   
                    if (results[j].success)
                    {
                       messages.Add("A ticket was created with an ID of: "
                                        + results[j].id);
                        idcreated = results[j].id;
                        success = true;

                    }
                    else
                    {
                        // There were errors during the create call,
                        // go through the errors array and write
                        // them to the console
                        for (int i = 0; i < results[j].errors.Length; i++)
                        {
                            Error err = results[j].errors[i];
                            messages.Add("Errors were found on item " + j.ToString());
                            messages.Add("Error code is: " + err.statusCode.ToString());
                            messages.Add("Error message: " + err.message);
                        }
                        
                    }
                }
            }
            catch (SoapException e)
            {
                messages.Add("An unexpected error has occurred: " + e.Message);
                messages.Add("Stack trace: " + e.StackTrace);
            }
            //now logout
            logout();
            return idcreated;



        }
        
        private void logout()
        {
            try
            {
                binding.logout();
                Console.WriteLine("Logged out.");
            }
            catch (SoapException e)
            {
                // Write the fault code to the console 
                Console.WriteLine(e.Code);

                // Write the fault message to the console 
                Console.WriteLine("An unexpected error has occurred: " + e.Message);

                // Write the stack trace to the console 
                Console.WriteLine(e.StackTrace);
            }
        }
        private bool login(out ArrayList messages)
        {
            messages = new ArrayList();
            //Console.Write("Enter username: ");
            //string username = "bplaster@completecsm.com";// Console.ReadLine();
            // Console.Write("Enter password: ");
            //string password = "P@ssw0rd10ESuICWpWj4sAcJLOhplSGiaj";// Console.ReadLine();


           
            // Create a service object 
            binding = new SforceService();
            

            // Timeout after a minute 
            binding.Timeout = 60000;

            // Try logging in   
            LoginResult lr;
            try
            {

                Console.WriteLine("\nLogging in...\n");
                lr = binding.login(username, password);
            }

            // ApiFault is a proxy stub generated from the WSDL contract when     
            // the web service was imported 
            catch (SoapException e)
            {
                // Write the fault code to the console 
                Console.WriteLine(e.Code);

                // Write the fault message to the console 
                Console.WriteLine("An unexpected error has occurred: " + e.Message);

                // Write the stack trace to the console 
                Console.WriteLine(e.StackTrace);

                //why not successful
                messages.Add(e.Message);
                // Return False to indicate that the login was not successful 
                return false;

            }



            // Check if the password has expired 
            if (lr.passwordExpired)
            {
                Console.WriteLine("An error has occurred. Your password has expired.");
                messages.Add("An error has occurred. Your password has expired.")
;
                return false;
            }


            /** Once the client application has logged in successfully, it will use
             * the results of the login call to reset the endpoint of the service
             * to the virtual server instance that is servicing your organization
             */
            // Save old authentication end point URL
            String authEndPoint = binding.Url;
            // Set returned service endpoint URL
            binding.Url = lr.serverUrl;

            /** The sample client application now has an instance of the SforceService
             * that is pointing to the correct endpoint. Next, the sample client
             * application sets a persistent SOAP header (to be included on all
             * subsequent calls that are made with SforceService) that contains the
             * valid sessionId for our login credentials. To do this, the sample
             * client application creates a new SessionHeader object and persist it to
             * the SforceService. Add the session ID returned from the login to the
             * session header
             */
            binding.SessionHeaderValue = new SessionHeader();
            binding.SessionHeaderValue.sessionId = lr.sessionId;

            //printUserInfo(lr, authEndPoint);

            // Return true to indicate that we are logged in, pointed  
            // at the right URL and have our security token in place.     
            return true;
        }
        public void createSample()
        {
            try
            {
                // Create a new sObject of type Contact
                // and fill out its fields.
                sObject contact = new SFAccess.sObject();
                System.Xml.XmlElement[] contactFields = new System.Xml.XmlElement[6];

                // Create the contact's fields
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                contactFields[0] = doc.CreateElement("FirstName");
                contactFields[0].InnerText = "Otto";
                contactFields[1] = doc.CreateElement("LastName");
                contactFields[1].InnerText = "Jespersen";
                contactFields[2] = doc.CreateElement("Salutation");
                contactFields[2].InnerText = "Professor";
                contactFields[3] = doc.CreateElement("Phone");
                contactFields[3].InnerText = "(999) 555-1234";
                contactFields[4] = doc.CreateElement("Title");
                contactFields[4].InnerText = "Philologist";

                contact.type = "Contact";
                contact.Any = contactFields;

                // Add this sObject to an array
                sObject[] contactList = new sObject[1];
                contactList[0] = contact;

                // Make a create call and pass it the array of sObjects 
                SaveResult[] results = binding.create(contactList);
                // Iterate through the results list
                // and write the ID of the new sObject
                // or the errors if the object creation failed.
                // In this case, we only have one result
                // since we created one contact.
                for (int j = 0; j < results.Length; j++)
                {
                    if (results[j].success)
                    {
                        Console.Write("\nA contact was created with an ID of: "
                                        + results[j].id);
                    }
                    else
                    {
                        // There were errors during the create call,
                        // go through the errors array and write
                        // them to the console
                        for (int i = 0; i < results[j].errors.Length; i++)
                        {
                            Error err = results[j].errors[i];
                            Console.WriteLine("Errors were found on item " + j.ToString());
                            Console.WriteLine("Error code is: " + err.statusCode.ToString());
                            Console.WriteLine("Error message: " + err.message);
                        }
                    }
                }
            }
            catch (SoapException e)
            {
                Console.WriteLine("An unexpected error has occurred: " + e.Message +
                    " Stack trace: " + e.StackTrace);
            }
        }

    }
}