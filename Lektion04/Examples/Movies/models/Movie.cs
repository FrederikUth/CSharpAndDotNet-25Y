namespace Movies.models;

public record Movie(int Id, string Name, string ReleaseDate, List<Actor> Actors);