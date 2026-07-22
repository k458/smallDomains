namespace Entity.Thinker;

public interface IEntityThinker
{
    void Think(IEntity entity);
    void ThinkRare(IEntity entity);
}
