namespace EntityEffect;

public interface IEntityEffect
{
    void OnAdded(IModifiableProperties modifiableProperties);
    void OnUpdate(IModifiableProperties modifiableProperties, float deltaTime);
}
