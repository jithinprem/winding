using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using windingApi.Controller.Repository.RepositoryInterfaces;
using windingApi.Data;
using windingApi.Models;

namespace windingApi.Controller.Repository;

public class TagDefinitionRepository: Repository<TagDefinition, int>, ITagDefinitionRepository
{
    private IdContext _context;
    private DbSet<TagDefinition> _dbSet;
    public TagDefinitionRepository(IdContext context) : base(context)
    {
        this._dbSet = context.TagDefinitions;
    }
    

    public async Task<TagDefinition> FindTagDefintionWithTagName(string tag)
    {
        var tagDefinition = await _dbSet.FirstOrDefaultAsync(tg => string.Equals(tg.TagName, tag));
        return tagDefinition;
    }
}