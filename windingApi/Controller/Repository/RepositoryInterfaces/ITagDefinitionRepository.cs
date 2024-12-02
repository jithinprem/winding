using System.Threading.Tasks;
using windingApi.Models;

namespace windingApi.Controller.Repository.RepositoryInterfaces;

public interface ITagDefinitionRepository: IRepository<TagDefinition, int>
{
    public Task<TagDefinition> FindTagDefintionWithTagName(string tag);
}