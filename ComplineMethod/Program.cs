var project1 = new Project("Project1");
var project2 = new Project("Project2");
var project3 = new Project("Project3");
var project4 = new Project("Project4");
var project5 = new Project("Project5");
var project6 = new Project("Project6");
var project7 = new Project("Project7");
var project8 = new Project("Project8");
var project9 = new Project("Project9");

project2.Children.Add(project1);

project4.Children.Add(project1);
project4.Children.Add(project2);
project4.Children.Add(project3);

project5.Children.Add(project1);
project5.Children.Add(project4);

project7.Children.Add(project6);

project8.Children.Add(project1);
project8.Children.Add(project4);
project8.Children.Add(project5);
project8.Children.Add(project6);

project9.Children.Add(project8);

var projects = new List<Project> { project1, project2, project3, project4, project5, project6, project7, project8, project9 };

await MultiImplementation_Execete(projects);

Console.ReadKey();

static async Task MultiImplementation_Execete(List<Project> projects)
{
    var groupProjects = projects.Where(x => !x.Compiled).GroupBy(
                            p => p.Order,
                            p => p,
                            (key, g) => new { OrderNumber = key, Projects = g.ToList() })
                        .OrderBy(i => i.OrderNumber);

    foreach (var item in groupProjects)
    {
        var taskList = item.Projects.Select(i => i.Compile());
        await Task.WhenAll(taskList);
    }
}

internal class Project
{
    public string Name { get; set; }
    public bool Compiled { get; private set; } = false;
    public int Order => Children.MaxBy(i => i.Order)?.Order + 1 ?? 1;

    public Project(string name)
    {
        Name = name;
        Children = new();
    }

    public List<Project> Children { get; set; }

    public async Task Compile()
    {
        if (Compiled) return;

        var allChildrenCompiled = Children.All(i => i.Compiled);

        if (!allChildrenCompiled) return;

        await Task.Delay(100);
        Compiled = true;
        Console.WriteLine($"{this} compiled");
    }

    public override string ToString()
    {
        return $"Project: {Name}({Order})";
    }
}