using Airport.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Airport.EF;

public abstract class EfRepo<Tcontext, TEntity>(Tcontext context) :
    ISelectableRepo<TEntity>, SelectableAsyncRepo<TEntity>,
    IEInsertableRepo<TEntity>, IInsertableAsyncRepo<TEntity>,
    IDeletableRepo<TEntity>,
    IUpdatetableRepo<TEntity>
    where TEntity : class, IEntity
    where Tcontext : DbContext
{

    public TEntity Insert(TEntity addedData)
    {
        context.Set<TEntity>().Add(addedData);
        context.SaveChanges();
        return addedData;
    }

    public async Task InsertAsync(TEntity addedData)
    {
        await context.Set<TEntity>().AddAsync(addedData);
        await context.SaveChangesAsync();
    }

    public List<TEntity> InsertRage(List<TEntity> addedListData)
    {
        context.Set<TEntity>().AddRange(addedListData);
        context.SaveChanges();
        return addedListData;
    }

    public async Task InsertRangeAsync(List<TEntity> addedListData)
    {
        await context.Set<TEntity>().AddRangeAsync(addedListData);
        await context.SaveChangesAsync();
    }

    public void MySaveChanges()
    {
        context.SaveChanges();
    }

    public List<TEntity> Select()
    {
        return context.Set<TEntity>().ToList();
    }

    public async Task<List<TEntity>> SelectAsync()
    {
        return await context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity> SelectByAsync(object Id)
    {
        return await context.Set<TEntity>().FindAsync(Id);
    }

    public List<TEntity> SelectByFunc(Func<TEntity, bool> whereCondition)
    {
        return context.Set<TEntity>().Where(whereCondition).ToList();
    }

    public ImmutableList<TEntity> SelectByFuncPer(Func<TEntity, bool> whereCondition)
    {
        return context.Set<TEntity>().Where(whereCondition).ToImmutableList();
    }

    public TEntity SelectByID(object Id)
    {
        return context.Set<TEntity>().Find(Id);
    }

    public void HardDelete(TEntity deletedData)
    {
        context.Set<TEntity>().Remove(deletedData);
        context.SaveChanges();
    }

    public void SoftDelete(TEntity deletedData)
    {
        deletedData.GetType().GetProperty("IsDelete").SetValue(deletedData, true);
        this.Update(deletedData);
    }

    public TEntity Update(TEntity updatedData)
    {
        context.Set<TEntity>().Attach(updatedData);
        context.Entry(updatedData).State = EntityState.Modified;
        context.SaveChanges();
        return updatedData;
    }       
}
