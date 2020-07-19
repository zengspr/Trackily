using System.Linq;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;
using Trackily.Services.DataAccess;

namespace Trackily.Services.Business
{
    public class UserProjectService
    {
        private readonly TrackilyContext _context;
        private readonly DbService _dbService;

        public UserProjectService(TrackilyContext context, DbService dbService)
        {
            _context = context;
            _dbService = dbService;
        }

        public UserProject CreateUserProject(TrackilyUser user, Project project)
        {
            return new UserProject()
            {
                Id = user.Id,
                User = user,
                ProjectId = project.ProjectId,
                Project = project
            };
        }

        public void AddMembersToProject(string[] usernames, Project project)
        {
            foreach (var username in usernames.Where(entry => entry != null))
            {
                var user = _dbService.GetUser(username);
                var userProject = CreateUserProject(user, project);
                project.Members.Add(userProject);
            }
        }
    }
}
