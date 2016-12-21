using System;
using System.Data;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.Xml.Linq;
using CCSM.BusinessObjects;
using System.Text;
using System.Collections;

/// <summary>
/// Summary description for Database
/// </summary>
namespace CCSM.DataAccess
{
    [Serializable]
    public class DatabaseUtils
    {
        private string _connectionString;
        //private System.Web.SessionState.HttpSessionState Session;

        public void Initialize()
        {
            // Initialize data source. Use connection string from configuration.

            if (ConfigurationManager.ConnectionStrings["ConnectionString"] == null ||
              ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString.Trim() == "")
            {
                throw new Exception("A connection string named 'ConnectionString' with a valid connection string " +
                                  "must exist in the <connectionStrings> configuration section for the application.");
            }

            _connectionString = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
        }
        public DatabaseUtils()
        {
            Initialize();
        }

       /* public DatabaseUtils(System.Web.SessionState.HttpSessionState session)
        {
            Initialize();
            Session = session;
        }*/

        public BaseCompany getCompanyByGUID(BaseCompany gcompany)
        {
            if (String.IsNullOrEmpty(gcompany.CompanyGUID) & gcompany.CompanyId == 0)
            {
                throw new Exception("We need a GUID or a companyId to load the company info");
            }

            return getCompanyInfo(gcompany);
        }
        
        public CUser getUserByEmail(CUser user)
        {
            if (String.IsNullOrEmpty(user.Email)) throw new Exception("we don't have an email to look for");


            string sqlWemail = "select * from Users where Email = @p1";
            DataRow dr = getFirstRow(getDataSetBySqlOneP(sqlWemail, user.Email));
            return mapUser(dr, user);

        }
        public CUser getUserById(CUser user)
        {
            if (user == null || user.UserId == 0) throw new Exception("we don't have a user id to look for");

            string sqlWemail = "select * from Users where UserId = @p1";
            DataRow dr = getFirstRow(getDataSetBySqlOneP(sqlWemail, user.UserId));
            return mapUser(dr, user);

        }
        public BaseCompany getLinkToSupportSignup(BaseCompany company)
        {
            return getCompanyInfo(company);
        }
        private BaseCompany getCompanyInfo(BaseCompany gcompany)
        {
           
            string sqlWId = "select * from Company where CompanyId = @p1";
            string sqlWGuid = "select * from Company where CompanyGUID = @p1";

            DataRow dr;

            if (String.IsNullOrEmpty(gcompany.CompanyGUID))
            {    //get by company ID
                dr = getFirstRow(getDataSetBySqlOneP(sqlWId, gcompany.CompanyId));               
            }else {//get by GUID
                dr = getFirstRow(getDataSetBySqlOneP(sqlWGuid, gcompany.CompanyGUID));
            }

            gcompany.CompanyGUID = returnRealOrBlank(dr["CompanyGUID"] as string);
            gcompany.CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
            gcompany.CompanyName = returnRealOrBlank(dr["companyName"] as string);
            gcompany.CsmContactEmail = returnRealOrBlank(dr["CSMContactPhone"] as string);
            gcompany.MainContactEmail = returnRealOrBlank(dr["MainContactEmail"] as string);
            gcompany.MainContactPhone = returnRealOrBlank(dr["MainContactPhone"] as string);
            gcompany.ProductsSoldDesc = returnRealOrBlank(dr["ProductsSoldDesc"] as string);
            gcompany.SupportContactPhone = returnRealOrBlank(dr["SupportContactPhone"] as string);
            gcompany.SupportContactEmail = returnRealOrBlank(dr["SupportContactEmail"] as string);

            //get the integrations
            Integrations intgr = this.getIntegrations(gcompany);
            gcompany.Intgr = intgr;

            return gcompany;


            /*
            //database access here
            gcompany.CompanyId = 1;
            gcompany.CompanyName = "ACME Bricks";
            gcompany.SupportSignupLink = "Http://mysupport.signup.com";
            //should have been database
            return gcompany;
            */
        }
        public Integrations getIntegrations(BaseCompany company)
        {
            if (company.CompanyId == 0) throw new Exception("we need a company with an ID");

            string sqlWemail = "select * from Integrations where CompanyId = @p1";
            DataRow dr = getFirstRow(getDataSetBySqlOneP(sqlWemail, company.CompanyId));
            return mapIntegration(dr, new Integrations());

        }

       
        //get one row of subscr info
       /* public Subscription getSubscription(CUser user)
        {
            if (user == null)
                throw new Exception("need a user to find a subscription");
            string sql = "Select SubscId, SubscStart, SubscEnd, SubscProductsMsg, CurrentStage, UserId from Subscriptions " +
                "where UserId = @p1 ";
            return mapSubscription(getDataSetBySql(sql, user.UserId), user);
       }*/
      

        //initial links ordered by the initial ordering.   if it is 0 then it is not initial
        public TrainingLink[] getInitialTrainingLinks(BaseCompany company)
        {
            string sql = "Select TrainingLinkId, TraininglinkName, Traininglink, TraininglinkImage, CompanyId, SearchKeywords, InitialOrdering from TrainingLinks " + 
                "where CompanyId = @p1 and InitialOrdering <> 0 order by InitialOrdering ASC";
            return mapTrainingLinks(getDataSetBySql(sql, company.CompanyId));
            
        }

        //initial links ordered by the initial ordering.   if it is 0 then it is not initial
        public TrainingLink getFirstTrainingLink(BaseCompany company)
        {
            string sql = "Select TrainingLinkId, TraininglinkName, Traininglink, TraininglinkImage, CompanyId, SearchKeywords, InitialOrdering from TrainingLinks " +
                "where CompanyId = @p1 and InitialOrdering = 1 order by InitialOrdering ASC";
            TrainingLink[] tl =  mapTrainingLinks(getDataSetBySql(sql, company.CompanyId));
            
            
            //if no first one set, return null
            if(tl != null && tl.Length > 0)
            {
                return tl[0];
            }
            return null;
        }


        /// <summary>
        /// get the keywords from the SearchKeyWord column of the company, separate them out by the commas, and get rid of duplicates
        /// </summary>
        /// <param name="company"></param>
        /// <returns>a distrinct list of keywords to search based on the training links</returns>
        public string[] getTrainingKeywords(BaseCompany company)
        {
            string sql = "select * from TrainingLinks where CompanyId = @p1";

            TrainingLink[] tls = mapTrainingLinks(getDataSetBySql(sql, company.CompanyId));

            ArrayList keywords = new ArrayList();

            foreach (TrainingLink tl in tls)
            {
                string[] kws = tl.SearchKeywords.Split(',');

                foreach (string kw in kws) {
                    if (!keywords.Contains(kw))
                    {
                        keywords.Add(kw);
                    }
                }              
            }

            if (keywords.Count > 0)
                return (string[])keywords.ToArray(typeof(string));
            else
                return new string[1] { "No Keywords to search on!" };
        }
        /// <summary>
        /// get the keywords from the SearchKeyWord column of the company, separate them out by the commas, and get rid of duplicates
        /// </summary>
        /// <param name="company"></param>
        /// <returns>a distrinct list of keywords to search based on the training links</returns>
        public string[] getSupportKeywords(BaseCompany company)
        {
            string sql = "select * from SupportLinks where CompanyId = @p1";

            SupportLink[] exps = mapSupportLinks(getDataSetBySql(sql, company.CompanyId));

            ArrayList keywords = new ArrayList();

            foreach (SupportLink es in exps)
            {
                string[] kws = es.SearchKeywords.Split(',');

                foreach (string kw in kws)
                {
                    if (!keywords.Contains(kw))
                    {
                        keywords.Add(kw);
                    }
                }
            }

            if (keywords.Count > 0)
                return (string[])keywords.ToArray(typeof(string));
            else
                return new string[1] { "No Keywords to search on!" };
        }



        /// <summary>
        /// based on the keywords sent in, return the training links that match.
        /// </summary>
        /// <param name="company"></param>
        /// <param name="keywords"></param>
        /// <returns></returns>
        public TrainingLink[] getUserSearchedTrainingLinks(BaseCompany company, string[] keywords)
        {

            StringBuilder sql = new StringBuilder("Select distinct TrainingLinkId, TraininglinkName, Traininglink, TraininglinkImage, CompanyId, SearchKeywords, InitialOrdering from TrainingLinks ");
            sql.Append("where CompanyId = @p1 and (");
            int size = keywords.Length;
            int index = 0;

            //build the OR clause
            foreach (string s in keywords)
            {
                index++;
                sql.Append("SearchKeywords like '%").Append(s).Append("%' ");
                if (index < size)
                    sql.Append(" OR ");
                else
                    sql.Append(" ) ");
            }
            return mapTrainingLinks(getDataSetBySql(sql.ToString(), company.CompanyId));

        }
        
        public ServiceOffLink[] getUserSearchedServiceOffLink(BaseCompany company, string[] keywords)
        {
            StringBuilder sql =
                new StringBuilder("Select distinct ServiceOffId, ServiceOffName, ServiceOffCost, ServiceOffLink,ServiceOffImage, CompanyId,SearchKeywords from ServiceOfferings ");
            
            sql.Append(" where CompanyId = @p1 and (");
            int size = keywords.Length;
            int index = 0;

            //build the OR clause
            foreach (string s in keywords)
            {
                index++;
                sql.Append("SearchKeywords like '%").Append(s).Append("%' ");
                if (index < size)
                    sql.Append(" OR ");
                else
                    sql.Append(" ) ");
            }
            return mapServiceOffLinks(getDataSetBySql(sql.ToString(),company.CompanyId));
        }

        public ExpertTimeSlot[] getUserSearchedExpertSlot(BaseCompany company, string[] keywords)
        {
            StringBuilder sql =
                new StringBuilder("Select distinct CompanyId, ExpertTimeSlotId, ExpertTimeSlotDesc, ExpertTimeSlotName, ExpertTimeSlotDate, ExpertTimeSlotType,SearchKeywords from ExpertTimeSlots ");

            sql.Append(" where CompanyId = @p1 and (");
            int size = keywords.Length;
            int index = 0;

            //build the OR clause
            foreach (string s in keywords)
            {
                index++;
                sql.Append("SearchKeywords like '%").Append(s).Append("%' ");
                if (index < size)
                    sql.Append(" OR ");
                else
                    sql.Append(" )  order by ExpertTimeSlotDate ");
            }
            return mapExpertTimeSlots(getDataSetBySql(sql.ToString(), company.CompanyId));
        }

        public SupportLink[] getUserSearchedSupportLinks(BaseCompany company, string[] keywords)
        {
            StringBuilder sql = 
                new StringBuilder("Select distinct SupportlinkId, SupportlinkName, Supportlink, SupportlinkImage, CompanyId, SearchKeywords from SupportLinks ");
            sql.Append(" where CompanyId = @p1 and (");
            int size = keywords.Length;
            int index = 0;

            //build the OR clause
            foreach (string s in keywords)
            {
                index++;
                sql.Append("SearchKeywords like '%").Append(s).Append("%' ");
                if (index < size)
                    sql.Append(" OR ");
                else
                    sql.Append(" ) ");
            }
            return mapSupportLinks(getDataSetBySql(sql.ToString(), company.CompanyId));
        }

        /// <summary>
        /// Get the support links - all of them - top 10
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public SupportLink[] getSupportLinks(BaseCompany company)
        {
            StringBuilder sql = new StringBuilder("Select * from SupportLinks where CompanyId = @p1 order by SupportLinkName");
            return mapSupportLinks(getDataSetBySql(sql.ToString(), company.CompanyId));
        }

        
        public SupportQuestion getSupportQuestions(BaseCompany company)
        {
            StringBuilder sql = new StringBuilder("Select * from SupportQuestions where CompanyId = @p1");
            return mapSupportQuestions(getDataSetBySql(sql.ToString(), company.CompanyId));
        }



        public ServiceOffLink[] getServiceOfferings(BaseCompany company)
        {
            string sql = "Select * " + 
                "from ServiceOfferings where companyId = @p1 order by ServiceOffName";
            return mapServiceOffLinks(getDataSetBySql(sql, company.CompanyId));           
        }

        public ExpertTimeSlot[] getExpertTimeSlots(BaseCompany company)
        {
            string sql = "Select CompanyId, ExpertTimeSlotId, ExpertTimeSlotDesc, ExpertTimeSlotName, ExpertTimeSlotDate, ExpertTimeSlotType,SearchKeywords from ExpertTimeSlots " +
                "where CompanyId = @p1 order by ExpertTimeSlotDate";
            return mapExpertTimeSlots(getDataSetBySql(sql, company.CompanyId));
        }

        /// <summary>
        /// gets instructor led training  -  time slot type 2
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        public ExpertTimeSlot[] getInstructorLedTraining(BaseCompany company)
        {
            string sql = "Select * from ExpertTimeSlots where CompanyId = @p1 and ExpertTimeSlotType = '2' order by ExpertTimeSlotDate";
            return mapExpertTimeSlots(getDataSetBySql(sql, company.CompanyId));
        }



        /*********************************************************************************************************************************************************
         * 
         * 
         * The mappings and private methods
         * 
         * 
         * 
         * 
         * ********************************************************************************************************************************************************/


        /*
    [SupportQuestionsId]
    INT NOT NULL PRIMARY KEY, 
[SupportQuestion1] NVARCHAR(50) NULL, 
[SupportQuestion2] NVARCHAR(50) NULL, 
[SupportQuestion3] NVARCHAR(50) NULL, 
[SupportQuestion4] NVARCHAR(50) NULL, 
[SupportQuestion5] NVARCHAR(50) NULL, 
[CompanyId]
    INT NULL
    */
        private SupportQuestion mapSupportQuestions(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null)
                return null;

            //create an array of same size
            SupportQuestion[] links = new SupportQuestion[ds.Tables[0].Rows.Count];
            int index = 0;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SupportQuestion tl = new SupportQuestion();
                tl.CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
                tl.SupportQuestionId = (dr["SupportQuestionId"] != DBNull.Value ? Convert.ToInt16(dr["SupportQuestionId"]) : 0);
                tl.SupportQuestion1 = returnRealOrBlank(dr["SupportQuestion1"] as string);
                tl.SupportQuestion2 = returnRealOrBlank(dr["SupportQuestion2"] as string);
                tl.SupportQuestion3 = returnRealOrBlank(dr["SupportQuestion3"] as string);
                tl.SupportQuestion4 = returnRealOrBlank(dr["SupportQuestion4"] as string);
                tl.SupportQuestion5 = returnRealOrBlank(dr["SupportQuestion5"] as string);

                links[index] = tl;
                index++;
            }
            if (ds.Tables[0].Rows.Count == 0)
                return new SupportQuestion();
            else
                return links[0];//just one
        }

        private Integrations mapIntegration(DataRow dr, Integrations intgr)
        {
            intgr.SupportSignupLink = returnRealOrBlank(dr["SupportSignUpLink"] as string);
            intgr.SupportSignupLinkInst = returnRealOrBlank(dr["SupportSignUpLinkInstructions"] as string);
            intgr.SFuser = returnRealOrBlank(dr["IntUser"] as string);
            intgr.SFpwd = returnRealOrBlank(dr["IntPwd"] as string);
            intgr.SFToken = returnRealOrBlank(dr["IntToken"] as string);
            intgr.EmailNotifications = returnRealOrBlank(dr["EmailNotifications"] as string);

            intgr.GainsightCTAPriority = returnRealOrBlank(dr["GainsightCTAPriority"] as string);
            intgr.GainsightCTAStatus = returnRealOrBlank(dr["GainsightCTAStatus"] as string);
            intgr.GainsightCTAPlaybook = returnRealOrBlank(dr["GainsightCTAPlaybook"] as string);
            intgr.GainsightCTAReason = returnRealOrBlank(dr["GainsightCTAReason"] as string);
            intgr.GainsightCTAType = returnRealOrBlank(dr["GainsightCTAType"] as string);
            intgr.GainsightCTAUserId = returnRealOrBlank(dr["GainsightCTAUserId"] as string);
            intgr.CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
            intgr.IntegrationId = (dr["IntegrationId"] != DBNull.Value ? Convert.ToInt16(dr["IntegrationId"]) : 0);

            return intgr;
        }
        private CUser mapUser(DataRow dr, CUser user)
        {
            user.Username = returnRealOrBlank(dr["UserName"] as string);
            user.UserId = (dr["UserId"] != DBNull.Value ? Convert.ToInt16(dr["UserId"]) : 0);
            user.Email = returnRealOrBlank(dr["Email"] as string);
            user.ExternalSystemId = returnRealOrBlank(dr["ExternalSystemId"] as string);
            user.TextPhone = returnRealOrBlank(dr["TextPhone"] as string);
            user.OfficePhone = returnRealOrBlank(dr["OfficePhone"] as string);
            user.BusinessName = returnRealOrBlank(dr["BusinessName"] as string);
            user.TargetSegment = returnRealOrBlank(dr["TargetSegment"] as string);
            int CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
            if (user.Company == null)
                user.Company = new GenericCompany(CompanyId);
            else
                user.Company.CompanyId = CompanyId;

            Subscription subsc = new Subscription(user);
            if(dr["SubscStart"] != DBNull.Value)
                subsc.SubscStart = DateTime.Parse(dr["SubscStart"].ToString());
            if (dr["SubscEnd"] != DBNull.Value)
                subsc.SubscEnd = DateTime.Parse(dr["SubscEnd"].ToString());
            
            if (dr["SubscStart"] == DBNull.Value || dr["SubscEnd"] == DBNull.Value)
            {
                user.Subscription = null;
            }
            else
            {
                if (dr["SubscProductsMsg"] == DBNull.Value)
                    subsc.SubscrProductMsg = returnRealOrBlank("<Product info not available>" as string);
                else
                    subsc.SubscrProductMsg = returnRealOrBlank(dr["SubscProductsMsg"] as string);
                user.Subscription = subsc;
            }

            return user;


            /*
            //database access here
            user.OfficePhone = "972-444-5555";
            user.TextPhone = "214-385=9128";
            user.UserId = 1;
            //should have been database

            return user;
            */
        }
        /*
        private Subscription mapSubscription(DataSet ds, CUser user)
        {
            if (ds == null || ds.Tables[0] == null)
                return null;

            DataRow dr = getFirstRow(ds);
            if (dr == null)
                return null;
            //create subscription with the user
            Subscription sb = new Subscription(user);
            sb.SubscId = (dr["SubscId"] != DBNull.Value ? Convert.ToInt16(dr["SubscId"]) : 0);
            sb.SubscStart = DateTime.Parse(dr["SubscStart"].ToString());
            sb.SubscEnd = DateTime.Parse(dr["SubscEnd"].ToString());
            sb.SubscrProductMsg = returnRealOrBlank(dr["SubscProductsMsg"] as string);
            sb.CurrentStage = returnRealOrBlank(dr["CurrentStage"] as string);
            return sb;
        }*/
        private TrainingLink[] mapTrainingLinks(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null)
                return null;

            //create an array of same size
            TrainingLink[] links = new TrainingLink[ds.Tables[0].Rows.Count];
            int index = 0;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                TrainingLink tl = new TrainingLink();
                tl.CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
                tl.InitialOrdering = (dr["InitialOrdering"] != DBNull.Value ? Convert.ToInt16(dr["InitialOrdering"]) : 0);
                tl.SearchKeywords = returnRealOrBlank(dr["SearchKeywords"] as string);
                tl.TraininglinkId = (dr["TrainingLinkId"] != DBNull.Value ? Convert.ToInt16(dr["TrainingLinkId"]) : 0);
                tl.TraininglinkImage = returnRealOrBlank(dr["TraininglinkImage"] as string);
                tl.TraininglinkName = returnRealOrBlank(dr["TraininglinkName"] as string);

                links[index] = tl;
                index++;
            }
            return links;

            /*
           //database access here
           user.OfficePhone = "972-444-5555";
           user.TextPhone = "214-385=9128";
           user.UserId = 1;
           //should have been database

           return user;
           */
        }
        private SupportLink[] mapSupportLinks(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null)
                return null;

            //create an array of same size
            SupportLink[] links = new SupportLink[ds.Tables[0].Rows.Count];
            int index = 0;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                SupportLink tl = new SupportLink();
                tl.CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
                tl.SupportlinkId = (dr["SupportLinkId"] != DBNull.Value ? Convert.ToInt16(dr["SupportLinkId"]) : 0);
                tl.SearchKeywords = returnRealOrBlank(dr["SearchKeywords"] as string);
                tl.SupportlinkImage = returnRealOrBlank(dr["SupportlinkImage"] as string);
                tl.SupportlinkName = returnRealOrBlank(dr["SupportlinkName"] as string);
                tl.Supportlink = returnRealOrBlank(dr["Supportlink"] as string);

                links[index] = tl;
                index++;
            }
            return links;
        }
        private ServiceOffLink[] mapServiceOffLinks(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null)
                return null;

            //create an array of same size
            ServiceOffLink[] links = new ServiceOffLink[ds.Tables[0].Rows.Count];
            int index = 0;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ServiceOffLink s = new ServiceOffLink();
                s.CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
                s.ServiceOffImage = returnRealOrBlank(dr["ServiceOffImage"] as string);
                s.ServiceOffId = (dr["ServiceOffId"] != DBNull.Value ? Convert.ToInt16(dr["ServiceOffId"]) : 0);
                s.ServiceOffName = returnRealOrBlank(dr["ServiceOffName"] as string);
                s.ServiceOffCost = returnRealOrBlank(dr["ServiceOffCost"] as string);
                s.ServiceOffLinker = returnRealOrBlank(dr["ServiceOffLink"] as string);
                s.ServiceOffDesc = returnRealOrBlank(dr["ServiceOffDesc"] as string);
                s.SearchKeywords = returnRealOrBlank(dr["SearchKeywords"] as string);


                links[index] = s;
                index++;              
            }
            return links;
        }
      
        private ExpertTimeSlot[] mapExpertTimeSlots(DataSet ds)
        {
            if (ds == null || ds.Tables[0] == null)
                return null;

            //create an array of same size
            ExpertTimeSlot[] slots = new ExpertTimeSlot[ds.Tables[0].Rows.Count];
            int index = 0;

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ExpertTimeSlot tl = new ExpertTimeSlot();
                tl.CompanyId = (dr["CompanyId"] != DBNull.Value ? Convert.ToInt16(dr["CompanyId"]) : 0);
                tl.ExpSlotId = (dr["ExpertTimeSlotId"] != DBNull.Value ? Convert.ToInt16(dr["ExpertTimeSlotId"]) : 0);
                tl.ExpSlotDate = DateTime.Parse(dr["ExpertTimeSlotDate"].ToString());
                tl.ExpSlotDesc = returnRealOrBlank(dr["ExpertTimeSlotDesc"] as string);
                tl.ExpSlotName = returnRealOrBlank(dr["ExpertTimeSlotName"] as string);
                int stype = (dr["ExpertTimeSlotType"] != DBNull.Value ? Convert.ToInt16(dr["ExpertTimeSlotType"]) : 0);

                if (stype == 1) tl.ExpSlotType = "Webinar";
                if (stype == 2) tl.ExpSlotType = "Training Course";
                if (stype == 3) tl.ExpSlotType = "Expert Session";

                slots[index] = tl;
                index++;
            }
            return slots;
        }
       
        
       



        //example to to update with no response
        //fire and forget
        public void ExecuteNonQuery(string sqlCmd)
        {

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand cmd = new SqlCommand(sqlCmd, conn);

            try
            {
                conn.Open();
                cmd.ExecuteNonQuery();
            }
            catch (SqlException exx)
            {
                throw exx;
            }
            finally
            {
                conn.Close();
            }
        }

        
        public DataSet getDataSetBySql(string sqlCmd, int p1)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlCmd;
            cmd.CommandType = CommandType.Text;

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@p1";
            parameter.SqlDbType = SqlDbType.Int;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = p1;
            cmd.Parameters.Add(parameter);

            //add select command
            adapter.SelectCommand = cmd;

            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adapter.Fill(ds);

            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        public DataSet getDataSetBySqlOneP(string sqlCmd, int p1)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlCmd;
            cmd.CommandType = CommandType.Text;

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@p1";
            parameter.SqlDbType = SqlDbType.Int;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = p1;
            cmd.Parameters.Add(parameter);

            //add select command
            adapter.SelectCommand = cmd;

            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adapter.Fill(ds);
                
            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }
        public DataSet getDataSetBySqlOneP(string sqlCmd, string p1)
        {
            SqlConnection conn = new SqlConnection(_connectionString);
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlCmd;
            cmd.CommandType = CommandType.Text;

            SqlParameter parameter = new SqlParameter();
            parameter.ParameterName = "@p1";
            parameter.SqlDbType = SqlDbType.VarChar;
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = p1;
            cmd.Parameters.Add(parameter);

            //add select command
            adapter.SelectCommand = cmd;

            DataSet ds = new DataSet();

            try
            {
                conn.Open();
                adapter.Fill(ds);

            }
            catch (SqlException e)
            {
                throw e;
            }
            finally
            {
                conn.Close();
            }

            return ds;
        }

        //return the raw datarow
        private DataRow getFirstRow(DataSet ds)
        {
            foreach (DataTable dt in ds.Tables)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    return dr;//return the first one
                }
            }

            return null; //if no records
        }

       

        private string returnRealOrBlank(string s )
        {
            if (String.IsNullOrEmpty(s))
                return "";
            else
                return s;
        }
        


    }
}