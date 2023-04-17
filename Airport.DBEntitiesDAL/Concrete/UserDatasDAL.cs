using Airport.DBEntities.Context;
using Airport.DBEntities.Entities;
using Airport.DBEntitiesDAL.Interfaces;
using Airport.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Airport.DBEntitiesDAL.Concrete
{
    public class UserDatasDAL : EfRepo<SporSepetiContext, UserDatas>, IUserDatasDAL
    {
        public UserDatasDAL()
        {

        }


        public void KullaniciRaporuGetir()
        {

        }

        public void Dispose()
        {

        }
 
    }
}
