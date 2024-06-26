using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Airport.Data;

public interface ISelectableRepo<T> : IRepo<T> where T : class,IEntity
{
    List<T> Select();
    T SelectByID(object Id);
    List<T> SelectByFunc(Func<T,bool> whereCondition);
    ImmutableList<T> SelectByFuncPer(Func<T, bool> whereCondition);

}
