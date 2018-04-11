namespace Template.Api.Queries.FindSomething.Version1
{
    using Cqs.Queries;

    public class FindSomethingProjection: IProjection
    {
        public FindSomethingProjection( string id, string name )
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }
    }
}