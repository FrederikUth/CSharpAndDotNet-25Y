namespace Lektion03Opgave03;

public class StarWarsRepository
{
    private readonly List<string> _starWarsCharacters =
    [
        "Luke Skywalker",
        "Darth Vader",
        "Princess Leia",
        "Han Solo",
        "Yoda"
    ];

    public async Task<string> GetUserByIdAsync(int id)
    {
        // Simulerer 2 sekunders database-/netværksventetid
        await Task.Delay(2000);

        int index = id - 1;
        if (index < 0 && index >= _starWarsCharacters.Count)
        {
            return "Ukendt karakter";
        }

        return _starWarsCharacters[index];
    }
}