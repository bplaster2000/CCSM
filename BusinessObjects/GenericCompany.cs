using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// This is the class we use to represent any company on the flow.   For companies that want a customization, 
/// they would also extend the BaseCompany with the company name
/// </summary>
/// 
namespace CCSM.BusinessObjects
{
    public class GenericCompany : BaseCompany
    {
        public GenericCompany(string CompanyGUID) : base(CompanyGUID)
        {
            //create the AnyCompany object which inherits from the BaseCompany.    

        }
        public GenericCompany(int CompanyId) : base(CompanyId)
        {
            //create the AnyCompany object which inherits from the BaseCompany.    

        }
    }
}