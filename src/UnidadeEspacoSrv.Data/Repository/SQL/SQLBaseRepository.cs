using Microsoft.EntityFrameworkCore;
using UnidadeEspacoSrv.Data.Contexto;
using UnidadeEspacoSrv.Domain.Interfaces.SQL;

namespace UnidadeEspacoSrv.Data.Repository.SQL
{

    /// <summary>
    /// SQLBaseRepository: Implementação genérica do repositório para operações CRUD utilizando SQL e Entity Framework Core.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class SQLBaseRepository<TEntity> 
        : ISQLBaseRepository<TEntity> where TEntity : class
    {
        private readonly SQLDbContext _context;

        public SQLBaseRepository(SQLDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GetAllAsync: Recupera todas as entidades do tipo TEntity do banco
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        /// <summary>
        /// GetByIdAsync: Recupera uma entidade do tipo TEntity pelo seu ID. Retorna null se a entidade não for encontrada.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<TEntity> GetByIdAsync(int id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }


        /// <summary>
        /// AddAsync: Adiciona uma nova entidade do tipo TEntity ao banco de dados. Retorna a entidade adicionada.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            var retorno = await _context.Set<TEntity>().AddAsync(entity);
            return retorno.Entity;
        }

        /// <summary>
        /// UpdateAsync: Atualiza uma entidade existente do tipo TEntity no banco de dados. Retorna a entidade atualizada. Lança uma exceção se a entidade não for encontrada.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<TEntity> UpdateAsync(TEntity entity, object id)
        {
            var set = _context.Set<TEntity>();

            var existingEntity = await set.FindAsync(id);

            if (existingEntity == null)
                throw new Exception("A entidade não foi encontrada");

            // Copia os valores para a entidade rastreada
            _context.Entry(existingEntity).CurrentValues.SetValues(entity);

            // Garante que o estado é Modified
            _context.Entry(existingEntity).State = EntityState.Modified;

            return existingEntity;
        }

        /// <summary>
        /// DeleteAsync: Remove uma entidade do tipo TEntity do banco de dados pelo seu ID. Lança uma exceção se a entidade não for encontrada.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.Set<TEntity>().Remove(entity);
            }
        }

        /// <summary>
        /// SaveChangesAsync: Persiste as alterações feitas no contexto do Entity Framework Core no banco de dados. Retorna o número de registros afetados.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.Commit();
        }
    }
}
