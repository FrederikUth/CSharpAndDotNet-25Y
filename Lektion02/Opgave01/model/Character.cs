namespace Opgave01.model;

public record Character(string FullName, 
                        string NickName,
                        string HogwartsHouse,
                        string InterpretedBy,
                        List<string> Children,
                        string Image,
                        string Birthdate,
                        int Index);