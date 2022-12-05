using Microsoft.EntityFrameworkCore;

public interface IRepositoryWrapper<T> where T : DbContext{
    T dbContext {get;}
}