using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// Summary description for WebLinks
/// </summary>
/// 
namespace CCSM.BusinessObjects
{
    public class TrainingLink
    {
        public TrainingLink() { }

        private int traininglinkId;
        public int TraininglinkId
        {
            get { return traininglinkId; }
            set { traininglinkId = value; }
        }
        private string traininglinkName;
        public string TraininglinkName
        {
            get { return traininglinkName; }
            set { traininglinkName = value; }
        }
        private string traininglinkImage;
        public string TraininglinkImage
        {
            get { return traininglinkImage; }
            set { traininglinkImage = value; }
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
        private int initialOrdering;
        public int InitialOrdering
        {
            get { return initialOrdering; }
            set { initialOrdering = value; }
        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.CompanyId);
            sb.Append(", ");
            sb.Append(this.InitialOrdering);
            sb.Append(", ");
            sb.Append(this.SearchKeywords);
            sb.Append(", ");
            sb.Append(this.TraininglinkId);
            sb.Append(", ");
            sb.Append(this.TraininglinkImage);
            sb.Append(", ");
            sb.Append(this.TraininglinkName);
            return sb.ToString();
        }



        /*
         * 
         * 
         *   [TraininglinkId]
        INT NOT NULL,
    [TraininglinkName]  NVARCHAR(50)  NULL,
    [Traininglink]      NVARCHAR(500) NULL,
    [TraininglinkImage] NVARCHAR(50)  NULL,
    [CompanyId]
        INT NULL,
    [SearchKeywords]    NVARCHAR(500) NULL,
    [InitialOrdering]


                //private Dictionary<string, string> dict = new Dictionary<string, string>();
       public void addLink(string linkDesc, string link)
       {
           dict.Add(linkDesc, link);
       }

       public Dictionary<string, string> getLinks()
       {
           return dict;
       }
       public override string ToString()
       {
           StringBuilder sb = new StringBuilder();
           foreach (KeyValuePair<string, string> pair in dict)
           {
               sb.Append(pair.Key.ToString() + "  -  " + pair.Value.ToString() + "<br>");
           }
           return sb.ToString();
       }


       dict.Add("one", 1);
       dict.Add("two", 2);
       dict.Add("three", 3);   
       dict.Add("four", 4);

       reading from this
       foreach (KeyValuePair<string, int> pair in dict)
       {
           MessageBox.Show(pair.Key.ToString ()+ "  -  "  + pair.Value.ToString () );
       }

   */

    }
}