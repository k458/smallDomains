using System.Numerics;
using Entity.EntityProperties;
using Entity.Thinker;

namespace Entity;

public interface IEntity
{
    Vector2 Position2D { get; set; }
    IEntityProperties Properties { get; }
    IEntityThinker Thinker { get; set; }

    void Update();
    void UpdateRare();
}
