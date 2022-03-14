# Trackily
[Trackily](https://trackily.azurewebsites.net) is an issue tracker web application that supports the development process through tickets to track issues and feature requests. I developed Trackily in order to get my feet wet with full-stack web development. 

My primary learning goals for this project were to:
- Create a web app end-to-end using an MVC architecture and deploy it
- Implement CRUD actions with an ORM and a relational database
- Implement authentication & authorization

**Tech stack:** ASP.NET Core, EF Core, ASP.NET Core Identity, Razor Pages, Bootstrap, TinyMCE, MSSQL (LocalDB dev, Azure prod) 

## What can I do with Trackily?

- Create projects, which can represent an entire app or a feature of an app
- Create tickets which can be used to record feature requests or bug fixes for a project
- Comment on tickets to facilitate discussion related to the ticket contents
- Create accounts that are associated with projects

## What features does Trackily have?

### CRUD Actions
- Projects, tickets, and comments can be created, updated, and deleted
- Deleting an entity will delete any child entities: project → tickets → comments

### Authentication & authorization
- Users must be a member of a project to create or view the tickets of that project
- A ticket or comment can only be edited or deleted by its creator or a Manager-level user
- A project can be edited by its creator or a Manager-level user, but can only be deleted by its creator
