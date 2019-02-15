
public class CardData
{
    public readonly string id;
    public readonly string name;

    public enum Type { Fast, Shield}
    public readonly Type type;

    public readonly float point;

    //public enum Rarelity { NN= 0, NR = 1, SR = 2, UR = 3,}
    //public readonly Rarelity rarelity;
    public readonly string rarelity;

    public readonly string textureDataName;

    public CardData (string id, string name, Type type, float point, string rarelity, string textureDataName)
    {
        this.id = id;
        this.name = name;
        this.type = type;
        this.point = point;
        this.rarelity = rarelity;
        this.textureDataName = textureDataName;
    }
}