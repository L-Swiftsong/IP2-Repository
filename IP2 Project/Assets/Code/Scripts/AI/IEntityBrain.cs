using System;

public interface IEntityBrain
{
    public event Action<Weapon, int> OnSwappedWeapon; // Weapon: newWeapon; int: weaponIndex;
}
