using Airport.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Airport.EF;

public abstract class EfRepo<Tcontext, TEntity> :
    ISelectableRepo<TEntity>, SelectableAsyncRepo<TEntity>,
    IEInsertableRepo<TEntity>, IInsertableAsyncRepo<TEntity>,
    IDeletableRepo<TEntity>,
    IUpdatetableRepo<TEntity>
    where TEntity : class, IEntity
    where Tcontext : DbContext, new()
{
    private readonly Tcontext _context;
    public EfRepo(Tcontext context)
    {
        _context = context;
    }

    public EfRepo()
    {
        _context = new Tcontext();
    }

    public TEntity Insert(TEntity addedData)
    {
        _context.Set<TEntity>().Add(addedData);
        _context.SaveChanges();
        return addedData;
    }

    public async Task InsertAsync(TEntity addedData)
    {
        await _context.Set<TEntity>().AddAsync(addedData);
        await _context.SaveChangesAsync();
    }

    public List<TEntity> InsertRage(List<TEntity> addedListData)
    {
        _context.Set<TEntity>().AddRange(addedListData);
        _context.SaveChanges();
        return addedListData;
    }

    public async Task InsertRangeAsync(List<TEntity> addedListData)
    {
        await _context.Set<TEntity>().AddRangeAsync(addedListData);
        await _context.SaveChangesAsync();
    }

    public void MySaveChanges()
    {
        _context.SaveChanges();
    }

    public List<TEntity> Select()
    {
        return _context.Set<TEntity>().ToList();
    }

    public async Task<List<TEntity>> SelectAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity> SelectByAsync(object Id)
    {
        return await _context.Set<TEntity>().FindAsync(Id);
    }

    public List<TEntity> SelectByFunc(Func<TEntity, bool> whereCondition)
    {
        return _context.Set<TEntity>().Where(whereCondition).ToList();
    }

    public ImmutableList<TEntity> SelectByFuncPer(Func<TEntity, bool> whereCondition)
    {
        return _context.Set<TEntity>().Where(whereCondition).ToImmutableList();
    }

    public TEntity SelectByID(object Id)
    {
        return _context.Set<TEntity>().Find(Id);
    }

    public void HardDelete(TEntity deletedData)
    {
        _context.Set<TEntity>().Remove(deletedData);
        _context.SaveChanges();
    }

    public void SoftDelete(TEntity deletedData)
    {
        deletedData.GetType().GetProperty("IsDelete").SetValue(deletedData, true);
        this.Update(deletedData);
    }

    public TEntity Update(TEntity updatedData)
    {
        _context.Set<TEntity>().Attach(updatedData);
        _context.Entry(updatedData).State = EntityState.Modified;
        _context.SaveChanges();
        return updatedData;
    }       
}
