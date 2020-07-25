using System.Collections.Generic;
using System.Linq;
using Trackily.Areas.Identity.Data;
using Trackily.Models.Domain;

namespace Trackily.Services
{
    public class UserProjectService
    {
        private readonly TrackilyContext _context;

        public UserProjectService(TrackilyContext context)
        {
            _context = context;
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

        public void AddMembersToProject(List<string> usernames, Project project)
        {
            foreach (var username in usernames.Where(entry => entry != null))
            {
                var user = _context.Users.Single(u => u.UserName == username);
                var userProject = CreateUserProject(user, project);
                project.Members.Add(userProject);
            }
        }

        public void RemoveMembersFromProject(List<string> usernames, Project project)
        {
            foreach (var username in usernames.Where(entry => entry != null))
            {
                var userProject =
                    _context.UserProjects.Single(up => up.User.UserName == username & up.Project == project);

                project.Members.Remove(userProject);
            }
        }
    }
}
