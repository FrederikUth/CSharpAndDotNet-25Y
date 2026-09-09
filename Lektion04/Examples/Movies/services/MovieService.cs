using Movies.models;

namespace Movies.services;

public class MovieService
{
    public List<Movie> Movies { get; } =
    [
        new Movie(1, "The Empire Strikes Back", "1980",
        [
            new Actor(10, "Harrison Ford", new DateOnly(1942, 7, 13)),
            new Actor(15, "Carrie Frances Fisher", new DateOnly(1956, 10, 21))
        ]),
        new Movie(2, "The Shawshank Redemption", "1994",
            [new Actor(16, "Tim Robbins", new DateOnly(1958, 10, 16))])
    ];
}